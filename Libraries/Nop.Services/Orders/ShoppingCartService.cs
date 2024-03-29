using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
namespace Nop.Services.Orders
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial class ShoppingCartService : IShoppingCartService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IRepository<ShoppingCartItem> _sciRepository;
        private readonly IShippingService _shippingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public ShoppingCartService(CatalogSettings catalogSettings,
            IAclService aclService,
            IActionContextAccessor actionContextAccessor,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IRepository<ShoppingCartItem> sciRepository,
            IShippingService shippingService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _actionContextAccessor = actionContextAccessor;
            _checkoutAttributeParser = checkoutAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
            _currencyService = currencyService;
            _customerService = customerService;
            _dateRangeService = dateRangeService;
            _dateTimeHelper = dateTimeHelper;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _priceFormatter = priceFormatter;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _sciRepository = sciRepository;
            _shippingService = shippingService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
        public virtual void DeleteShoppingCartItem(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true,
            bool ensureOnlyActiveCheckoutAttributes = false)
        {
            if (shoppingCartItem == null)
            {
                throw new ArgumentNullException(nameof(shoppingCartItem));
            }

            Customer customer = shoppingCartItem.Customer;
            int storeId = shoppingCartItem.StoreId;

            //reset checkout data
            if (resetCheckoutData)
            {
                _customerService.ResetCheckoutData(shoppingCartItem.Customer, shoppingCartItem.StoreId);
            }

            //delete item
            _sciRepository.Delete(shoppingCartItem);

            //reset "HasShoppingCartItems" property used for performance optimization
            customer.HasShoppingCartItems = customer.ShoppingCartItems.Any();
            _customerService.UpdateCustomer(customer);

            //validate checkout attributes
            if (ensureOnlyActiveCheckoutAttributes &&
                //only for shopping cart items (ignore wishlist)
                shoppingCartItem.ShoppingCartType == ShoppingCartType.ShoppingCart)
            {
                List<ShoppingCartItem> cart = customer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(storeId)
                    .ToList();

                string checkoutAttributesXml = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CheckoutAttributes, storeId);
                checkoutAttributesXml = _checkoutAttributeParser.EnsureOnlyActiveAttributes(checkoutAttributesXml, cart);
                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CheckoutAttributes, checkoutAttributesXml, storeId);
            }

            //event notification
            _eventPublisher.EntityDeleted(shoppingCartItem);

            if (!_catalogSettings.RemoveRequiredProducts)
            {
                return;
            }

            Product product = _productService.GetProductById(shoppingCartItem.ProductId);
            if (!product?.RequireOtherProducts ?? true)
            {
                return;
            }

            int[] requiredProductIds = _productService.ParseRequiredProductIds(product);
            List<ShoppingCartItem> requiredShoppingCartItems = customer.ShoppingCartItems
                .Where(x => x.ShoppingCartType == shoppingCartItem.ShoppingCartType)
                .Where(item => requiredProductIds.Any(id => id == item.ProductId))
                .ToList();

            //update quantity of required products in the cart if the main one is removed
            foreach (ShoppingCartItem cartItem in requiredShoppingCartItems)
            {
                //at now we ignore quantities of required products and use 1
                int requiredProductQuantity = 1;

                UpdateShoppingCartItem(customer, cartItem.Id, cartItem.AttributesXml, cartItem.CustomerEnteredPrice,
                    quantity: cartItem.Quantity - shoppingCartItem.Quantity * requiredProductQuantity, resetCheckoutData: false);
            }
        }

        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItemId">Shopping cart item ID</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
        public virtual void DeleteShoppingCartItem(int shoppingCartItemId, bool resetCheckoutData = true,
            bool ensureOnlyActiveCheckoutAttributes = false)
        {
            ShoppingCartItem shoppingCartItem = _sciRepository.Table.FirstOrDefault(sci => sci.Id == shoppingCartItemId);
            if (shoppingCartItem != null)
            {
                DeleteShoppingCartItem(shoppingCartItem, resetCheckoutData, ensureOnlyActiveCheckoutAttributes);
            }
        }

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>Number of deleted items</returns>
        public virtual int DeleteExpiredShoppingCartItems(DateTime olderThanUtc)
        {
            IQueryable<ShoppingCartItem> query = from sci in _sciRepository.Table
                                                 where sci.UpdatedOnUtc < olderThanUtc
                                                 select sci;

            List<ShoppingCartItem> cartItems = query.ToList();
            foreach (ShoppingCartItem cartItem in cartItems)
            {
                _sciRepository.Delete(cartItem);
            }

            return cartItems.Count;
        }


        /// <summary>
        /// Added By Sunil Kumar at 13-12-18
        /// Deletes shopping cart items basesd on Product ID
        /// </summary>
        /// <param name="ProductID">ProductID</param>
        /// <returns>Number of deleted items</returns>
        public virtual int DeleteShoppingCartItems(int ProductID, int CustomerID, int shoppingCartTypeId)
        {
            IQueryable<ShoppingCartItem> query = from sci in _sciRepository.Table
                                                 where sci.ProductId == ProductID && sci.CustomerId == CustomerID && sci.ShoppingCartTypeId == shoppingCartTypeId
                                                 select sci;

            List<ShoppingCartItem> cartItems = query.ToList();
            foreach (ShoppingCartItem cartItem in cartItems)
            {
                _sciRepository.Delete(cartItem);
            }

            return cartItems.Count;
        }

        /// <summary>
        /// Validates required products (products which require some other products to be added to the cart)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="addRequiredProducts">Whether to add required products</param>
        /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetRequiredProductWarnings(Customer customer, ShoppingCartType shoppingCartType, Product product,
            int storeId, int quantity, bool addRequiredProducts, int shoppingCartItemId)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            List<string> warnings = new List<string>();

            //at now we ignore quantities of required products and use 1
            int requiredProductQuantity = 1;

            //get customer shopping cart
            List<ShoppingCartItem> cart = customer.ShoppingCartItems
                .Where(item => item.ShoppingCartType == shoppingCartType)
                .LimitPerStore(storeId).ToList();

            //whether other cart items require the passed product
            int passedProductRequiredQuantity = cart
                .Where(item => item.Product.RequireOtherProducts && _productService.ParseRequiredProductIds(item.Product).Contains(product.Id))
                .Sum(item => item.Quantity * requiredProductQuantity);
            if (passedProductRequiredQuantity > quantity)
            {
                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.RequiredProductUpdateWarning"), passedProductRequiredQuantity));
            }

            //whether the passed product requires other products
            if (!product.RequireOtherProducts)
            {
                return warnings;
            }

            //get these required products
            IList<Product> requiredProducts = _productService.GetProductsByIds(_productService.ParseRequiredProductIds(product));
            if (!requiredProducts.Any())
            {
                return warnings;
            }

            //get warnings
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            string warningLocale = _localizationService.GetResource("ShoppingCart.RequiredProductWarning");
            foreach (Product requiredProduct in requiredProducts)
            {
                //get the required quantity of the required product
                int requiredProductRequiredQuantity = quantity * requiredProductQuantity + cart
                    .Where(item => item.Product.RequireOtherProducts && _productService.ParseRequiredProductIds(item.Product).Contains(requiredProduct.Id))
                    .Where(item => item.Id != shoppingCartItemId)
                    .Sum(item => item.Quantity * requiredProductQuantity);

                //whether required product is already in the cart in the required quantity
                int quantityToAdd = requiredProductRequiredQuantity - (cart.FirstOrDefault(item => item.ProductId == requiredProduct.Id)?.Quantity ?? 0);
                if (quantityToAdd <= 0)
                {
                    continue;
                }

                //prepare warning message
                string requiredProductName = WebUtility.HtmlEncode(_localizationService.GetLocalized(requiredProduct, x => x.Name));
                string requiredProductWarning = _catalogSettings.UseLinksInRequiredProductWarnings
                    ? string.Format(warningLocale, $"<a href=\"{urlHelper.RouteUrl(nameof(Product), new { SeName = _urlRecordService.GetSeName(requiredProduct) })}\">{requiredProductName}</a>", requiredProductRequiredQuantity)
                    : string.Format(warningLocale, requiredProductName, requiredProductRequiredQuantity);

                //add to cart (if possible)
                if (addRequiredProducts && product.AutomaticallyAddRequiredProducts)
                {
                    //do not add required products to prevent circular references
                    ErrorListModel addToCartWarnings = AddToCart(customer, requiredProduct, shoppingCartType, storeId,
                        quantity: quantityToAdd, addRequiredProducts: false, count:1);

                    //don't display all specific errors only the generic one
                    if (addToCartWarnings.ErrorList.Any())
                    {
                        warnings.Add(requiredProductWarning);
                    }
                }
                else
                {
                    warnings.Add(requiredProductWarning);
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates a product for standard properties
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetStandardWarnings(Customer customer, ShoppingCartType shoppingCartType,
            Product product, string attributesXml, decimal customerEnteredPrice,
            int quantity, int count)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            List<string> warnings = new List<string>();

            //deleted
            if (product.Deleted)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ProductDeleted"));
                return warnings;
            }

            //published
            if (!product.Published)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ProductUnpublished"));
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                warnings.Add("This is not simple product");
            }

            //ACL
            if (!_aclService.Authorize(product, customer))
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ProductUnpublished"));
            }

            //Store mapping
            if (!_storeMappingService.Authorize(product, _storeContext.CurrentStore.Id))
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.ProductUnpublished"));
            }

            //disabled "add to cart" button
            if (shoppingCartType == ShoppingCartType.ShoppingCart && product.DisableBuyButton)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.BuyingDisabled"));
            }

            //disabled "add to wishlist" button
            if (shoppingCartType == ShoppingCartType.Wishlist && product.DisableWishlistButton)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.WishlistDisabled"));
            }

            //call for price
            if (shoppingCartType == ShoppingCartType.ShoppingCart && product.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
            {
                warnings.Add(_localizationService.GetResource("Products.CallForPrice"));
            }

            //customer entered price
            if (product.CustomerEntersPrice)
            {
                if (customerEnteredPrice < product.MinimumCustomerEnteredPrice ||
                    customerEnteredPrice > product.MaximumCustomerEnteredPrice)
                {
                    decimal minimumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MinimumCustomerEnteredPrice, _workContext.WorkingCurrency);
                    decimal maximumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MaximumCustomerEnteredPrice, _workContext.WorkingCurrency);
                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.CustomerEnteredPrice.RangeError"),
                        _priceFormatter.FormatPrice(minimumCustomerEnteredPrice, false, false),
                        _priceFormatter.FormatPrice(maximumCustomerEnteredPrice, false, false)));
                }
            }

            //quantity validation
            bool hasQtyWarnings = false;
            if (quantity < product.OrderMinimumQuantity)
            {
                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MinimumQuantity"), product.OrderMinimumQuantity));
                hasQtyWarnings = true;
            }

            if (quantity > product.OrderMaximumQuantity)
            {
                warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumQuantity"), product.OrderMaximumQuantity));
                hasQtyWarnings = true;
            }

            int[] allowedQuantities = _productService.ParseAllowedQuantities(product);

            if (allowedQuantities.Length > 0)
            {
                int maximumValue = allowedQuantities.Max();
                if (quantity > maximumValue)
                {
                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.AllowedQuantities"), string.Join(", ", allowedQuantities)));
                }
            }

            //if (allowedQuantities.Length > 0 && !allowedQuantities.Contains(quantity))
            //{
            //    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.AllowedQuantities"), string.Join(", ", allowedQuantities)));
            //}

            bool validateOutOfStock = shoppingCartType == ShoppingCartType.ShoppingCart || !_shoppingCartSettings.AllowOutOfStockItemsToBeAddedToWishlist;
            if (validateOutOfStock && !hasQtyWarnings)
            {
                switch (product.ManageInventoryMethod)
                {
                    case ManageInventoryMethod.DontManageStock:
                        //do nothing
                        break;
                    case ManageInventoryMethod.ManageStock:
                        if (product.BackorderMode == BackorderMode.NoBackorders)
                        {
                            
                            int maximumQuantityCanBeAdded = _productService.GetTotalStockQuantity(product);
                            if (maximumQuantityCanBeAdded < (count))
                            {
                                if (maximumQuantityCanBeAdded <= 0)
                                {
                                    Core.Domain.Shipping.ProductAvailabilityRange productAvailabilityRange = _dateRangeService.GetProductAvailabilityRangeById(product.ProductAvailabilityRangeId);
                                    string warning = productAvailabilityRange == null ? _localizationService.GetResource("ShoppingCart.OutOfStock")
                                        : string.Format(_localizationService.GetResource("ShoppingCart.AvailabilityRange"),
                                            _localizationService.GetLocalized(productAvailabilityRange, range => range.Name));
                                    warnings.Add(warning);
                                }
                                else
                                {
                                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
                                }
                            }
                        }

                        break;
                    case ManageInventoryMethod.ManageStockByAttributes:
                        ProductAttributeCombination combination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
                        if (combination != null)
                        {
                            //combination exists
                            //let's check stock level
                            if (!combination.AllowOutOfStockOrders && combination.StockQuantity < quantity)
                            {
                                int maximumQuantityCanBeAdded = combination.StockQuantity;
                                if (maximumQuantityCanBeAdded <= 0)
                                {
                                    Core.Domain.Shipping.ProductAvailabilityRange productAvailabilityRange = _dateRangeService.GetProductAvailabilityRangeById(product.ProductAvailabilityRangeId);
                                    string warning = productAvailabilityRange == null ? _localizationService.GetResource("ShoppingCart.OutOfStock")
                                        : string.Format(_localizationService.GetResource("ShoppingCart.AvailabilityRange"),
                                            _localizationService.GetLocalized(productAvailabilityRange, range => range.Name));
                                    warnings.Add(warning);
                                }
                                else
                                {
                                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
                                }
                            }
                        }
                        else
                        {
                            //combination doesn't exist
                            if (product.AllowAddingOnlyExistingAttributeCombinations)
                            {
                                //maybe, is it better  to display something like "No such product/combination" message?
                                Core.Domain.Shipping.ProductAvailabilityRange productAvailabilityRange = _dateRangeService.GetProductAvailabilityRangeById(product.ProductAvailabilityRangeId);
                                string warning = productAvailabilityRange == null ? _localizationService.GetResource("ShoppingCart.OutOfStock")
                                    : string.Format(_localizationService.GetResource("ShoppingCart.AvailabilityRange"),
                                        _localizationService.GetLocalized(productAvailabilityRange, range => range.Name));
                                warnings.Add(warning);
                            }
                        }

                        break;
                    default:
                        break;
                }
            }

            //availability dates
            bool availableStartDateError = false;
            if (product.AvailableStartDateTimeUtc.HasValue)
            {
                DateTime availableStartDateTime = DateTime.SpecifyKind(product.AvailableStartDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableStartDateTime.CompareTo(DateTime.UtcNow) > 0)
                {
                    warnings.Add(_localizationService.GetResource("ShoppingCart.NotAvailable"));
                    availableStartDateError = true;
                }
            }

            if (!product.AvailableEndDateTimeUtc.HasValue || availableStartDateError)
            {
                return warnings;
            }

            DateTime availableEndDateTime = DateTime.SpecifyKind(product.AvailableEndDateTimeUtc.Value, DateTimeKind.Utc);
            if (availableEndDateTime.CompareTo(DateTime.UtcNow) < 0)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.NotAvailable"));
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <param name="ignoreConditionMet">A value indicating whether we should ignore filtering by "is condition met" property</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemAttributeWarnings(Customer customer,
            ShoppingCartType shoppingCartType,
            Product product,
            int quantity = 1,
            string attributesXml = "",
            bool ignoreNonCombinableAttributes = false,
            bool ignoreConditionMet = false)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            List<string> warnings = new List<string>();

            //ensure it's our attributes
            IList<ProductAttributeMapping> attributes1 = _productAttributeParser.ParseProductAttributeMappings(attributesXml);
            if (ignoreNonCombinableAttributes)
            {
                attributes1 = attributes1.Where(x => !x.IsNonCombinable()).ToList();
            }

            foreach (ProductAttributeMapping attribute in attributes1)
            {
                if (attribute.Product != null)
                {
                    if (attribute.Product.Id != product.Id)
                    {
                        warnings.Add("Attribute error");
                    }
                }
                else
                {
                    warnings.Add("Attribute error");
                    return warnings;
                }
            }

            //validate required product attributes (whether they're chosen/selected/entered)
            IList<ProductAttributeMapping> attributes2 = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            if (ignoreNonCombinableAttributes)
            {
                attributes2 = attributes2.Where(x => !x.IsNonCombinable()).ToList();
            }

            //validate conditional attributes only (if specified)
            if (!ignoreConditionMet)
            {
                attributes2 = attributes2.Where(x =>
                {
                    bool? conditionMet = _productAttributeParser.IsConditionMet(x, attributesXml);
                    return !conditionMet.HasValue || conditionMet.Value;
                }).ToList();
            }

            foreach (ProductAttributeMapping a2 in attributes2)
            {
                if (a2.IsRequired)
                {
                    bool found = false;
                    //selected product attributes
                    foreach (ProductAttributeMapping a1 in attributes1)
                    {
                        if (a1.Id != a2.Id)
                        {
                            continue;
                        }

                        IList<string> attributeValuesStr = _productAttributeParser.ParseValues(attributesXml, a1.Id);

                        foreach (string str1 in attributeValuesStr)
                        {
                            if (string.IsNullOrEmpty(str1.Trim()))
                            {
                                continue;
                            }

                            found = true;
                            break;
                        }
                    }

                    //if not found
                    if (!found)
                    {
                        string textPrompt = _localizationService.GetLocalized(a2, x => x.TextPrompt);
                        string notFoundWarning = !string.IsNullOrEmpty(textPrompt) ?
                            textPrompt :
                            string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"), _localizationService.GetLocalized(a2.ProductAttribute, a => a.Name));
                        //-------start---Added by Alexandar Rajavel on 20-Feb-2019
                        string select = _localizationService.GetResource("Please.Select");
                        if (_workContext.WorkingLanguage.Id == 1)
                        {
                            notFoundWarning = textPrompt != null ? select + " " + notFoundWarning : notFoundWarning;
                        }
                        else
                        {
                            notFoundWarning = textPrompt != null ? notFoundWarning + " " + select : notFoundWarning;
                        }
                        //--------end----------
                        warnings.Add(notFoundWarning);
                    }
                }

                if (a2.AttributeControlType != AttributeControlType.ReadonlyCheckboxes)
                {
                    continue;
                }

                //customers cannot edit read-only attributes
                int[] allowedReadOnlyValueIds = _productAttributeService.GetProductAttributeValues(a2.Id)
                    .Where(x => x.IsPreSelected)
                    .Select(x => x.Id)
                    .ToArray();

                int[] selectedReadOnlyValueIds = _productAttributeParser.ParseProductAttributeValues(attributesXml)
                    .Where(x => x.ProductAttributeMappingId == a2.Id)
                    .Select(x => x.Id)
                    .ToArray();

                if (!CommonHelper.ArraysEqual(allowedReadOnlyValueIds, selectedReadOnlyValueIds))
                {
                    warnings.Add("You cannot change read-only values");
                }
            }

            //validation rules
            foreach (ProductAttributeMapping pam in attributes2)
            {
                if (!pam.ValidationRulesAllowed())
                {
                    continue;
                }

                string enteredText;
                int enteredTextLength;

                //minimum length
                if (pam.ValidationMinLength.HasValue)
                {
                    if (pam.AttributeControlType == AttributeControlType.TextBox ||
                        pam.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        enteredText = _productAttributeParser.ParseValues(attributesXml, pam.Id).FirstOrDefault();
                        enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                        if (pam.ValidationMinLength.Value > enteredTextLength)
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.TextboxMinimumLength"), _localizationService.GetLocalized(pam.ProductAttribute, a => a.Name), pam.ValidationMinLength.Value));
                        }
                    }
                }

                //maximum length
                if (!pam.ValidationMaxLength.HasValue)
                {
                    continue;
                }

                if (pam.AttributeControlType != AttributeControlType.TextBox && pam.AttributeControlType != AttributeControlType.MultilineTextbox)
                {
                    continue;
                }

                enteredText = _productAttributeParser.ParseValues(attributesXml, pam.Id).FirstOrDefault();
                enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                if (pam.ValidationMaxLength.Value < enteredTextLength)
                {
                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.TextboxMaximumLength"), _localizationService.GetLocalized(pam.ProductAttribute, a => a.Name), pam.ValidationMaxLength.Value));
                }
            }

            if (warnings.Any())
            {
                return warnings;
            }

            //validate bundled products
            IList<ProductAttributeValue> attributeValues = _productAttributeParser.ParseProductAttributeValues(attributesXml);
            foreach (ProductAttributeValue attributeValue in attributeValues)
            {
                if (attributeValue.AttributeValueType != AttributeValueType.AssociatedToProduct)
                {
                    continue;
                }

                if (ignoreNonCombinableAttributes && attributeValue.ProductAttributeMapping.IsNonCombinable())
                {
                    continue;
                }

                //associated product (bundle)
                Product associatedProduct = _productService.GetProductById(attributeValue.AssociatedProductId);
                if (associatedProduct != null)
                {
                    int totalQty = quantity * attributeValue.Quantity;
                    ErrorListModel associatedProductWarnings = GetShoppingCartItemWarnings(customer,
                        shoppingCartType, associatedProduct, _storeContext.CurrentStore.Id,
                        string.Empty, decimal.Zero, null, null, totalQty, false,1);
                    foreach (string associatedProductWarning in associatedProductWarnings.ErrorList)
                    {
                        string attributeName = _localizationService.GetLocalized(attributeValue.ProductAttributeMapping.ProductAttribute, a => a.Name);
                        string attributeValueName = _localizationService.GetLocalized(attributeValue, a => a.Name);
                        warnings.Add(string.Format(
                            _localizationService.GetResource("ShoppingCart.AssociatedAttributeWarning"),
                            attributeName, attributeValueName, associatedProductWarning));
                    }
                }
                else
                {
                    warnings.Add($"Associated product cannot be loaded - {attributeValue.AssociatedProductId}");
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartItemGiftCardWarnings(ShoppingCartType shoppingCartType,
            Product product, string attributesXml)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            List<string> warnings = new List<string>();

            //gift cards
            if (!product.IsGiftCard)
            {
                return warnings;
            }

            _productAttributeParser.GetGiftCardAttribute(attributesXml, out string giftCardRecipientName, out string giftCardRecipientEmail, out string giftCardSenderName, out string giftCardSenderEmail, out string _);

            if (string.IsNullOrEmpty(giftCardRecipientName))
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.RecipientNameError"));
            }

            if (product.GiftCardType == GiftCardType.Virtual)
            {
                //validate for virtual gift cards only
                if (string.IsNullOrEmpty(giftCardRecipientEmail) || !CommonHelper.IsValidEmail(giftCardRecipientEmail))
                {
                    warnings.Add(_localizationService.GetResource("ShoppingCart.RecipientEmailError"));
                }
            }

            if (string.IsNullOrEmpty(giftCardSenderName))
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.SenderNameError"));
            }

            if (product.GiftCardType != GiftCardType.Virtual)
            {
                return warnings;
            }

            //validate for virtual gift cards only
            if (string.IsNullOrEmpty(giftCardSenderEmail) || !CommonHelper.IsValidEmail(giftCardSenderEmail))
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.SenderEmailError"));
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item for rental products
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetRentalProductWarnings(Product product,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            List<string> warnings = new List<string>();

            if (!product.IsRental)
            {
                return warnings;
            }

            if (!rentalStartDate.HasValue)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.Rental.EnterStartDate"));
                return warnings;
            }

            if (!rentalEndDate.HasValue)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.Rental.EnterEndDate"));
                return warnings;
            }

            if (rentalStartDate.Value.CompareTo(rentalEndDate.Value) > 0)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.Rental.StartDateLessEndDate"));
                return warnings;
            }

            //allowed start date should be the future date
            //we should compare rental start date with a store local time
            //but we what if a store works in distinct timezones? how we should handle it? skip it for now
            //we also ignore hours (anyway not supported yet)
            //today
            DateTime nowDtInStoreTimeZone = _dateTimeHelper.ConvertToUserTime(DateTime.Now, TimeZoneInfo.Local, _dateTimeHelper.DefaultStoreTimeZone);
            DateTime todayDt = new DateTime(nowDtInStoreTimeZone.Year, nowDtInStoreTimeZone.Month, nowDtInStoreTimeZone.Day);
            DateTime todayDtUtc = _dateTimeHelper.ConvertToUtcTime(todayDt, _dateTimeHelper.DefaultStoreTimeZone);
            //dates are entered in store timezone (e.g. like in hotels)
            DateTime startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.DefaultStoreTimeZone);
            //but we what if dates should be entered in a customer timezone?
            //DateTime startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.CurrentTimeZone);
            if (todayDtUtc.CompareTo(startDateUtc) <= 0)
            {
                return warnings;
            }

            warnings.Add(_localizationService.GetResource("ShoppingCart.Rental.StartDateShouldBeFuture"));
            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="addRequiredProducts">Whether to add required products</param>
        /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
        /// <param name="getStandardWarnings">A value indicating whether we should validate a product for standard properties</param>
        /// <param name="getAttributesWarnings">A value indicating whether we should validate product attributes</param>
        /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
        /// <param name="getRequiredProductWarnings">A value indicating whether we should validate required products (products which require other products to be added to the cart)</param>
        /// <param name="getRentalWarnings">A value indicating whether we should validate rental properties</param>
        /// <returns>Warnings</returns>
        public virtual ErrorListModel GetShoppingCartItemWarnings(Customer customer, ShoppingCartType shoppingCartType,
            Product product, int storeId,
            string attributesXml, decimal customerEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool addRequiredProducts = true, int shoppingCartItemId = 0,
            bool getStandardWarnings = true, bool getAttributesWarnings = true,
            bool getGiftCardWarnings = true, bool getRequiredProductWarnings = true,
            bool getRentalWarnings = true, int count=1)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            List<string> warnings = new List<string>();
            ErrorListModel errorList = new ErrorListModel();

            //standard properties
            if (getStandardWarnings)
            {
                warnings.AddRange(GetStandardWarnings(customer, shoppingCartType, product, attributesXml, customerEnteredPrice, quantity, count));
            }

            //selected attributes
            if (getAttributesWarnings)
            {
                IList<string> errors = GetShoppingCartItemAttributeWarnings(customer, shoppingCartType, product, quantity, attributesXml);
                errorList.IsAttributeError = errors.Any() ? true : false;
                //Added by Alexandar Rajavel on 14-Jan-2019
                //var select = _localizationService.GetResource("Please.Select");
                //if (_workContext.WorkingLanguage.Id == 1)
                //{
                //    errors = errors.Any() && !errors.Where(s => s.Contains(select)).Any() ? errors.Select(s => select + " " + s).ToList() : errors;
                //}
                //else
                //{
                //    errors = errors.Any() && !errors.Where(s => s.Contains(select)).Any() ? errors.Select(s => s + " " + select).ToList() : errors;
                //}
                warnings.AddRange(errors);
            }

            //gift cards
            if (getGiftCardWarnings)
            {
                warnings.AddRange(GetShoppingCartItemGiftCardWarnings(shoppingCartType, product, attributesXml));
            }

            //required products
            if (getRequiredProductWarnings)
            {
                warnings.AddRange(GetRequiredProductWarnings(customer, shoppingCartType, product, storeId, quantity, addRequiredProducts, shoppingCartItemId));
            }

            //rental products
            if (getRentalWarnings)
            {
                warnings.AddRange(GetRentalProductWarnings(product, rentalStartDate, rentalEndDate));
            }

            errorList.ErrorList = warnings;
            return errorList;
        }

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="checkoutAttributesXml">Checkout attributes in XML format</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetShoppingCartWarnings(IList<ShoppingCartItem> shoppingCart,
            string checkoutAttributesXml, bool validateCheckoutAttributes)
        {
            List<string> warnings = new List<string>();

            bool hasStandartProducts = false;
            bool hasRecurringProducts = false;

            foreach (ShoppingCartItem sci in shoppingCart)
            {
                Product product = sci.Product;
                if (product == null)
                {
                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.CannotLoadProduct"), sci.ProductId));
                    return warnings;
                }

                if (product.IsRecurring)
                {
                    hasRecurringProducts = true;
                }
                else
                {
                    hasStandartProducts = true;
                }
            }

            //don't mix standard and recurring products
            if (hasStandartProducts && hasRecurringProducts)
            {
                warnings.Add(_localizationService.GetResource("ShoppingCart.CannotMixStandardAndAutoshipProducts"));
            }

            //recurring cart validation
            if (hasRecurringProducts)
            {
                string cyclesError = GetRecurringCycleInfo(shoppingCart, out int _, out RecurringProductCyclePeriod _, out int _);
                if (!string.IsNullOrEmpty(cyclesError))
                {
                    warnings.Add(cyclesError);
                    return warnings;
                }
            }

            //validate checkout attributes
            if (!validateCheckoutAttributes)
            {
                return warnings;
            }

            //selected attributes
            IList<CheckoutAttribute> attributes1 = _checkoutAttributeParser.ParseCheckoutAttributes(checkoutAttributesXml);

            //existing checkout attributes
            bool excludeShippableAttributes = !ShoppingCartRequiresShipping(shoppingCart);
            IList<CheckoutAttribute> attributes2 = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, excludeShippableAttributes);

            //validate conditional attributes only (if specified)
            attributes2 = attributes2.Where(x =>
            {
                bool? conditionMet = _checkoutAttributeParser.IsConditionMet(x, checkoutAttributesXml);
                return !conditionMet.HasValue || conditionMet.Value;
            }).ToList();

            foreach (CheckoutAttribute a2 in attributes2)
            {
                if (!a2.IsRequired)
                {
                    continue;
                }

                bool found = false;
                //selected checkout attributes
                foreach (CheckoutAttribute a1 in attributes1)
                {
                    if (a1.Id != a2.Id)
                    {
                        continue;
                    }

                    IList<string> attributeValuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, a1.Id);
                    foreach (string str1 in attributeValuesStr)
                    {
                        if (!string.IsNullOrEmpty(str1.Trim()))
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                {
                    continue;
                }

                //if not found
                warnings.Add(!string.IsNullOrEmpty(_localizationService.GetLocalized(a2, a => a.TextPrompt))
                    ? _localizationService.GetLocalized(a2, a => a.TextPrompt)
                    : string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"),
                        _localizationService.GetLocalized(a2, a => a.Name)));
            }

            //now validation rules

            //minimum length
            foreach (CheckoutAttribute ca in attributes2)
            {
                string enteredText;
                int enteredTextLength;

                if (ca.ValidationMinLength.HasValue)
                {
                    if (ca.AttributeControlType == AttributeControlType.TextBox ||
                        ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        enteredText = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id).FirstOrDefault();
                        enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                        if (ca.ValidationMinLength.Value > enteredTextLength)
                        {
                            warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.TextboxMinimumLength"), _localizationService.GetLocalized(ca, a => a.Name), ca.ValidationMinLength.Value));
                        }
                    }
                }

                //maximum length
                if (!ca.ValidationMaxLength.HasValue)
                {
                    continue;
                }

                if (ca.AttributeControlType != AttributeControlType.TextBox && ca.AttributeControlType != AttributeControlType.MultilineTextbox)
                {
                    continue;
                }

                enteredText = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id).FirstOrDefault();
                enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                if (ca.ValidationMaxLength.Value < enteredTextLength)
                {
                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.TextboxMaximumLength"), _localizationService.GetLocalized(ca, a => a.Name), ca.ValidationMaxLength.Value));
                }
            }

            return warnings;
        }

        /// <summary>
        /// Finds a shopping cart item in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">Price entered by a customer</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>Found shopping cart item</returns>
        public virtual ShoppingCartItem FindShoppingCartItemInTheCart(IList<ShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
            Product product,
            string attributesXml = "",
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null,
            DateTime? rentalEndDate = null)
        {
            if (shoppingCart == null)
            {
                throw new ArgumentNullException(nameof(shoppingCart));
            }

            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            foreach (ShoppingCartItem sci in shoppingCart.Where(a => a.ShoppingCartType == shoppingCartType))
            {
                if (sci.ProductId != product.Id)
                {
                    continue;
                }

                //attributes
                bool attributesEqual = _productAttributeParser.AreProductAttributesEqual(sci.AttributesXml, attributesXml, false, false);

                //gift cards
                bool giftCardInfoSame = true;
                if (sci.Product.IsGiftCard)
                {
                    _productAttributeParser.GetGiftCardAttribute(attributesXml, out string giftCardRecipientName1, out string _, out string giftCardSenderName1, out string _, out string _);

                    _productAttributeParser.GetGiftCardAttribute(sci.AttributesXml, out string giftCardRecipientName2, out string _, out string giftCardSenderName2, out string _, out string _);

                    if (giftCardRecipientName1.ToLowerInvariant() != giftCardRecipientName2.ToLowerInvariant() ||
                        giftCardSenderName1.ToLowerInvariant() != giftCardSenderName2.ToLowerInvariant())
                    {
                        giftCardInfoSame = false;
                    }
                }

                //price is the same (for products which require customers to enter a price)
                bool customerEnteredPricesEqual = true;
                if (sci.Product.CustomerEntersPrice)
                {
                    //TODO should we use PriceCalculationService.RoundPrice here?
                    customerEnteredPricesEqual = Math.Round(sci.CustomerEnteredPrice, 2) == Math.Round(customerEnteredPrice, 2);
                }

                //rental products
                bool rentalInfoEqual = true;
                if (sci.Product.IsRental)
                {
                    rentalInfoEqual = sci.RentalStartDateUtc == rentalStartDate && sci.RentalEndDateUtc == rentalEndDate;
                }

                //found?
                if (attributesEqual && giftCardInfoSame && customerEnteredPricesEqual && rentalInfoEqual)
                {
                    return sci;
                }
            }

            return null;
        }

        /// <summary>
        /// Add a product to shopping cart
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="product">Product</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">The price enter by a customer</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="addRequiredProducts">Whether to add required products</param>
        /// <returns>Warnings</returns>
        public virtual ErrorListModel AddToCart(Customer customer, Product product,
            ShoppingCartType shoppingCartType, int storeId, string attributesXml = null,
            decimal customerEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool addRequiredProducts = true, int count=1)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            ErrorListModel warnings = new ErrorListModel();
            if (shoppingCartType == ShoppingCartType.ShoppingCart && !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart, customer))
            {
                warnings.ErrorList.Add("Shopping cart is disabled");
                return warnings;
            }

            if (shoppingCartType == ShoppingCartType.Wishlist && !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist, customer))
            {
                warnings.ErrorList.Add("Wishlist is disabled");
                return warnings;
            }

            if (customer.IsSearchEngineAccount())
            {
                warnings.ErrorList.Add("Search engine can't add to cart");
                return warnings;
            }
            if (quantity >= 100)
            {
                warnings.ErrorList.Add("Quantity should be below 100");
                return warnings;
            }

            if (quantity <= 0)
            {
                warnings.ErrorList.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));
                return warnings;
            }

            //reset checkout info
            _customerService.ResetCheckoutData(customer, storeId);

            List<ShoppingCartItem> cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == shoppingCartType)
                .LimitPerStore(storeId)
                .ToList();

            ShoppingCartItem shoppingCartItem = FindShoppingCartItemInTheCart(cart,
                shoppingCartType, product, attributesXml, customerEnteredPrice,
                rentalStartDate, rentalEndDate);

            if (shoppingCartItem != null)
            {
                //update existing shopping cart item
                int newQuantity = shoppingCartItem.Quantity + quantity;
                count = (newQuantity * count);
                if (newQuantity >= 100)
                {
                    warnings.ErrorList.Add("Product Quantity should be below 100");
                    return warnings;
                }

                warnings = GetShoppingCartItemWarnings(customer, shoppingCartType, product,
                    storeId, attributesXml,
                    customerEnteredPrice, rentalStartDate, rentalEndDate,
                    newQuantity, addRequiredProducts, shoppingCartItem.Id, true, true, true, true, true, count);
                //warnings.AddRange(errors.ErrorList);

                if (warnings.ErrorList.Any())
                {
                    return warnings;
                }

                shoppingCartItem.AttributesXml = attributesXml;
                shoppingCartItem.Quantity = newQuantity;
                shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;
                _customerService.UpdateCustomer(customer);

                //event notification
                _eventPublisher.EntityUpdated(shoppingCartItem);
            }
            else
            {
                count = quantity * count;
                if (shoppingCartType != ShoppingCartType.Wishlist)
                {
                    //new shopping cart item
                    warnings = GetShoppingCartItemWarnings(customer, shoppingCartType, product,
                        storeId, attributesXml, customerEnteredPrice,
                        rentalStartDate, rentalEndDate,
                        quantity, addRequiredProducts, 0, true, true, true, true, true, count);
                    //warnings.AddRange(errors.ErrorList);

                    if (warnings.ErrorList.Any())
                    {
                        return warnings;
                    }
                }
                //maximum items validation
                switch (shoppingCartType)
                {
                    case ShoppingCartType.ShoppingCart:
                        if (cart.Count >= _shoppingCartSettings.MaximumShoppingCartItems)
                        {
                            warnings.ErrorList.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumShoppingCartItems"), _shoppingCartSettings.MaximumShoppingCartItems));
                            return warnings;
                        }

                        break;
                    case ShoppingCartType.Wishlist:
                        if (cart.Count >= _shoppingCartSettings.MaximumWishlistItems)
                        {
                            warnings.ErrorList.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumWishlistItems"), _shoppingCartSettings.MaximumWishlistItems));
                            return warnings;
                        }

                        break;
                    default:
                        break;
                }

                DateTime now = DateTime.UtcNow;
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartType = shoppingCartType,
                    StoreId = storeId,
                    Product = product,
                    AttributesXml = attributesXml,
                    CustomerEnteredPrice = customerEnteredPrice,
                    Quantity = quantity,
                    RentalStartDateUtc = rentalStartDate,
                    RentalEndDateUtc = rentalEndDate,
                    CreatedOnUtc = now,
                    UpdatedOnUtc = now
                };
                customer.ShoppingCartItems.Add(shoppingCartItem);
                _customerService.UpdateCustomer(customer);

                //updated "HasShoppingCartItems" property used for performance optimization
                customer.HasShoppingCartItems = customer.ShoppingCartItems.Any();
                _customerService.UpdateCustomer(customer);

                //event notification
                _eventPublisher.EntityInserted(shoppingCartItem);
            }

            return warnings;
        }

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customerEnteredPrice">New customer entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> UpdateShoppingCartItem(Customer customer,
            int shoppingCartItemId, string attributesXml,
            decimal customerEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool resetCheckoutData = true, int count=1)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            List<string> warnings = new List<string>();

            ShoppingCartItem shoppingCartItem = customer.ShoppingCartItems.FirstOrDefault(sci => sci.Id == shoppingCartItemId);
            if (shoppingCartItem == null)
            {
                return warnings;
            }

            if (resetCheckoutData)
            {
                //reset checkout data
                _customerService.ResetCheckoutData(customer, shoppingCartItem.StoreId);
            }

            if (quantity > 0)
            {

                count = quantity * count;
                //check warnings
                ErrorListModel errors = GetShoppingCartItemWarnings(customer, shoppingCartItem.ShoppingCartType,
                    shoppingCartItem.Product, shoppingCartItem.StoreId,
                    attributesXml, customerEnteredPrice,
                    rentalStartDate, rentalEndDate, quantity, false, shoppingCartItemId, true, true, true, true, true, count);
                warnings.AddRange(errors.ErrorList);

                if (warnings.Any())
                {
                    return warnings;
                }

                //if everything is OK, then update a shopping cart item
                shoppingCartItem.Quantity = quantity;
                shoppingCartItem.AttributesXml = attributesXml;
                shoppingCartItem.CustomerEnteredPrice = customerEnteredPrice;
                shoppingCartItem.RentalStartDateUtc = rentalStartDate;
                shoppingCartItem.RentalEndDateUtc = rentalEndDate;
                shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;
                _customerService.UpdateCustomer(customer);

                //event notification
                _eventPublisher.EntityUpdated(shoppingCartItem);
            }
            else
            {
                //check warnings for required products
                warnings.AddRange(GetRequiredProductWarnings(customer, shoppingCartItem.ShoppingCartType,
                    shoppingCartItem.Product, shoppingCartItem.StoreId, quantity, false, shoppingCartItemId));
                if (warnings.Any())
                {
                    return warnings;
                }

                //delete a shopping cart item
                DeleteShoppingCartItem(shoppingCartItem, resetCheckoutData, true);
            }

            return warnings;
        }

        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromCustomer">From customer</param>
        /// <param name="toCustomer">To customer</param>
        /// <param name="includeCouponCodes">A value indicating whether to coupon codes (discount and gift card) should be also re-applied</param>
        public virtual void MigrateShoppingCart(Customer fromCustomer, Customer toCustomer, bool includeCouponCodes)
        {
            if (fromCustomer == null)
            {
                throw new ArgumentNullException(nameof(fromCustomer));
            }

            if (toCustomer == null)
            {
                throw new ArgumentNullException(nameof(toCustomer));
            }

            if (fromCustomer.Id == toCustomer.Id)
            {
                return; //the same customer
            }

            //shopping cart items
            List<ShoppingCartItem> fromCart = fromCustomer.ShoppingCartItems.ToList();
            for (int i = 0; i < fromCart.Count; i++)
            {
                ShoppingCartItem sci = fromCart[i];
                AddToCart(toCustomer, sci.Product, sci.ShoppingCartType, sci.StoreId,
                    sci.AttributesXml, sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, false, 1);
            }

            for (int i = 0; i < fromCart.Count; i++)
            {
                ShoppingCartItem sci = fromCart[i];
                DeleteShoppingCartItem(sci);
            }

            //copy discount and gift card coupon codes
            if (includeCouponCodes)
            {
                //discount
                foreach (string code in _customerService.ParseAppliedDiscountCouponCodes(fromCustomer))
                {
                    _customerService.ApplyDiscountCouponCode(toCustomer, code);
                }

                //gift card
                foreach (string code in _customerService.ParseAppliedGiftCardCouponCodes(fromCustomer))
                {
                    _customerService.ApplyGiftCardCouponCode(toCustomer, code);
                }

                //save customer
                _customerService.UpdateCustomer(toCustomer);
            }

            //move selected checkout attributes
            string checkoutAttributesXml = _genericAttributeService.GetAttribute<string>(fromCustomer, NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);
            _genericAttributeService.SaveAttribute(toCustomer, NopCustomerDefaults.CheckoutAttributes, checkoutAttributesXml, _storeContext.CurrentStore.Id);
        }

        /// <summary>
        /// Migrate shopping cart,  Added by Alexandar Rajavel on 01-Mar-2019
        /// </summary>
        /// <param name="fromCustomer">From customer</param>
        /// <param name="toCustomer">To customer</param>
        public virtual void MigrateShoppingCartNew(Customer fromCustomer, Customer toCustomer)
        {
            //shopping cart items
            DateTime now = DateTime.UtcNow;
            List<ShoppingCartItem> shoppingCartItems = fromCustomer.ShoppingCartItems.ToList();
            foreach (ShoppingCartItem sci in shoppingCartItems)
            {
                ShoppingCartItem shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartType = sci.ShoppingCartType,
                    StoreId = sci.StoreId,
                    Product = sci.Product,
                    AttributesXml = sci.AttributesXml,
                    CustomerEnteredPrice = sci.CustomerEnteredPrice,
                    Quantity = sci.Quantity,
                    RentalStartDateUtc = sci.RentalStartDateUtc,
                    RentalEndDateUtc = sci.RentalEndDateUtc,
                    CreatedOnUtc = now,
                    UpdatedOnUtc = now
                };
                toCustomer.ShoppingCartItems.Add(shoppingCartItem);
                _customerService.UpdateCustomer(toCustomer);

                //updated "HasShoppingCartItems" property used for performance optimization
                toCustomer.HasShoppingCartItems = toCustomer.ShoppingCartItems.Any();
                _customerService.UpdateCustomer(toCustomer);

                //event notification
                _eventPublisher.EntityInserted(shoppingCartItem);
            }

            for (int i = 0; i < shoppingCartItems.Count; i++)
            {
                ShoppingCartItem sci = shoppingCartItems[i];
                DeleteShoppingCartItem(sci);
            }
        }

        /// <summary>
        /// Indicates whether the shopping cart requires shipping
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>True if the shopping cart requires shipping; otherwise, false.</returns>
        public virtual bool ShoppingCartRequiresShipping(IList<ShoppingCartItem> shoppingCart)
        {
            return shoppingCart.Any(shoppingCartItem => _shippingService.IsShipEnabled(shoppingCartItem));
        }

        /// <summary>
        /// Gets a value indicating whether shopping cart is recurring
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>Result</returns>
        public virtual bool ShoppingCartIsRecurring(IList<ShoppingCartItem> shoppingCart)
        {
            return shoppingCart.Any(item => item.Product?.IsRecurring ?? false);
        }

        /// <summary>
        /// Get a recurring cycle information
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="cycleLength">Cycle length</param>
        /// <param name="cyclePeriod">Cycle period</param>
        /// <param name="totalCycles">Total cycles</param>
        /// <returns>Error (if exists); otherwise, empty string</returns>
        public virtual string GetRecurringCycleInfo(IList<ShoppingCartItem> shoppingCart,
            out int cycleLength, out RecurringProductCyclePeriod cyclePeriod, out int totalCycles)
        {
            cycleLength = 0;
            cyclePeriod = 0;
            totalCycles = 0;

            int? _cycleLength = null;
            RecurringProductCyclePeriod? _cyclePeriod = null;
            int? _totalCycles = null;

            foreach (ShoppingCartItem sci in shoppingCart)
            {
                Product product = sci.Product;
                if (product == null)
                {
                    throw new NopException($"Product (Id={sci.ProductId}) cannot be loaded");
                }

                if (!product.IsRecurring)
                {
                    continue;
                }

                string conflictError = _localizationService.GetResource("ShoppingCart.ConflictingShipmentSchedules");

                //cycle length
                if (_cycleLength.HasValue && _cycleLength.Value != product.RecurringCycleLength)
                {
                    return conflictError;
                }

                _cycleLength = product.RecurringCycleLength;

                //cycle period
                if (_cyclePeriod.HasValue && _cyclePeriod.Value != product.RecurringCyclePeriod)
                {
                    return conflictError;
                }

                _cyclePeriod = product.RecurringCyclePeriod;

                //total cycles
                if (_totalCycles.HasValue && _totalCycles.Value != product.RecurringTotalCycles)
                {
                    return conflictError;
                }

                _totalCycles = product.RecurringTotalCycles;
            }

            if (!_cycleLength.HasValue)
            {
                return string.Empty;
            }

            cycleLength = _cycleLength.Value;
            cyclePeriod = _cyclePeriod.Value;
            totalCycles = _totalCycles.Value;

            return string.Empty;
        }

        #endregion
    }
}
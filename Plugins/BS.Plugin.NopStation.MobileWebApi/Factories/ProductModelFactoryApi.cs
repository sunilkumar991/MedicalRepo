using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework.Security.Captcha;
using BS.Plugin.NopStation.MobileWebApi.Models.Product;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Product;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using BS.Plugin.NopStation.MobileWebApi.Models.Vendor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.Seo;
using Nop.Services.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Discounts;
using static BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel.ProductOverviewModel;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    /// <summary>
    /// Represents the product model factory
    /// </summary>
    public partial class ProductModelFactoryApi : IProductModelFactoryApi
    {
        #region Fields

        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWebHelper _webHelper;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IProductTagService _productTagService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDateRangeService _dateRangeService;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly SeoSettings _seoSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;

        #endregion

        #region Constructors

        public ProductModelFactoryApi(ISpecificationAttributeService specificationAttributeService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IVendorService vendorService,
            IProductTemplateService productTemplateService,
            IProductAttributeService productAttributeService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IWebHelper webHelper,
            IDateTimeHelper dateTimeHelper,
            IProductTagService productTagService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            IDownloadService downloadService,
            IProductAttributeParser productAttributeParser,
            IDateRangeService dateRangeService,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings,
            CustomerSettings customerSettings,
            CaptchaSettings captchaSettings,
            SeoSettings seoSettings,
            ICacheManager cacheManager,
            IUrlRecordService urlRecordService,
            ICustomerService customerService,
            IDiscountService discountService)
        {
            this._specificationAttributeService = specificationAttributeService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._vendorService = vendorService;
            this._productTemplateService = productTemplateService;
            this._productAttributeService = productAttributeService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._measureService = measureService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._webHelper = webHelper;
            this._dateTimeHelper = dateTimeHelper;
            this._productTagService = productTagService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;
            this._productAttributeParser = productAttributeParser;
            this._dateRangeService = dateRangeService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._vendorSettings = vendorSettings;
            this._customerSettings = customerSettings;
            this._captchaSettings = captchaSettings;
            this._seoSettings = seoSettings;
            this._cacheManager = cacheManager;
            _urlRecordService = urlRecordService;
            _customerService = customerService;
            this._discountService = discountService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the product review overview model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Product review overview model</returns>
        protected virtual ProductOverViewModelApi.ProductReviewOverviewModel PrepareProductReviewOverviewModel(Product product)
        {
            ProductOverViewModelApi.ProductReviewOverviewModel productReview = new ProductOverViewModelApi.ProductReviewOverviewModel
            {
                ProductId = product.Id,
                //RatingSum = product.ApprovedRatingSum,
                TotalReviews = product.ApprovedTotalReviews,
                AllowCustomerReviews = product.AllowCustomerReviews,
                //Added by Sunil Kumar 18-2-19
                RatingUrlFromGSMArena = product.RatingUrlFromGSMArena
            };
            if (product.ApprovedTotalReviews != 0)
            {
                double RatingSumval = (double)product.ApprovedRatingSum / product.ApprovedTotalReviews;
                productReview.RatingSum = Math.Round(RatingSumval, 1);
            }
            else
            {
                productReview.RatingSum = product.ApprovedRatingSum;

            }
            return productReview;
        }

        //added code by Sunil Kumar at 05-03-19
        /// <summary>
        /// Prepare Product Attributes model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="singleProductId">ProductId</param>
        /// <returns>Product review overview model</returns>
        /// //List<Product> products
        //protected virtual IList<ProductOverViewModelApi.ProductAttributesModel> PrepareProductAttributesModel(IProductAttributeService productAttributeService, int singleProductId)
        //{
        //    var ProductAttribute = new List<ProductOverViewModelApi.ProductAttributesModel>();
        //    //------start Added by Alexandar Rajavel on 10-Nov-2018
        //    var productAttributeMappings = new List<ProductAttributeMapping>();
        //    var mappings = productAttributeService.GetProductAttributeMappingsByProductId(singleProductId);
        //    productAttributeMappings.AddRange(mappings);
        //    var filterByProductAttribute = new List<ProductOverViewModelApi.ProductAttributesModel>();
        //    foreach (var productAttributes in productAttributeMappings)
        //    {
        //        var attributeName = productAttributes.ProductAttribute.Name;
        //        var productId = productAttributes.ProductId;
        //        var productAttributeValues = productAttributes.ProductAttributeValues.ToList();
        //        productAttributeValues.ForEach(x => filterByProductAttribute.Add(new ProductOverViewModelApi.ProductAttributesModel()
        //        {
        //            SpecificationAttributeName = attributeName,
        //            SpecificationAttributeOptionName = x.Name,
        //            FilterId = x.Id,
        //            ProductId = productId
        //        }));
        //    }
        //    ProductAttribute = filterByProductAttribute.OrderBy(saof => saof.SpecificationAttributeName)
        //        .ThenBy(saof => saof.SpecificationAttributeName)
        //        .ThenBy(saof => saof.SpecificationAttributeOptionName).ToList();

        //    return ProductAttribute;
        //    //------End----

        //}

        /// <summary>
        /// Prepare the product overview price model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>Product overview price model</returns>
        protected virtual ProductPriceModel PrepareProductOverviewPriceModel(Product product, bool forceRedirectionAfterAddingToCart = false)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var priceModel = new ProductOverviewModel.ProductPriceModel();

            switch (product.ProductType)
            {
                case ProductType.GroupedProduct:
                    {
                        #region Grouped product

                        var associatedProducts = _productService.GetAssociatedProducts(product.Id, _storeContext.CurrentStore.Id);

                        switch (associatedProducts.Count)
                        {
                            case 0:
                                {

                                }
                                break;
                            default:
                                {


                                    if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                                    {
                                        //find a minimum possible price
                                        decimal? minPossiblePrice = null;
                                        Product minPriceProduct = null;
                                        foreach (var associatedProduct in associatedProducts)
                                        {
                                            //calculate for the maximum quantity (in case if we have tier prices)
                                            var tmpPrice = _priceCalculationService.GetFinalPrice(associatedProduct,
                                                _workContext.CurrentCustomer, decimal.Zero, true, int.MaxValue);
                                            if (!minPossiblePrice.HasValue || tmpPrice < minPossiblePrice.Value)
                                            {
                                                minPriceProduct = associatedProduct;
                                                minPossiblePrice = tmpPrice;
                                            }
                                        }
                                        if (minPriceProduct != null && !minPriceProduct.CustomerEntersPrice)
                                        {
                                            if (minPriceProduct.CallForPrice)
                                            {
                                                priceModel.OldPrice = null;
                                                priceModel.Price = _localizationService.GetResource("Products.CallForPrice");
                                            }
                                            else if (minPossiblePrice.HasValue)
                                            {
                                                //calculate prices
                                                decimal taxRate;
                                                decimal finalPriceBase = _taxService.GetProductPrice(minPriceProduct, minPossiblePrice.Value, out taxRate);
                                                decimal finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, _workContext.WorkingCurrency);

                                                priceModel.OldPrice = null;
                                                priceModel.Price = String.Format(_localizationService.GetResource("Products.PriceRangeFrom"), _priceFormatter.FormatPrice(finalPrice));

                                            }
                                            else
                                            {
                                                //Actually it's not possible (we presume that minimalPrice always has a value)
                                                //We never should get here
                                                Debug.WriteLine("Cannot calculate minPrice for product #{0}", product.Id);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //hide prices
                                        priceModel.OldPrice = null;
                                        priceModel.Price = null;
                                    }
                                }
                                break;
                        }

                        #endregion
                    }
                    break;
                case ProductType.SimpleProduct:
                default:
                    {
                        #region Simple product


                        //prices
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            if (!product.CustomerEntersPrice)
                            {
                                if (product.CallForPrice)
                                {
                                    //call for price
                                    priceModel.OldPrice = null;
                                    priceModel.Price = _localizationService.GetResource("Products.CallForPrice");
                                }
                                else
                                {
                                    //prices
                                    var minPossiblePrice = _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer);

                                    if (product.HasTierPrices)
                                    {
                                        //calculate price for the maximum quantity if we have tier prices, and choose minimal
                                        minPossiblePrice = Math.Min(minPossiblePrice,
                                            _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, quantity: int.MaxValue));
                                    }

                                    decimal taxRate;
                                    decimal oldPriceBase = _taxService.GetProductPrice(product, product.OldPrice, out taxRate);
                                    decimal finalPriceBase = _taxService.GetProductPrice(product, minPossiblePrice, out taxRate);

                                    decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                                    decimal finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, _workContext.WorkingCurrency);

                                    //do we have tier prices configured?
                                    var tierPrices = new List<TierPrice>();
                                    if (product.HasTierPrices)
                                    {
                                        tierPrices.AddRange(product.TierPrices
                                            .OrderBy(tp => tp.Quantity)
                                            .ToList()
                                            .FilterByStore(_storeContext.CurrentStore.Id)
                                            .FilterForCustomer(_workContext.CurrentCustomer)
                                            .RemoveDuplicatedQuantities());
                                    }
                                    //When there is just one tier (with  qty 1), 
                                    //there are no actual savings in the list.
                                    bool displayFromMessage = tierPrices.Count > 0 &&
                                        !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);
                                    if (displayFromMessage)
                                    {
                                        priceModel.OldPrice = null;
                                        priceModel.Price = String.Format(_localizationService.GetResource("Products.PriceRangeFrom"), _priceFormatter.FormatPrice(finalPrice));
                                    }
                                    else
                                    {
                                        if (finalPriceBase != oldPriceBase && oldPriceBase != decimal.Zero)
                                        {
                                            priceModel.OldPrice = _priceFormatter.FormatPrice(oldPrice);
                                            priceModel.Price = _priceFormatter.FormatPrice(finalPrice);
                                        }
                                        else
                                        {
                                            priceModel.OldPrice = null;
                                            priceModel.Price = _priceFormatter.FormatPrice(finalPrice);
                                        }
                                    }
                                    if (product.IsRental)
                                    {
                                        //rental product
                                        priceModel.OldPrice = _priceFormatter.FormatRentalProductPeriod(product, priceModel.OldPrice);
                                        priceModel.Price = _priceFormatter.FormatRentalProductPeriod(product, priceModel.Price);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //hide prices
                            priceModel.OldPrice = null;
                            priceModel.Price = null;
                        }

                        #endregion
                    }
                    break;
            }

            return priceModel;
        }

        /// <summary>
        /// Prepare the product overview picture model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productThumbPictureSize">Product thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <returns>Picture model</returns>
        protected virtual PictureModel PrepareProductOverviewPictureModel(Product product, int? productThumbPictureSize = null)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //If a size has been set in the view, we use it in priority
            int pictureSize = productThumbPictureSize.HasValue
                ? productThumbPictureSize.Value
                : _mediaSettings.ProductThumbPictureSize;

            //prepare picture model
            var cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY,
                product.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(),
                _storeContext.CurrentStore.Id);

            PictureModel defaultPictureModel = _cacheManager.Get(cacheKey, () =>
            {
                var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                var pictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize)
                };

                return pictureModel;
            });

            return defaultPictureModel;
        }

        /// <summary>
        /// Prepare the product breadcrumb model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Product breadcrumb model</returns>
        protected virtual ProductDetailsModelApi.ProductBreadcrumbModel PrepareProductBreadcrumbModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_BREADCRUMB_MODEL_KEY,
                    product.Id,
                    _workContext.WorkingLanguage.Id,
                    string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                    _storeContext.CurrentStore.Id);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var breadcrumbModel = new ProductDetailsModelApi.ProductBreadcrumbModel
                {
                    Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                    ProductId = product.Id,
                    ProductName = _localizationService.GetLocalized(product, p => p.Name),
                    ProductSeName = _urlRecordService.GetSeName(product),
                };
                var productCategories = _categoryService.GetProductCategoriesByProductId(product.Id);
                if (productCategories.Any())
                {
                    var category = productCategories[0].Category;
                    if (category != null)
                    {
                        foreach (var catBr in _categoryService.GetCategoryBreadCrumb(category))
                        {
                            breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                            {
                                Id = catBr.Id,
                                Name = _localizationService.GetLocalized(catBr, x => x.Name),
                                IncludeInTopMenu = catBr.IncludeInTopMenu
                            });
                        }
                    }
                }
                return breadcrumbModel;
            });
            return cachedModel;
        }

        /// <summary>
        /// Prepare the product tag models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of product tag model</returns>
        protected virtual IList<ProductTagModel> PrepareProductTagModels(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var productTagsCacheKey = string.Format(ModelCacheEventConsumer.PRODUCTTAG_BY_PRODUCT_MODEL_KEY, product.Id, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id);
            var model = _cacheManager.Get(productTagsCacheKey, () =>
                product.ProductProductTagMappings
                //filter by store
                .Where(x => _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id) > 0)
                .Select(x => new ProductTagModel
                {
                    Id = x.Id,
                    Name = _localizationService.GetLocalized(x.ProductTag, y => y.Name),
                    SeName = _urlRecordService.GetSeName(product),
                    ProductCount = _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id)
                })
                .ToList());

            return model;
        }

        /// <summary>
        /// Prepare the product price model
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Product price model</returns>
        protected virtual ProductDetailsModelApi.ProductPriceModel PrepareProductPriceModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var model = new ProductDetailsModelApi.ProductPriceModel();

            model.ProductId = product.Id;
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.HidePrices = false;
                if (product.CustomerEntersPrice)
                {
                    model.CustomerEntersPrice = true;
                }
                else
                {
                    if (product.CallForPrice)
                    {
                        model.CallForPrice = true;
                    }
                    else
                    {
                        decimal taxRate;
                        decimal oldPriceBase = _taxService.GetProductPrice(product, product.OldPrice, out taxRate);
                        decimal finalPriceWithoutDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: false), out taxRate);
                        decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: true), out taxRate);

                        decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                        decimal finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _workContext.WorkingCurrency);
                        decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                            model.OldPrice = _priceFormatter.FormatPrice(oldPrice);

                        model.Price = _priceFormatter.FormatPrice(finalPriceWithoutDiscount);

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                            model.PriceWithDiscount = _priceFormatter.FormatPrice(finalPriceWithDiscount);

                        model.PriceValue = finalPriceWithDiscount;
                        model.PriceWithDiscountValue = finalPriceWithDiscount;

                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this product
                        //we also ensure this it's not free shipping
                        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductDetailsPage
                            && product.IsShipEnabled &&
                            !product.IsFreeShipping;

                        //PAngV baseprice (used in Germany)
                        model.BasePricePAngV = _priceFormatter.FormatBasePrice(product, finalPriceWithDiscountBase);

                        //currency code
                        model.CurrencyCode = _workContext.WorkingCurrency.CurrencyCode;

                        //rental
                        if (product.IsRental)
                        {
                            model.IsRental = true;
                            var priceStr = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                            model.RentalPrice = _priceFormatter.FormatRentalProductPeriod(product, priceStr);
                        }
                    }
                }
            }
            else
            {
                model.HidePrices = true;
                model.OldPrice = null;
                model.Price = null;
            }

            return model;
        }

        /// <summary>
        /// Prepare the product add to cart model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <returns>Product add to cart model</returns>
        protected virtual ProductDetailsModelApi.AddToCartModel PrepareProductAddToCartModel(Product product, ShoppingCartItem updatecartitem)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var model = new ProductDetailsModelApi.AddToCartModel();


            model.ProductId = product.Id;
            if (updatecartitem != null)
            {
                model.UpdatedShoppingCartItemId = updatecartitem.Id;
                //model.UpdateShoppingCartItemType = updatecartitem.ShoppingCartType;
            }

            //quantity
            model.EnteredQuantity = updatecartitem != null ? updatecartitem.Quantity : product.OrderMinimumQuantity;

            //'add to cart', 'add to wishlist' buttons
            model.DisableBuyButton = product.DisableBuyButton || !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart);
            model.DisableWishlistButton = product.DisableWishlistButton || !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist);
            if (!_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.DisableBuyButton = true;
                model.DisableWishlistButton = true;
            }
            //pre-order
            if (product.AvailableForPreOrder)
            {
                model.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                    product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                model.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
            }
            //rental
            model.IsRental = product.IsRental;

            //customer entered price
            model.CustomerEntersPrice = product.CustomerEntersPrice;
            if (model.CustomerEntersPrice)
            {
                decimal minimumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MinimumCustomerEnteredPrice, _workContext.WorkingCurrency);
                decimal maximumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MaximumCustomerEnteredPrice, _workContext.WorkingCurrency);

                model.CustomerEnteredPrice = updatecartitem != null ? updatecartitem.CustomerEnteredPrice : minimumCustomerEnteredPrice;
                model.CustomerEnteredPriceRange = string.Format(_localizationService.GetResource("Products.EnterProductPrice.Range"),
                    _priceFormatter.FormatPrice(minimumCustomerEnteredPrice, false, false),
                    _priceFormatter.FormatPrice(maximumCustomerEnteredPrice, false, false));
            }

            //allowed quantities
            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            foreach (var qty in allowedQuantities)
            {
                model.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = updatecartitem != null && updatecartitem.Quantity == qty
                });
            }

            //Added By Sunil Kumar at 12/12/18
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist && sci.ProductId == product.Id);
            if (cart.Any())
            {
                model.IsWishList = true;
            }

            return model;
        }

        /// <summary>
        /// Prepare the product attribute models
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <returns>List of product attribute model</returns>
        protected virtual IList<ProductDetailsModelApi.ProductAttributeModel> PrepareProductAttributeModels(Product product, ShoppingCartItem updatecartitem, bool isAssociatedProduct)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //performance optimization
            //We cache a value indicating whether a product has attributes
            IList<ProductAttributeMapping> productAttributeMapping = null;
            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_HAS_PRODUCT_ATTRIBUTES_KEY, product.Id);
            var hasProductAttributesCache = _cacheManager.Get(cacheKey, () =>
            {
                //no value in the cache yet
                //let's load attributes and cache the result (true/false)
                productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                return productAttributeMapping.Any();
            });
            //if (!hasProductAttributesCache.HasValue)
            //{
            //    //no value in the cache yet
            //    //let's load attributes and cache the result (true/false)
            //    productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            //    hasProductAttributesCache = productAttributeMapping.Any();
            //    _cacheManager.Set(cacheKey, hasProductAttributesCache, 60);
            //}
            if (hasProductAttributesCache && productAttributeMapping == null)
            {
                //cache indicates that the product has attributes
                //let's load them
                productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            }
            if (productAttributeMapping == null)
            {
                productAttributeMapping = new List<ProductAttributeMapping>();
            }

            var model = new List<ProductDetailsModelApi.ProductAttributeModel>();

            foreach (var attribute in productAttributeMapping)
            {
                var attributeModel = new ProductDetailsModelApi.ProductAttributeModel
                {
                    Id = attribute.Id,
                    ProductId = product.Id,
                    ProductAttributeId = attribute.ProductAttributeId,
                    Name = _localizationService.GetLocalized(attribute.ProductAttribute, x => x.Name),
                    Description = _localizationService.GetLocalized(attribute.ProductAttribute, x => x.Description),
                    //TextPrompt = attribute.TextPrompt,   // Commented by Alexandar Rajavel on 09-Feb-2019
                    TextPrompt = _localizationService.GetLocalized(attribute, x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = updatecartitem != null ? null : attribute.DefaultValue,
                };
                if (!String.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new ProductDetailsModelApi.ProductAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = _localizationService.GetLocalized(attributeValue, x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(valueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            decimal taxRate;
                            decimal attributeValuePriceAdjustment = _priceCalculationService.GetProductAttributeValuePriceAdjustment(attributeValue, updatecartitem?.Customer ?? _workContext.CurrentCustomer);
                            decimal priceAdjustmentBase = _taxService.GetProductPrice(product, attributeValuePriceAdjustment, out taxRate);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                //Commented by Sunil Kumar S at 13-03-2020
                                //valueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment, false, false);
                                valueModel.PriceAdjustment = _priceFormatter.FormatPrice(priceAdjustment, false, false);
                            else if (priceAdjustmentBase < decimal.Zero)
                                //valueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment, false, false);
                                valueModel.PriceAdjustment = _priceFormatter.FormatPrice(-priceAdjustment, false, false);

                            valueModel.PriceAdjustmentValue = priceAdjustment;
                        }

                        var defaultPictureSize = isAssociatedProduct ?
                            _mediaSettings.AssociatedProductPictureSize :
                            _mediaSettings.ProductDetailsPictureSize;
                        //picture
                        if (attributeValue.PictureId > 0)
                        {
                            var productAttributePictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCTATTRIBUTE_PICTURE_MODEL_KEY,
                                attributeValue.PictureId,
                                _webHelper.IsCurrentConnectionSecured(),
                                _storeContext.CurrentStore.Id);

                            valueModel.PictureModel = _cacheManager.Get(productAttributePictureCacheKey, () =>
                            {
                                var valuePicture = _pictureService.GetPictureById(attributeValue.PictureId);
                                if (valuePicture != null)
                                {
                                    return new PictureModel
                                    {
                                        //added by Sunil Kumar at 24-04-19
                                        Id = valuePicture.Id,
                                        ImageUrl = _pictureService.GetPictureUrl(valuePicture, defaultPictureSize)
                                    };
                                }
                                return new PictureModel();
                            });
                        }
                    }

                }

                //set already selected attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            {
                                if (!String.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = _productAttributeParser.ParseProductAttributeValues(updatecartitem.AttributesXml);
                                    foreach (var attributeValue in selectedValues)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                                item.IsPreSelected = true;
                                }
                            }
                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                            {
                                //do nothing
                                //values are already pre-set
                            }
                            break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                            {
                                if (!String.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var enteredText = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                    if (enteredText.Any())
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }
                            break;
                        case AttributeControlType.Datepicker:
                            {
                                //keep in mind my that the code below works only in the current culture
                                var selectedDateStr = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                if (selectedDateStr.Any())
                                {
                                    DateTime selectedDate;
                                    if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                           DateTimeStyles.None, out selectedDate))
                                    {
                                        //successfully parsed
                                        attributeModel.SelectedDay = selectedDate.Day;
                                        attributeModel.SelectedMonth = selectedDate.Month;
                                        attributeModel.SelectedYear = selectedDate.Year;
                                    }
                                }

                            }
                            break;
                        default:
                            break;
                    }
                }

                model.Add(attributeModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the product tier price models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of tier price model</returns>
        protected virtual IList<ProductDetailsModelApi.TierPriceModel> PrepareProductTierPriceModels(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var model = product.TierPrices.OrderBy(x => x.Quantity)
                   .FilterByStore(_storeContext.CurrentStore.Id)
                   .FilterForCustomer(_workContext.CurrentCustomer)
                   .FilterByDate()
                   .RemoveDuplicatedQuantities()
                   .Select(tierPrice =>
                   {
                       decimal taxRate;
                       var priceBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product,
                           _workContext.CurrentCustomer, decimal.Zero, _catalogSettings.DisplayTierPricesWithDiscounts, tierPrice.Quantity), out taxRate);
                       var price = _currencyService.ConvertFromPrimaryStoreCurrency(priceBase, _workContext.WorkingCurrency);

                       return new ProductDetailsModelApi.TierPriceModel
                       {
                           Quantity = tierPrice.Quantity,
                           Price = _priceFormatter.FormatPrice(price, false, false)
                       };
                   }).ToList();

            return model;
        }

        /// <summary>
        /// Prepare the product manufacturer models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of manufacturer brief info model</returns>
        protected virtual IList<MenuFacturerModelShortDetailApi> PrepareProductManufacturerModels(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            string manufacturersCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_MANUFACTURERS_MODEL_KEY,
                     product.Id,
                     _workContext.WorkingLanguage.Id,
                     string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                     _storeContext.CurrentStore.Id);
            var model = _cacheManager.Get(manufacturersCacheKey,
                () => _manufacturerService.GetProductManufacturersByProductId(product.Id)
                    .Select(pm =>
                    {
                        var manufacturer = pm.Manufacturer;
                        var modelMan = new MenuFacturerModelShortDetailApi
                        {
                            //Id = manufacturer.Id,
                            Name = _localizationService.GetLocalized(manufacturer, x => x.Name),
                        };
                        return modelMan;
                    })
                    .ToList()
                );

            return model;
        }

        /// <summary>
        /// Prepare the product details picture model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="isAssociatedProduct">Whether the product is associated</param>
        /// <returns>Picture model for the default picture and list of picture models for all product pictures</returns>
        protected virtual dynamic PrepareProductDetailsPictureModel(Product product, bool isAssociatedProduct = false)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //default picture size
            var defaultPictureSize = isAssociatedProduct ?
                _mediaSettings.AssociatedProductPictureSize :
                _mediaSettings.ProductDetailsPictureSize;

            //prepare picture models
            var productPicturesCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DETAILS_PICTURES_MODEL_KEY, product.Id, defaultPictureSize, isAssociatedProduct, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
            var cachedPictures = _cacheManager.Get(productPicturesCacheKey, () =>
            {
                var pictures = _pictureService.GetPicturesByProductId(product.Id);
                var defaultPicture = pictures.FirstOrDefault();
                var defaultPictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(defaultPicture, defaultPictureSize, !isAssociatedProduct),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(defaultPicture, 0, !isAssociatedProduct),
                    Title = _pictureService.GetPictureSeName(product.Name)
                };

                //all pictures
                var pictureModels = new List<PictureModel>();
                foreach (var picture in pictures)
                {
                    var pictureModel = new PictureModel
                    {
                        //added by Sunil Kumar At 24-04-19
                        Id = picture.Id,
                        ImageUrl = _pictureService.GetPictureUrl(picture, 400),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture, 800),
                        Title = _pictureService.GetPictureSeName(product.Name)
                    };

                    pictureModels.Add(pictureModel);
                }

                return new { DefaultPictureModel = defaultPictureModel, PictureModels = pictureModels };
            });

            return cachedPictures;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the product template view path
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>View path</returns>
        public virtual string PrepareProductTemplateViewPath(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var templateCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_TEMPLATE_MODEL_KEY, product.ProductTemplateId);
            var productTemplateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _productTemplateService.GetProductTemplateById(product.ProductTemplateId);
                if (template == null)
                    template = _productTemplateService.GetAllProductTemplates().FirstOrDefault();
                if (template == null)
                    throw new Exception("No default template could be loaded");
                return template.ViewPath;
            });

            return productTemplateViewPath;
        }

        /// <summary>
        /// Prepare the product overview models
        /// </summary>
        /// <param name="products">Collection of products</param>
        /// <param name="preparePriceModel">Whether to prepare the price model</param>
        /// <param name="preparePictureModel">Whether to prepare the picture model</param>
        /// <param name="productThumbPictureSize">Product thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <param name="prepareSpecificationAttributes">Whether to prepare the specification attribute models</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>Collection of product overview model</returns>
        public virtual IEnumerable<ProductOverViewModelApi> PrepareProductOverviewModels(IEnumerable<Product> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false, bool isHomePageCategoriesProduct = false)
        {
            if (products == null)
                throw new ArgumentNullException("products");

            var models = new List<ProductOverViewModelApi>();
            //Added Code By Sunil Kumar for only getting 4 Products In Homepagecategorieswithproduct at 13-02-2020
            if (isHomePageCategoriesProduct)
            {
                products = products.Take(4);
            }
            foreach (var product in products)
            {
                var model = new ProductOverViewModelApi
                {
                    Id = product.Id,
                    Name = _localizationService.GetLocalized(product, x => x.Name),
                    ShortDescription = _localizationService.GetLocalized(product, x => x.ShortDescription),
                    Sku = product.Sku,
                    //Added By Sunil Kumar at 26/12/18
                    CreatedOn = product.CreatedOnUtc,
                    DisplayOrder = product.DisplayOrder,
                    InStock = (product.StockQuantity>0?true:false)


                };
                //Added By Sunil Kumar at 21/12/18
                var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist && sci.ProductId == product.Id);
                if (cart.Any())
                {
                    model.IsWishList = true;
                }

                //Added By Sunil Kumar at 01/04/19
                DateTime now = DateTime.Now;
                if (product.MarkAsNew && product.MarkAsNewStartDateTimeUtc <= now && product.MarkAsNewEndDateTimeUtc >= now)
                {
                    model.MarkAsNew = true;
                }

                //Added By Sunil Kumar at 27/02/19
                model.DisableWishlistButton = product.DisableWishlistButton;
                //price
                if (preparePriceModel)
                {
                    model.ProductPrice = PrepareProductPriceModel(product);
                    //Discount Percentage
                    //Added By Sunil Kumar on 27-Dec-2018
                    model.ProductPrice.DiscountOfferName = string.Empty;
                    if (product.HasDiscountsApplied)
                    {
                        //Added by Sunil Kumar at 28-02-19
                        var discount = _discountService.GetDiscountPercentageAndName(product);
                        model.ProductPrice.DiscountPercentage = discount.Item1;
                        model.ProductPrice.DiscountOfferName = discount.Item2;
                    }
                    // model.ProductPrice = PrepareProductOverviewPriceModel(product, forceRedirectionAfterAddingToCart);
                }

                //picture
                if (preparePictureModel)
                {
                    model.DefaultPictureModel = PrepareProductOverviewPictureModel(product, productThumbPictureSize);
                }
                //reviews
                model.ReviewOverviewModel = PrepareProductReviewOverviewModel(product);
                //product attributes
                //model.ProductAttributes = PrepareProductAttributesModel(_productAttributeService, product.Id);
                models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// Prepare the product details model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <param name="isAssociatedProduct">Whether the product is associated</param>
        /// <returns>Product details model</returns>
        public virtual ProductDetailsModelApi PrepareProductDetailsModel(Product product,
            ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            // Added by Alexandar Rajavel on 26-Sep-2019
            var category = _categoryService.GetProductCategoriesByProductId(product.Id).ToList();
            var categoryName = string.Empty;
            if (category != null && category.FirstOrDefault().Category.ParentCategoryId == 0)
            {
                categoryName = category.FirstOrDefault().Category.Name;
            }
            else if (category != null)
            {
                var resultCategory = new Category();
                var parentCategoryId = category.FirstOrDefault().Category.ParentCategoryId;
                do
                {
                    resultCategory = _categoryService.GetCategoryById(parentCategoryId);
                    parentCategoryId = resultCategory.ParentCategoryId;
                }
                while (resultCategory.ParentCategoryId != 0);
                categoryName = resultCategory == null ? string.Empty : resultCategory.Name;
            }

            switch (categoryName.ToLower())
            {
                case "electronics":
                case "liquors store":
                case "fashions":
                case "medicine":
                    break;
                default:
                    categoryName = "Others";
                    break;
            }

            //standard properties
            var model = new ProductDetailsModelApi
            {
                Id = product.Id,
                Name = _localizationService.GetLocalized(product, x => x.Name),
                ProductCategoryName = categoryName.Replace(" ", ""),
                Url = _webHelper.GetStoreLocation() + _urlRecordService.GetSeName(product),
                ShortDescription = _localizationService.GetLocalized(product, x => x.ShortDescription),
                FullDescription = _localizationService.GetLocalized(product, x => x.FullDescription),
                ShowManufacturerPartNumber = _catalogSettings.ShowManufacturerPartNumber,
                FreeShippingNotificationEnabled = _catalogSettings.ShowFreeShippingNotification,
                ManufacturerPartNumber = product.ManufacturerPartNumber,
                StockAvailability = _productService.FormatStockMessage(product, ""),
                HasSampleDownload = product.IsDownload && product.HasSampleDownload,
                //Added By Sunil Kumar at 25-04-19
                AvailableStartDateTimeUtc = product.AvailableStartDateTimeUtc,
                AvailableEndDateTimeUtc = product.AvailableEndDateTimeUtc,
                //Added By Sunil Kumar at 02-04-2020
                CGMItemID = product.CGMItemID,
                Barcode = product.Barcode,
                MaxItemQuantity = 99,
                ERPItemCode = product.ERPItemCode,
                skucodeerp = product.skucodeerp,

                Sku = product.Sku
            };

            //Added By Sunil Kumar at 27/12/19
            DateTime now = DateTime.Now;
            if (product.MarkAsNew && product.MarkAsNewStartDateTimeUtc <= now && product.MarkAsNewEndDateTimeUtc >= now)
            {
                model.MarkAsNew = true;
            }

            //shipping info
            model.IsShipEnabled = product.IsShipEnabled;
            if (product.IsShipEnabled)
            {
                model.IsFreeShipping = product.IsFreeShipping;
                //delivery date
                var deliveryDate = _dateRangeService.GetDeliveryDateById(product.DeliveryDateId);
                if (deliveryDate != null)
                {
                    model.DeliveryDate = _localizationService.GetLocalized(deliveryDate, dd => dd.Name);
                }
            }

            //email a friend
            model.EmailAFriendEnabled = _catalogSettings.EmailAFriendEnabled;
            //compare products
            model.CompareProductsEnabled = _catalogSettings.CompareProductsEnabled;
            //store name
            //model.CurrentStoreName = _storeContext.CurrentStore.GetLocalized(x => x.Name);

            //vendor details
            if (_vendorSettings.ShowVendorOnProductDetailsPage)
            {
                var vendor = _vendorService.GetVendorById(product.VendorId);
                if (vendor != null && !vendor.Deleted && vendor.Active)
                {
                    model.ShowVendor = true;

                    model.VendorModel = new VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = _localizationService.GetLocalized(vendor, x => x.Name),
                        SeName = _urlRecordService.GetSeName(vendor),
                    };
                }
            }

            //back in stock subscriptions
            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                _productService.GetTotalStockQuantity(product) <= 0)
            {
                //out of stock
                model.DisplayBackInStockSubscription = true;
            }

            //breadcrumb
            //do not prepare this model for the associated products. anyway it's not used
            if (_catalogSettings.CategoryBreadcrumbEnabled && !isAssociatedProduct)
            {
                model.Breadcrumb = PrepareProductBreadcrumbModel(product);
            }

            //product tags
            //do not prepare this model for the associated products. anyway it's not used
            if (!isAssociatedProduct)
            {
                model.ProductTags = PrepareProductTagModels(product);
            }

            //pictures
            model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
            var pictureModels = PrepareProductDetailsPictureModel(product, isAssociatedProduct);
            model.DefaultPictureModel = pictureModels.DefaultPictureModel;
            model.PictureModels = pictureModels.PictureModels;

            //price
            model.ProductPrice = PrepareProductPriceModel(product);
            //Discount Percentage
            // Created by Alexandar Rajavel on 05-Oct-2018
            model.ProductPrice.DiscountOfferName = string.Empty;
            if (product.HasDiscountsApplied)
            {
                var percentage = product.DiscountProductMappings.Select(x => new { x.Discount.DiscountPercentage, x.Discount.Name }).ToList();
                // Added by Sunil Kumar on 28-Feb-2019
                var discount = _discountService.GetDiscountPercentageAndName(product);
                model.ProductPrice.DiscountPercentage = discount.Item1;
                model.ProductPrice.DiscountOfferName = discount.Item2;
            }

            // Quantity
            model.Quantity.OrderMinimumQuantity = product.OrderMinimumQuantity;
            model.Quantity.OrderMaximumQuantity = product.OrderMaximumQuantity;
            //model.Quantity.StockQuantity = product.StockQuantity;

            // Added by Alexandar Rajavel on 23-Feb-2019
            switch (product.ManageInventoryMethod)
            {
                case ManageInventoryMethod.ManageStock:
                    model.Quantity.StockQuantity = product.StockQuantity;
                    break;
                case ManageInventoryMethod.ManageStockByAttributes:
                    var checking = _productAttributeService.GetAllProductAttributeCombinations(product.Id);
                    model.Quantity.StockQuantity = checking.Sum(s => s.StockQuantity);
                    break;
            }

            //'Add to cart' model
            model.AddToCart = PrepareProductAddToCartModel(product, updatecartitem);

            //gift card
            if (product.IsGiftCard)
            {
                model.GiftCard.IsGiftCard = true;
                model.GiftCard.GiftCardType = product.GiftCardType;

                if (updatecartitem == null)
                {
                    model.GiftCard.SenderName = _customerService.GetCustomerFullName(_workContext.CurrentCustomer);
                    model.GiftCard.SenderEmail = _workContext.CurrentCustomer.Email;
                }
                else
                {
                    string giftCardRecipientName, giftCardRecipientEmail, giftCardSenderName, giftCardSenderEmail, giftCardMessage;
                    _productAttributeParser.GetGiftCardAttribute(updatecartitem.AttributesXml,
                        out giftCardRecipientName, out giftCardRecipientEmail,
                        out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                    model.GiftCard.RecipientName = giftCardRecipientName;
                    model.GiftCard.RecipientEmail = giftCardRecipientEmail;
                    model.GiftCard.SenderName = giftCardSenderName;
                    model.GiftCard.SenderEmail = giftCardSenderEmail;
                    model.GiftCard.Message = giftCardMessage;
                }
            }

            //product attributes
            model.ProductAttributes = PrepareProductAttributeModels(product, updatecartitem, isAssociatedProduct);

            //for update the Price when ProductAttributes having pre-selected
            //Added by Alexandar Rajavel on 24-May-2019
            if (model.ProductAttributes.Any())
            {
                var finalPrice = 0M;
                foreach (var item in model.ProductAttributes)
                {
                    var attributeValueModel = item.Values.FirstOrDefault(s => s.IsPreSelected);
                    finalPrice += attributeValueModel == null ? 0 : attributeValueModel.PriceAdjustmentValue;
                }
                if (string.IsNullOrEmpty(model.ProductPrice.PriceWithDiscount))
                {
                    model.ProductPrice.Price = finalPrice > decimal.Zero ? _priceFormatter.FormatPrice(model.ProductPrice.PriceValue + finalPrice) : model.ProductPrice.Price;
                }
                else
                {
                    model.ProductPrice.PriceWithDiscount = finalPrice > decimal.Zero ? _priceFormatter.FormatPrice(model.ProductPrice.PriceValue + finalPrice) : model.ProductPrice.PriceWithDiscount;
                }
            }

            //product specifications
            //do not prepare this model for the associated products. anyway it's not used
            if (!isAssociatedProduct)
            {
                model.ProductSpecifications = PrepareProductSpecificationModel(product);
            }

            //product review overview
            model.ProductReviewOverview = PrepareProductReviewOverviewModel(product);

            //tier prices
            if (product.HasTierPrices && _permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.TierPrices = PrepareProductTierPriceModels(product);
            }

            //manufacturers
            //do not prepare this model for the associated products. anywway it's not used
            if (!isAssociatedProduct)
            {
                model.ProductManufacturers = PrepareProductManufacturerModels(product);
            }

            //rental products
            if (product.IsRental)
            {
                model.IsRental = true;
                //set already entered dates attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    model.RentalStartDate = updatecartitem.RentalStartDateUtc;
                    model.RentalEndDate = updatecartitem.RentalEndDateUtc;
                }
            }

            //associated products
            if (product.ProductType == ProductType.GroupedProduct)
            {
                //ensure no circular references
                if (!isAssociatedProduct)
                {
                    var associatedProducts = _productService.GetAssociatedProducts(product.Id, _storeContext.CurrentStore.Id);
                    foreach (var associatedProduct in associatedProducts)
                        model.AssociatedProducts.Add(PrepareProductDetailsModel(associatedProduct, null, true));
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the product reviews model
        /// </summary>
        /// <param name="model">Product reviews model</param>
        /// <param name="product">Product</param>
        /// <returns>Product reviews model</returns>
        public virtual ProductReviewsResponseModel PrepareProductReviewsModel(ProductReviewsResponseModel model, Product product)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (product == null)
                throw new ArgumentNullException("product");

            model.ProductId = product.Id;
            model.ProductName = _localizationService.GetLocalized(product, x => x.Name);
            model.ProductSeName = _urlRecordService.GetSeName(product);

            var productReviews = product.ProductReviews.Where(pr => pr.IsApproved).OrderBy(pr => pr.CreatedOnUtc);
            foreach (var pr in productReviews)
            {
                var customer = pr.Customer;
                model.Items.Add(new ProductReviewModel
                {
                    Id = pr.Id,
                    CustomerId = pr.CustomerId,
                    CustomerName = _customerService.FormatUserName(customer),
                    AllowViewingProfiles = _customerSettings.AllowViewingProfiles && customer != null && !customer.IsGuest(),
                    Title = pr.Title,
                    ReviewText = pr.ReviewText,
                    ReplyText = pr.ReplyText,
                    Rating = pr.Rating,
                    Helpfulness = new ProductReviewHelpfulnessModel
                    {
                        ProductReviewId = pr.Id,
                        HelpfulYesTotal = pr.HelpfulYesTotal,
                        HelpfulNoTotal = pr.HelpfulNoTotal,
                    },
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(pr.CreatedOnUtc, DateTimeKind.Utc).ToString("g"),
                });
            }

            model.AddProductReview.CanCurrentCustomerLeaveReview = _catalogSettings.AllowAnonymousUsersToReviewProduct || !_workContext.CurrentCustomer.IsGuest();
            model.AddProductReview.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnProductReviewPage;

            return model;
        }

        /// <summary>
        /// Created By : Ankur Shrivastava
        /// Date : 23-11-2018
        /// get customer review of product model for cutomer
        /// </summary>
        /// <param name="model">Product reviews model</param>
        /// <param name="product">Product</param>
        /// <returns>Product reviews model</returns>
        public virtual ProductReviewsResponseModel PrepareCustomerProductReviewsModel(ProductReviewsResponseModel model, Product product)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (product == null)
                throw new ArgumentNullException("product");

            model.ProductId = product.Id;
            model.ProductName = _localizationService.GetLocalized(product, x => x.Name);
            model.ProductSeName = _urlRecordService.GetSeName(product);

            var productReviews = product.ProductReviews.Where(pr => pr.CustomerId == _workContext.CurrentCustomer.Id).OrderBy(pr => pr.CreatedOnUtc);
            foreach (var pr in productReviews)
            {
                var customer = pr.Customer;
                model.Items.Add(new ProductReviewModel
                {
                    Id = pr.Id,
                    CustomerId = pr.CustomerId,
                    CustomerName = _customerService.FormatUserName(customer),
                    AllowViewingProfiles = _customerSettings.AllowViewingProfiles && customer != null && !customer.IsGuest(),
                    Title = pr.Title,
                    ReviewText = pr.ReviewText,
                    ReplyText = pr.ReplyText,
                    Rating = pr.Rating,
                    Helpfulness = new ProductReviewHelpfulnessModel
                    {
                        ProductReviewId = pr.Id,
                        HelpfulYesTotal = pr.HelpfulYesTotal,
                        HelpfulNoTotal = pr.HelpfulNoTotal,
                    },
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(pr.CreatedOnUtc, DateTimeKind.Utc).ToString("g"),
                });
            }

            model.AddProductReview.CanCurrentCustomerLeaveReview = _catalogSettings.AllowAnonymousUsersToReviewProduct || !_workContext.CurrentCustomer.IsGuest();
            model.AddProductReview.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnProductReviewPage;

            return model;
        }


        /// <summary>
        /// Prepare the product specification models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of product specification model</returns>
        public virtual IList<ProductSpecificationModel> PrepareProductSpecificationModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_SPECS_MODEL_KEY, product.Id, _workContext.WorkingLanguage.Id);
            return _cacheManager.Get(cacheKey, () =>
                _specificationAttributeService.GetProductSpecificationAttributes(product.Id, 0, null, true)
                .Select(psa =>
                {
                    var m = new ProductSpecificationModel
                    {
                        SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttributeId,
                        SpecificationAttributeName = _localizationService.GetLocalized(psa.SpecificationAttributeOption.SpecificationAttribute, x => x.Name),
                        ColorSquaresRgb = psa.SpecificationAttributeOption.ColorSquaresRgb
                    };

                    switch (psa.AttributeType)
                    {
                        case SpecificationAttributeType.Option:
                            m.ValueRaw = WebUtility.HtmlEncode(_localizationService.GetLocalized(psa.SpecificationAttributeOption, x => x.Name));
                            break;
                        case SpecificationAttributeType.CustomText:
                            m.ValueRaw = WebUtility.HtmlEncode(psa.CustomValue);
                            break;
                        case SpecificationAttributeType.CustomHtmlText:
                            m.ValueRaw = psa.CustomValue;
                            break;
                        case SpecificationAttributeType.Hyperlink:
                            m.ValueRaw = string.Format("{0}", psa.CustomValue);
                            break;
                        default:
                            break;
                    }
                    return m;
                }).ToList()
            );
        }

        /// <summary>
        /// Prepare filter items
        /// Created by Alexandar Rajavel on 11-Nov-2018
        /// </summary>
        /// <returns>List of filter items</returns>
        public List<SpecificationFilterItem> GetFilterItems(IList<Product> products)
        {
            var productAttributeMappings = new List<ProductAttributeMapping>();
            var prdAttribute = new List<SpecificationFilterItem>();

            foreach (var product in products)
            {
                var mappings = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                productAttributeMappings.AddRange(mappings);

                //Added code at 03-04-19 by Sunil Kumar
                prdAttribute.Add(new SpecificationFilterItem()
                {
                    SpecificationAttributeName = "Category",
                    SpecificationAttributeOptionName = product.ProductCategories.Any() ? product.ProductCategories.FirstOrDefault().Category.Name.ToString() : "General",
                    FilterId = 0,
                    ProductId = product.Id
                });
            }
            var filterByProductAttribute = new List<SpecificationFilterItem>();
            foreach (var productAttributes in productAttributeMappings)
            {
                var attributeName = productAttributes.ProductAttribute.Name;
                var productId = productAttributes.ProductId;
                var productAttributeValues = productAttributes.ProductAttributeValues.ToList();
                productAttributeValues.ForEach(x => filterByProductAttribute.Add(new SpecificationFilterItem()
                {
                    SpecificationAttributeName = attributeName,
                    SpecificationAttributeOptionName = x.Name,
                    FilterId = x.Id,
                    ProductId = productId
                }));
            }

            filterByProductAttribute.AddRange(prdAttribute);

            return filterByProductAttribute.OrderBy(saof => saof.SpecificationAttributeName)
                .ThenBy(saof => saof.SpecificationAttributeName)
                .ThenBy(saof => saof.SpecificationAttributeOptionName).ToList();
        }

        /// <summary>
        /// Prepare sort items
        /// Created by Alexandar Rajavel on 13-Nov-2018
        /// </summary>
        /// <returns>List of sort items</returns>
        public dynamic GetAvailableSortOptions()
        {
            var sortItems = new List<SelectListItem>();
            if (_catalogSettings.AllowProductSorting)
            {
                foreach (ProductSortingEnum enumValue in Enum.GetValues(typeof(ProductSortingEnum)))
                {
                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    //var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + ((int)enumValue).ToString(), null);

                    var sortValue = _localizationService.GetLocalizedEnum(enumValue);
                    sortItems.Add(new SelectListItem
                    {
                        Text = sortValue,
                        Value = string.Concat((int)enumValue),
                        Selected = enumValue == 0
                    });
                }
            }
            return sortItems;
        }
        #endregion
    }
}

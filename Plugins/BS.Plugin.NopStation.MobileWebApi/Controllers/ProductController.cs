using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models.Product;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Product;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Product;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Core.Domain.Security;
using System.Net;
using Nop.Services.Customers;
using Castle.Core.Internal;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class ProductController : BaseApiController
    {
        #region Field
        private readonly IProductModelFactoryApi _productModelFactoryApi;
        private readonly IProductService _productService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICategoryService _categoryService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        private readonly ICacheManager _cacheManager;
        private readonly CatalogSettings _catalogSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IShippingService _shippingService;
        private readonly VendorSettings _vendorSettings;
        private readonly IVendorService _vendorService;
        private readonly IMeasureService _measureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IProductTagService _productTagService;
        private readonly CustomerSettings _customerSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly CaptchaSettings _captchaSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IEventPublisher _eventPublisher;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IProductServiceApi _productServiceApi;
        private readonly IOrderService _orderService;
        private readonly IProductReviewService _productReviewService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor
        public ProductController(IProductModelFactoryApi productModelFactoryApi,
            IProductService productService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ICategoryService categoryService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPictureService pictureService,
            IWebHelper webHelper,
            ICacheManager cacheManager,
            CatalogSettings catalogSettings,
            MediaSettings mediaSettings,
            IShippingService shippingService,
            VendorSettings vendorSettings,
            IVendorService vendorService,
            IMeasureService measureService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IManufacturerService manufacturerService,
            ISpecificationAttributeService specificationAttributeService,
            ShoppingCartSettings shoppingCartSettings,
            IProductTagService productTagService,
            CustomerSettings customerSettings,
            IDateTimeHelper dateTimeHelper,
            CaptchaSettings captchaSettings,
            ICustomerActivityService customerActivityService,
            LocalizationSettings localizationSettings,
            IEventPublisher eventPublisher,
            IProductServiceApi productServiceApi,
            IOrderService orderService,
            IProductReviewService productReviewService,
            ICustomerService customerService)
        {
            this._productModelFactoryApi = productModelFactoryApi;
            this._productService = productService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._categoryService = categoryService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._webHelper = webHelper;
            this._cacheManager = cacheManager;
            this._catalogSettings = catalogSettings;
            this._mediaSettings = mediaSettings;
            this._shippingService = shippingService;
            this._vendorService = vendorService;
            this._vendorSettings = vendorSettings;
            this._measureService = measureService;
            this._productAttributeParser = productAttributeParser;
            this._productAttributeService = productAttributeService;
            this._manufacturerService = manufacturerService;
            this._specificationAttributeService = specificationAttributeService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._productTagService = productTagService;
            this._customerSettings = customerSettings;
            this._captchaSettings = captchaSettings;
            this._dateTimeHelper = dateTimeHelper;
            this._customerActivityService = customerActivityService;
            this._localizationSettings = localizationSettings;
            this._eventPublisher = eventPublisher;
            this._productServiceApi = productServiceApi;
            this._orderService = orderService; //Added By Ankur
            _productReviewService = productReviewService;
            _customerService = customerService;
        }
        #endregion

        #region Action Method
        [Route("api/homepageproducts")]
        [HttpGet]
        public IActionResult HomepageProducts(int? thumbPictureSize = null, int orderBy = 0)
        {
            if (!thumbPictureSize.HasValue)
            {
                thumbPictureSize = _mediaSettings.ProductThumbPictureSize;
            }
            var products = _productService.GetAllProductsDisplayedOnHomePage();

            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            // Sorting the products, Added by Alexandar Rajavel on 15-Nov-2018
            switch (orderBy)
            {
                case 0:
                    {
                        products = products.OrderBy(p => p.DisplayOrder).ToList();
                        break;
                    }
                case 5:
                    {
                        products = products.OrderBy(p => p.Name).ToList();
                        break;
                    }
                case 6:
                    {
                        products = products.OrderByDescending(p => p.Name).ToList();
                        break;
                    }
                case 10:
                    {
                        products = products.OrderBy(p => p.Price).ToList();
                        break;
                    }
                case 11:
                    {
                        products = products.OrderByDescending(p => p.Price).ToList();
                        break;
                    }
                case 15:
                    {
                        products = products.OrderByDescending(p => p.CreatedOnUtc).ToList();
                        break;
                    }
            }

            var model = _productModelFactoryApi.PrepareProductOverviewModels(products, true, true, thumbPictureSize).ToList();
            var result = new ProductOverViewWithFilterModelApi();
            result.Data = model;
            var maxPrice = products.Any() ? products.Max(p => p.Price) : 0;
            var minPrice = products.Any() ? products.Min(p => p.Price) : 0;
            result.PriceRange = new PriceRange() { From = minPrice, To = maxPrice };
            result.NotFilteredItems = _productModelFactoryApi.GetFilterItems(products);
            result.AvailableSortOptions = _productModelFactoryApi.GetAvailableSortOptions();
            return Ok(result);
        }

        [Route("api/productdetails/{productId}")]
        [HttpGet]
        public IActionResult ProductDetails(int productId, int updatecartitemid = 0)
        {
            Product product = null;
            bool success = false;
            ShoppingCartItem updatecartitem = null;

            product = _productService.GetProductById(productId);
            success = true;

            if (product == null || product.Deleted)
                return Unauthorized();

            IList<int> productIds = new List<int>();
            var lastOrDefault = product.ProductCategories.LastOrDefault();
            if (lastOrDefault != null)
            {
                productIds =
                   _productServiceApi.GetPreviousAndNextProducts(lastOrDefault.CategoryId,
                       productId);
            }
            //Is published?
            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a product before publishing
            if (!product.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return Unauthorized();

            //ACL (access control list)
            if (!_aclService.Authorize(product))
                return Unauthorized();

            //Store mapping
            if (!_storeMappingService.Authorize(product))
                return Unauthorized();

            //availability dates
            if (!product.IsAvailable())
                return Unauthorized();

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct != null)
                    product = parentGroupedProduct;
                else
                {
                    return Unauthorized();
                }
                //return RedirectToRoute("Product", new { SeName = parentGroupedProduct.GetSeName() });
            }

            //update existing shopping cart item?

            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    product = _productService.GetProductById(productId);
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    product = _productService.GetProductById(productId);
                }
            }

            //prepare the model
            var model = _productModelFactoryApi.PrepareProductDetailsModel(product, updatecartitem, false);
            if (productIds.Count >= 2)
            {
                model.NextProduct = productIds[0];
                model.PreviousProduct = productIds[1];
            }

            //save as recently viewed,   Added by Alexandar Rajavel on 10-May-2019 
            var customer = _workContext.CurrentCustomer;
            var strPrdId = product.Id.ToString();
            if (_catalogSettings.RecentlyViewedProductsEnabled)
            {
                if (customer.RecentlyViewedProductsIds == null || customer.RecentlyViewedProductsIds == "")
                {
                    customer.RecentlyViewedProductsIds = strPrdId;
                    _customerService.UpdateCustomer(customer);
                }
                if (customer.RecentlyViewedProductsIds != null)
                {
                    var productsIds = _workContext.CurrentCustomer.RecentlyViewedProductsIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //whether product identifier to add already exist
                    if (!productsIds.Contains(strPrdId))
                    {
                        customer.RecentlyViewedProductsIds = strPrdId + COMMA.Trim() + customer.RecentlyViewedProductsIds;
                        _customerService.UpdateCustomer(customer);
                    }
                }
            }


            ////activity log
            //_customerActivityService.InsertActivity("PublicStore.ViewProduct", _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name);

            //return View(model.ProductTemplateViewPath, model);
            var result = new GeneralResponseModel<ProductDetailsModelApi>()
            {
                Data = model
            };
            return Ok(result);
        }

        //Added by Alexandar Rajavel on 10-May-2019
        [Route("api/product/RecentlyViewedProducts")]
        [HttpGet]
        public IActionResult RecentlyViewedProducts(int thumbPictureSize = 400)
        {
            IList<ProductOverViewModelApi> model = null;
            if (_catalogSettings.RecentlyViewedProductsEnabled && _workContext.CurrentCustomer.RecentlyViewedProductsIds != null)
            {
                var productIds = _workContext.CurrentCustomer.RecentlyViewedProductsIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var products = _productService.GetProductsByIds(productIds.Select(int.Parse).Take(_catalogSettings.RecentlyViewedProductsNumber).ToArray()).Where(product => product.Published && !product.Deleted).ToList();
                if (products.Count > 0)
                {
                    model = _productModelFactoryApi.PrepareProductOverviewModels(products, true, true, thumbPictureSize).ToList();
                }
                else
                {
                    model = null;
                }
            }
            var result = new GeneralResponseModel<IList<ProductOverViewModelApi>>()
            {
                Data = model
            };
            return Ok(result);
        }

        [Route("api/relatedproducts/{productId}")]
        [HttpGet]
        public IActionResult RelatedProducts(int productId, int thumbPictureSize = 400)
        {
            //load and cache report
            var productIds = _cacheManager.Get(string.Format(ModelCacheEventConsumer.PRODUCTS_RELATED_IDS_KEY, productId, _storeContext.CurrentStore.Id),
                () =>
                    _productService.GetRelatedProductsByProductId1(productId).Select(x => x.ProductId2).ToArray()
                    );

            //load products
            var products = _productService.GetProductsByIds(productIds);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();
            IList<ProductOverViewModelApi> model;
            if (products.Count > 0)
            {
                model = _productModelFactoryApi.PrepareProductOverviewModels(products, true, true, thumbPictureSize).ToList();
            }
            else
            {
                model = null;
            }

            var result = new GeneralResponseModel<IList<ProductOverViewModelApi>>()
            {
                Data = model
            };
            return Ok(result);
        }

        [Route("api/product/productreviews/{productId}")]
        [HttpGet]
        public IActionResult ProductReviews(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return Challenge();

            var model = new ProductReviewsResponseModel();
            model = _productModelFactoryApi.PrepareProductReviewsModel(model, product);
            //only registered users can leave reviews
            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                //ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
                model.AddProductReview.CanCurrentCustomerLeaveReview = false;
            //default value
            model.AddProductReview.Rating = _catalogSettings.DefaultProductRatingValue;
            return Ok(model);
        }

        /// <summary>
        /// Created By : Ankur Shrivastava
        /// Created Date : 23/11/2018
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [Route("api/product/getcustomerproductreview/{productId}")]
        [HttpGet]
        public IActionResult GetCustomerProductReview(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                //added by Sunil Kumar at 30-12-19
                return NotFound(new { ProductId = productId, Message = "Product is not Allowed for Customer Reviews" });
            //return Challenge();

            var model = new ProductReviewsResponseModel();
            model = _productModelFactoryApi.PrepareCustomerProductReviewsModel(model, product);
            //only registered users can leave reviews
            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                //ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
                model.AddProductReview.CanCurrentCustomerLeaveReview = false;
            //default value
            model.AddProductReview.Rating = _catalogSettings.DefaultProductRatingValue;
            return Ok(model);
        }

        [Route("api/product/productreviewsadd/{productId}")]
        [HttpPost]
        public IActionResult ProductReviewsAdd(int productId, [FromBody]ProductReviewQueryModel model)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                //added by Sunil Kumar at 30-12-19
                return NotFound(new { ProductId = productId, Message = "Product is not Allowed for Customer Reviews" });
            // return Challenge();
            var response = new GeneralResponseModel<bool>();

            //checking if user has purchased the product or not.   By Ankur on 8 Nov. 2018 for EC-203
            if (_orderService.SearchOrders(_storeContext.CurrentStore.Id, 0, _workContext.CurrentCustomer.Id, productId).Count > 0)
            {
                //commented at 21-12-18 for removing review title and text by Sunil
                //ValidationExtension.WriteReviewValidator(ModelState, model, _localizationService);
                if (ModelState.IsValid)
                {
                    //save review
                    int rating = model.Rating;
                    if (rating < 1 || rating > 5)
                        rating = _catalogSettings.DefaultProductRatingValue;
                    bool isApproved = !_catalogSettings.ProductReviewsMustBeApproved;

                    var productReview = new ProductReview
                    {
                        Id = product.ProductReviews.FirstOrDefault(pr => pr.CustomerId == _workContext.CurrentCustomer.Id) != null ?  //null check
                             product.ProductReviews.FirstOrDefault(pr => pr.CustomerId == _workContext.CurrentCustomer.Id).Id : 0,
                        ProductId = product.Id,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        Title = model.Title,
                        ReviewText = model.ReviewText,
                        Rating = rating,
                        HelpfulYesTotal = 0,
                        HelpfulNoTotal = 0,
                        IsApproved = isApproved,
                        StoreId = _storeContext.CurrentStore.Id,  //Added By Ankur for Add Product Review Bug Review Bug
                        CreatedOnUtc = DateTime.UtcNow,
                    };
                    if (!product.ProductReviews.Any(pr => pr.CustomerId == _workContext.CurrentCustomer.Id))
                    {
                        product.ProductReviews.Add(productReview);
                        _productService.UpdateProduct(product);

                        //update product review totals
                        _productService.UpdateProductReviewTotals(product);
                    }
                    else
                    {
                        productReview = product.ProductReviews.FirstOrDefault(pr => pr.CustomerId == _workContext.CurrentCustomer.Id && pr.ProductId == productId);
                        productReview.ReviewText = model.ReviewText;
                        productReview.Title = model.Title;
                        productReview.Rating = rating;
                        productReview.IsApproved = false; // on update of review need to review again by admin.
                        _productReviewService.UpdateProductReview(productReview);
                    }

                    //notify store owner
                    if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                        _workflowMessageService.SendProductReviewNotificationMessage(productReview, _localizationSettings.DefaultAdminLanguageId);

                    //activity log
                    _customerActivityService.InsertActivity("PublicStore.AddProductReview", _localizationService.GetResource("ActivityLog.PublicStore.AddProductReview"), product);

                    //raise event
                    if (productReview.IsApproved)
                        _eventPublisher.Publish(new ProductReviewApprovedEvent(productReview));

                    response.Data = true;
                    if (!isApproved)
                        response.SuccessMessage = _localizationService.GetResource("Reviews.SeeAfterApproving");
                    else
                        response.SuccessMessage = _localizationService.GetResource("Reviews.SuccessfullyAdded");

                    return Ok(response);
                }
                else
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            response.ErrorList.Add(error.ErrorMessage);
                        }
                    }
                    response.StatusCode = (int)ErrorType.NotOk;
                    response.Data = false;
                    return Ok(response);
                }
                //If we got this far, something failed, redisplay form
                //PrepareProductReviewsModel(model, product);
            }
            else
            {
                response.ErrorList.Add(_localizationService.GetResource("Reviews.PurchaseBeforeReview"));
                response.StatusCode = (int)ErrorType.NotOk;
                response.Data = false;
                return Ok(response);
            }



        }

        //Sunil Kumar Code at 22/09/18
        [Route("api/product/GetProductIdbyBarcode/{barcode}")]
        [HttpGet]
        public IActionResult GetProductIdbyBarcode(string Barcode)
        {
            var product = _productService.GetProductIdbyBarcode(Barcode);
            if (product == null) //|| product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return NotFound(new { ProductId = 0, Message = BARCODE_NOT_FOUND });
            return Ok(new { ProductId = product.Id, Message = SUCCESS });
        }

        [Route("api/product/GetProductAttributeMeasurement/{productAttributeValueId}")]
        [HttpPost]
        public IActionResult GetProductAttributeMeasurement([FromBody] int productAttributeValueId)
        {
            var result = _productAttributeService.GetProductAttributeValueById(productAttributeValueId);
            var response = new GeneralResponseModel<string>();
            response.StatusCode = (int)HttpStatusCode.OK;
            response.Data = result.MeasurementData;

            return Ok(response);
        }

        //Added by Alexandar Rajavel on 29-Mar-2019
        [Route("api/product/SaveNotifyRequest/{productId}")]
        [HttpGet]
        public IActionResult SaveNotifyRequest(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                return NotFound(new { ProductId = productId, Message = "Product id not found" });
            }
            var productsOldQty = 0;
            switch (product.ManageInventoryMethod)
            {
                case ManageInventoryMethod.ManageStock:
                    productsOldQty = product.StockQuantity;
                    break;
                case ManageInventoryMethod.ManageStockByAttributes:
                    var checking = _productAttributeService.GetAllProductAttributeCombinations(product.Id);
                    productsOldQty = checking.Sum(s => s.StockQuantity);
                    break;
            }
            var isManageStock = product.ManageInventoryMethod == ManageInventoryMethod.ManageStock ? true : false;
            var customersId = string.Empty;
            if (isManageStock)
            {
                if (!(string.IsNullOrEmpty(product.NotificationToCustomers)) ? product.NotificationToCustomers.Contains(_workContext.CurrentCustomer.Id.ToString()) : false)
                {
                    return Ok(new { ProductId = product.Id, Message = "Already saved your notify request" });
                }
                else
                {
                    customersId = string.IsNullOrEmpty(product.NotificationToCustomers) ? _workContext.CurrentCustomer.Id.ToString() : product.NotificationToCustomers + COMMA + _workContext.CurrentCustomer.Id;
                    product.NotificationToCustomers = customersId;
                    _productService.UpdateProduct(product);
                    return Ok(new { ProductId = product.Id, Message = "Your notify request saved" });
                }
            }
            else
            {
                if (!(string.IsNullOrEmpty(product.NotificationToCustomers_1)) ? product.NotificationToCustomers_1.Contains(_workContext.CurrentCustomer.Id.ToString()) : false)
                {
                    return Ok(new { ProductId = product.Id, Message = "Already saved your notify request" });
                }
                else
                {
                    customersId = string.IsNullOrEmpty(product.NotificationToCustomers_1) ? _workContext.CurrentCustomer.Id.ToString() : product.NotificationToCustomers_1 + COMMA + _workContext.CurrentCustomer.Id;
                    product.NotificationToCustomers_1 = customersId;
                    _productService.UpdateProduct(product);
                    return Ok(new { ProductId = product.Id, Message = "Your notify request saved" });
                }
            }
        }
        #endregion
    }
}

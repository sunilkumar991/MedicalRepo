using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using BS.Plugin.NopStation.MobileWebApi.Model;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models.Product;
using BS.Plugin.NopStation.MobileWebApi.Models.Vendor;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Framework.Extensions;
//using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class VendorController : BaseApiController
    {
        #region Field

        private readonly ILocalizationService _localizationService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerModelFactoryApi _customerModelFactoryApi;
        private readonly IWorkContext _workContext;
        private readonly IProductService _productService;
        private readonly IProductModelFactoryApi _productModelFactoryApi;
        private readonly IPermissionService _permissionService;
        private readonly VendorSettings _vendorSettings;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IDiscountService _discountService;
        private readonly IProductTagService _productTagService;
        private readonly IShippingService _shippingService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly TaxSettings _taxSettings;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly ISettingModelFactory _settingModelFactory;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IShipmentService _shipmentService;
        private readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
        //added for get all Category 
        private readonly ICatalogModelFactoryApi _catalogModelFactoryApi;
        private readonly IDownloadService _downloadService;
        private readonly IPictureService _pictureService;
        private readonly IDeviceService _deviceService;
        private readonly INotificationService _notificationService;
        private readonly CustomerSettings _customerSettings;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Constructor

        public VendorController(ILocalizationService localizationService, ICustomerRegistrationService customerRegistrationService, ICustomerService customerService, ICustomerModelFactoryApi customerModelFactoryApi,
              IWorkContext workContext, IProductService productService, IProductModelFactoryApi productModelFactoryApi, IPermissionService permissionService, VendorSettings vendorSettings, IUrlRecordService urlRecordService, ILocalizedEntityService localizedEntityService, ICategoryService categoryService,
               IManufacturerService manufacturerService, IAclService aclService, IStoreMappingService storeMappingService, IStoreService storeService, IDiscountService discountService, IProductTagService productTagService, IShippingService shippingService, ICustomerActivityService customerActivityService, IDateTimeHelper dateTimeHelper,
                 IProductAttributeService productAttributeService, IStaticCacheManager cacheManager, ISpecificationAttributeService specificationAttributeService, IBaseAdminModelFactory baseAdminModelFactory, TaxSettings taxSettings
            , ICurrencyService currencyService, CurrencySettings currencySettings, IMeasureService measureService, MeasureSettings measureSettings, ILocalizedModelFactory localizedModelFactory, ISettingModelFactory settingModelFactory, IStoreContext storeContext
            , ISettingService settingService, IProductTemplateService productTemplateService, IShipmentService shipmentService, IDiscountSupportedModelFactory discountSupportedModelFactory, ICatalogModelFactoryApi catalogModelFactoryApi, IDownloadService downloadService, IPictureService pictureService, IDeviceService deviceService,
                 INotificationService notificationService, CustomerSettings customerSettings, IGenericAttributeService genericAttributeService)
        {
            _localizationService = localizationService;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _customerModelFactoryApi = customerModelFactoryApi;
            _workContext = workContext;
            this._productService = productService;
            this._productModelFactoryApi = productModelFactoryApi;
            this._permissionService = permissionService;
            this._vendorSettings = vendorSettings;
            this._urlRecordService = urlRecordService;
            this._localizedEntityService = localizedEntityService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
            this._discountService = discountService;
            this._productTagService = productTagService;
            this._shippingService = shippingService;
            this._customerActivityService = customerActivityService;
            this._dateTimeHelper = dateTimeHelper;
            this._productAttributeService = productAttributeService;
            this._cacheManager = cacheManager;
            this._specificationAttributeService = specificationAttributeService;
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._taxSettings = taxSettings;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._measureService = measureService;
            this._measureSettings = measureSettings;
            this._localizedModelFactory = localizedModelFactory;
            this._settingModelFactory = settingModelFactory;
            this._storeContext = storeContext;
            this._settingService = settingService;
            this._productTemplateService = productTemplateService;
            this._shipmentService = shipmentService;
            this._discountSupportedModelFactory = discountSupportedModelFactory;
            this._catalogModelFactoryApi = catalogModelFactoryApi;
            this._downloadService = downloadService;
            this._pictureService = pictureService;
            _deviceService = deviceService;
            _notificationService = notificationService;
            this._customerSettings = customerSettings;
            this._genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Utilities


        #endregion

        #region Action methods

        [Route("api/vendor/login")]
        [HttpPost]
        public IActionResult Login([FromBody]LoginQueryModel model)
        {
            var customerLoginModel = new LogInPostResponseModel();
            customerLoginModel = PrepareLogInPostResponseModel(model);
            return Ok(customerLoginModel);
        }

        [Route("api/vendor/registervendorapi")]
        [HttpPost]
        public IActionResult RegisterVendorApi([FromBody]VendorRegisterQueryModel model)
        {
            var baseResponse = new BaseResponse();
            var existingcustomer = new Customer();
            var existingcustomerbyemail = new Customer();
            try
            {
                if (string.IsNullOrEmpty(model.FirstName))
                {
                    baseResponse.StatusCode = (int)ErrorType.NotFound;
                    baseResponse.ErrorList.Add("First Name Required.");
                    return Ok(baseResponse);
                }
                else if (string.IsNullOrEmpty(model.LastName))
                {
                    baseResponse.StatusCode = (int)ErrorType.NotFound;
                    baseResponse.ErrorList.Add("Last Name Required.");
                    return Ok(baseResponse);
                }
                else if (string.IsNullOrEmpty(model.MobileNumber))
                {
                    baseResponse.StatusCode = (int)ErrorType.NotFound;
                    baseResponse.ErrorList.Add("Mobile Number Required.");
                    return Ok(baseResponse);

                }
                else if (string.IsNullOrEmpty(model.Address1))
                {
                    baseResponse.StatusCode = (int)ErrorType.NotFound;
                    baseResponse.ErrorList.Add("Address1 Required.");
                    return Ok(baseResponse);
                }
                else if (string.IsNullOrEmpty(model.Password))
                {
                    baseResponse.StatusCode = (int)ErrorType.NotFound;
                    baseResponse.ErrorList.Add("Password Required.");
                    return Ok(baseResponse);
                }
                else if (string.IsNullOrEmpty(model.Email))
                {
                    baseResponse.StatusCode = (int)ErrorType.NotFound;
                    baseResponse.ErrorList.Add("Email Required.");
                    return Ok(baseResponse);

                }
                else if (string.IsNullOrEmpty(model.ShopName))
                {
                    baseResponse.StatusCode = (int)ErrorType.NotFound;
                    baseResponse.ErrorList.Add("Shop Name Required.");
                    return Ok(baseResponse);

                }
                existingcustomer = _customerService.GetCustomerByUsername(model.MobileNumber);
                existingcustomerbyemail = _customerService.GetCustomerByEmail(model.Email);
                if (existingcustomer == null && existingcustomerbyemail == null)
                {
                    var customer = new Customer
                    {
                        CustomerGuid = Guid.NewGuid(),
                        Email = model.Email,
                        Username = model.MobileNumber,
                        VendorId = 0,
                        AdminComment = "",
                        IsTaxExempt = false,
                        Active = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        LastActivityDateUtc = DateTime.UtcNow,
                        RegisteredInStoreId = _storeContext.CurrentStore.Id,
                        ShopName = model.ShopName
                    };
                    _customerService.InsertCustomer(customer);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);

                    //insert default address (if possible)
                    var defaultAddress = new Address
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        CountryId = 60,
                        StateProvinceId = model.StateId == 0 ? 83 : model.StateId,
                        CityId = model.CityId == 0 ? 307 : model.CityId,
                        Address1 = model.Address1,
                        Address2 = model.Address2,
                        PhoneNumber = model.MobileNumber,
                        CreatedOnUtc = DateTime.UtcNow,
                        HouseNo = model.HouseNo,
                        FloorNo = model.FloorNo,
                        RoomNo = model.RoomNo,
                        CountryCode = "MM",
                        Latitude = model.Latitude,
                        Longitude = model.Longitude
                    };

                    customer.CustomerAddressMappings.Add(new CustomerAddressMapping
                    {
                        Address = defaultAddress
                    });

                    //customer roles
                    var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
                    var newCustomerRoles = new List<CustomerRole>();

                    foreach (var customerRole in allCustomerRoles)
                        if (customerRole.SystemName == NopCustomerDefaults.RegisteredRoleName || customerRole.SystemName == NopCustomerDefaults.VendorsRoleName)
                        {
                            newCustomerRoles.Add(customerRole);
                        }

                    foreach (var customerRole in newCustomerRoles)
                    {
                        //ensure that the current customer cannot add to "Administrators" system role if he's not an admin himself
                        if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName && !_workContext.CurrentCustomer.IsAdmin())
                            continue;

                        customer.CustomerCustomerRoleMappings.Add(new CustomerCustomerRoleMapping { CustomerRole = customerRole });
                    }

                    _customerService.UpdateCustomer(customer);


                    //password
                    if (!string.IsNullOrWhiteSpace(model.Password))
                    {
                        var changePassRequest = new ChangePasswordRequest(model.Email, false, _customerSettings.DefaultPasswordFormat, model.Password);
                        var changePassResult = _customerRegistrationService.ChangePassword(changePassRequest);
                        if (!changePassResult.Success)
                        {
                            baseResponse.StatusCode = (int)ErrorType.NotFound;
                            if (changePassResult.Errors.Count > 0)
                            {
                                baseResponse.ErrorList.Add("Error in Registration." + changePassResult.Errors[0].ToString());
                            }
                            else
                            {
                                baseResponse.ErrorList.Add("Error in Registration.");
                            }

                            return Ok(baseResponse);
                        }
                    }

                    //activity log
                    _customerActivityService.InsertActivity("AddNewCustomer",
                        string.Format(_localizationService.GetResource("ActivityLog.AddNewCustomer"), customer.Id), customer);

                    var customerLoginModel = new LogInPostResponseModel();
                    var loginModel = new LoginQueryModel();
                    loginModel.Username = customer.Username;
                    loginModel.Password = model.Password;
                    customerLoginModel = PrepareLogInPostResponseModel(loginModel);
                    return Ok(customerLoginModel);

                }
                else if (existingcustomerbyemail != null)
                {
                    baseResponse.StatusCode = (int)ErrorType.NotFound;
                    baseResponse.ErrorList.Add("Email already Registered.");
                    return Ok(baseResponse);
                }
                else
                {
                    baseResponse.StatusCode = (int)ErrorType.NotFound;
                    baseResponse.ErrorList.Add("User already Registered with Mobile No.");
                    return Ok(baseResponse);

                }

            }
            catch (Exception ex)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Error in Registration " + ex.Message);
                return Ok(baseResponse);
            }
         
        }

        [Route("api/vendor/dashboard")]
        [HttpGet]
        public IActionResult Dashboard()
        {
            var customer = _customerService.GetCustomerById(_workContext.CurrentCustomer.Id);
            var products = _productService.GetAllProductsCountByVendorId(customer.VendorId);

            if (products != 0)
            {
                var dashboardDetails = new VendorDashboardResponseModel();
                dashboardDetails.ProductCount = products;
                return Ok(dashboardDetails);
            }
            else
            {
                var dashboardDetails = new VendorDashboardResponseModel();
                dashboardDetails.StatusCode = (int)ErrorType.NotFound;
                dashboardDetails.ErrorList.Add(NO_DATA);
                return Ok(dashboardDetails);
            }


        }

        [Route("api/vendor/productdetails")]
        [HttpGet]
        public IActionResult VendorProductDetails(int pageNumber = 1, int pageSize = 10)
        {
            var customer = _customerService.GetCustomerById(_workContext.CurrentCustomer.Id);
            var products = _productService.SearchProducts(
               categoryIds: new List<int> { },
               manufacturerId: 0,
               storeId: 0,
               vendorId: customer.VendorId,
               productType: null,
               keywords: null,
               pageIndex: pageNumber - 1,
               pageSize: pageSize,
               showHidden: true
               );
            var searchModelApi = new SearchModelApi();
            searchModelApi.Products = _productModelFactoryApi.PrepareProductOverviewModels(products).ToList();
            searchModelApi.NotFilteredItems = _productModelFactoryApi.GetFilterItems(products);
            //searchModelApi.NoResults = !model.Products.Any();
            searchModelApi.PagingFilteringContext.LoadPagedList(products);
            var price = new PriceRange();
            if (searchModelApi.Products.Any())
            {
                price.From = searchModelApi.Products.Min(p => p.ProductPrice.PriceValue);
                price.To = searchModelApi.Products.Max(p => p.ProductPrice.PriceValue);
            }
            searchModelApi.PriceRange = price;

            var result = PrepareSearchProductResponseModel(searchModelApi);
            return Ok(result);
        }

        [Route("api/vendor/createproduct")]
        [HttpPost]
        public IActionResult CreateProduct([FromBody]ProductsModel model)
        {
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
                model.VendorId = _workContext.CurrentVendor.Id;
            if (model.Name == "" || model.DeliveryDateId == 0 || model.VendorId == 0)
            {
                var baseResponse = new BaseResponse();
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Name & Delivery Date Fields are Required.");
                return Ok(baseResponse);
            }

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
            {
                var baseResponse = new BaseResponse();
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Error in Add Product ");
                return Ok(baseResponse);
            }

            Product entity = new Product();
            try
            {
                entity = ToProductEntity(model);
                entity.CreatedOnUtc = DateTime.UtcNow;
                entity.UpdatedOnUtc = DateTime.UtcNow;
                _productService.InsertProduct(entity);
                var baseResponse = new BaseResponse();
                return Ok(baseResponse);
            }
            catch (Exception ex)
            {
                var baseResponse = new BaseResponse();
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Error in Add Product " + ex.Message);
                return Ok(baseResponse);
            }

            ////search engine name
            //model.SeName = _urlRecordService.ValidateSeName(product, model.SeName, product.Name, true);
            //_urlRecordService.SaveSlug(product, model.SeName, 0);

            ////locales
            //UpdateLocales(product, model);

            ////categories
            //SaveCategoryMappings(product, model);

            ////manufacturers
            //SaveManufacturerMappings(product, model);

            ////ACL (customer roles)
            //SaveProductAcl(product, model);

            ////stores
            //SaveStoreMappings(product, model);

            ////discounts
            //SaveDiscountMappings(product, model);

            ////tags
            //_productTagService.UpdateProductTags(product, ParseProductTags(model.ProductTags));

            ////warehouses
            //SaveProductWarehouseInventory(product, model);

            ////quantity change history
            //_productService.AddStockQuantityHistoryEntry(product, product.StockQuantity, product.StockQuantity, product.WarehouseId,
            //    _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Edit"));

            ////activity log
            //_customerActivityService.InsertActivity("AddNewProduct",
            //    string.Format(_localizationService.GetResource("ActivityLog.AddNewProduct"), product.Name), product);

        }

        [Route("api/vendor/getproductbyid/{productId}")]
        [HttpGet]
        public IActionResult GetProductById(string productId)
        {
            var baseResponse = new BaseResponse();

            // get a product with the specified id
            var product = _productService.GetProductById(Convert.ToInt32(productId));
            if (product == null || product.Deleted || product.VendorId != _workContext.CurrentVendor.Id)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Product Not Available.");
                return Ok(baseResponse);
            }
            //var model = ToProductModel(product);
            //var result = new CommonResponseModel<ProductsModel>()
            //{
            //    Data = model
            //};

            var model = _productModelFactoryApi.PrepareProductDetailsModel(product, null, false);


            var result = new CommonResponseModel<ProductDetailsModelApi>()
            {
                Data = model
            };

            return Ok(result);
        }

        [Route("api/vendor/createproductfromvendor")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult CreateProductFromVendor([FromForm]ProductsModel model)
        {
            var baseResponse = new BaseResponse();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
                model.VendorId = _workContext.CurrentVendor.Id;
            if (model.Name == "" || model.DeliveryDateId == 0 || model.VendorId == 0)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Name & Delivery Date Fields are Required.");
                return Ok(baseResponse);
            }
            if (model.Files == null || model.Files.Length == 0)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Product Image Required.");
                return Ok(baseResponse);
            }

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Error in Add Product ");
                return Ok(baseResponse);
            }

            Product entity = new Product();
            try
            {

                if (model.Files != null && model.Files.Length != 0)
                {
                    var pictureId = 0;
                    var displayOrder = 0;

                    try
                    {
                        entity = ToProductEntity(model);
                        entity.CreatedOnUtc = DateTime.UtcNow;
                        entity.UpdatedOnUtc = DateTime.UtcNow;
                        _productService.InsertProduct(entity);

                        foreach (IFormFile photo in model.Files)
                        {
                            var contentType = photo.ContentType;
                            var vendorPictureBinary = _downloadService.GetDownloadBits(photo);
                            var picture = _pictureService.InsertPicture(vendorPictureBinary, contentType, null);

                            if (picture != null)
                                pictureId = picture.Id;
                            if (pictureId != 0)
                            {
                                _productService.InsertProductPicture(new ProductPicture
                                {
                                    PictureId = pictureId,
                                    ProductId = entity.Id,
                                    DisplayOrder = displayOrder
                                });
                            }
                        }
                        //insert the new product category mapping
                        if (model.CategoryId != 0)
                        {
                            _categoryService.UpdateProductCategory(new ProductCategory
                            {
                                CategoryId = model.CategoryId,
                                ProductId = entity.Id,
                                IsFeaturedProduct = false,
                                DisplayOrder = 1
                            });
                        }

                        //quantity change history
                        _productService.AddStockQuantityHistoryEntry(entity, entity.StockQuantity, entity.StockQuantity, entity.WarehouseId,
                            _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Edit"));

                        //activity log
                        _customerActivityService.InsertActivity("AddNewProduct",
                            string.Format(_localizationService.GetResource("ActivityLog.AddNewProduct"), entity.Name), entity);

                    }
                    catch (Exception ex)
                    {
                        baseResponse.StatusCode = (int)ErrorType.NotFound;
                        baseResponse.ErrorList.Add("Error in Add Product " + ex.Message);
                        return Ok(baseResponse);
                    }
                }
                return Ok(baseResponse);
            }
            catch (Exception ex)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Error in Add Product " + ex.Message);
                return Ok(baseResponse);
            }
        }

        [Route("api/vendor/updateproductfromvendor")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult UpdateProductFromVendor([FromForm]ProductsModel model)
        {
            var baseResponse = new BaseResponse();
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Access Denied.");
                return Ok(baseResponse);
            }

            //try to get a product with the specified id
            var product = _productService.GetProductById(model.Id);
            if (product == null || product.Deleted)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Product Not Available.");
                return Ok(baseResponse);
            }
            if (_workContext.CurrentVendor != null)
                model.VendorId = _workContext.CurrentVendor.Id;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Access Denied.");
                return Ok(baseResponse);
            }

            //check if the product quantity has been changed while we were editing the product
            //and if it has been changed then we show error notification
            //and redirect on the editing page without data saving
            //if (product.StockQuantity != model.LastStockQuantity)
            //{
            //    baseResponse.StatusCode = (int)ErrorType.NotFound;
            //    baseResponse.ErrorList.Add(_localizationService.GetResource("Admin.Catalog.Products.Fields.StockQuantity.ChangedWarning"));
            //    return Ok(baseResponse);
            //}

            if (model.Name == "" || model.DeliveryDateId == 0 || model.VendorId == 0)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Name & Delivery Date Fields are Required.");
                return Ok(baseResponse);
            }
            if (model.Name != "" && model.DeliveryDateId != 0 && model.VendorId != 0)//&& model.Files != null && model.Files.Length != 0)
            {
                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null)
                    model.VendorId = _workContext.CurrentVendor.Id;

                //we do not validate maximum number of products per vendor when editing existing products (only during creation of new products)
                //vendors cannot edit "Show on home page" property
                if (_workContext.CurrentVendor != null && model.ShowOnHomePage != product.ShowOnHomePage)
                    model.ShowOnHomePage = product.ShowOnHomePage;
                try
                {
                    if (product.StockQuantity <= 0 && model.StockQuantity > 0 && !string.IsNullOrEmpty(product.NotificationToCustomers))
                    {
                        var customersId = product.NotificationToCustomers.Split(',');
                        foreach (var id in customersId)
                        {
                            var deviceDetails = _deviceService.GetDevicesByCustomerId(Convert.ToInt32(id));
                            if (deviceDetails.Any())
                            {
                                var notification = new QueuedNotification()
                                {
                                    DeviceType = (DeviceType)deviceDetails[0].DeviceType,
                                    SubscriptionId = deviceDetails[0].SubscriptionId,
                                    //Subject = product.Id.ToString(),
                                    ItemId = product.Id,
                                    Message = product.Name + " is available now",
                                };
                                //_adminPanelNotificationService.SendNotication(notification);
                                _notificationService.SendNotication(notification);
                            }
                        }
                        product.NotificationToCustomers = string.Empty;
                    }

                    //product = model.ToEntity(product);

                    //product = model.ToEntity(product);

                    //product
                    //product = model.ToProductEntity();//ToProductsEntity(model);

                    product.Name = model.Name != null ? model.Name : product.Name;
                    product.Barcode = model.Barcode != null ? model.Barcode : product.Barcode;
                    product.Price = model.Price;
                    product.ShortDescription = model.ShortDescription != null ? model.ShortDescription : product.ShortDescription;
                    product.FullDescription = model.FullDescription != null ? model.ShortDescription : product.ShortDescription;
                    product.IsShipEnabled = model.IsShipEnabled;
                    product.IsFreeShipping = model.IsFreeShipping;
                    product.AdditionalShippingCharge = model.AdditionalShippingCharge;
                    product.StockQuantity = model.StockQuantity;



                    product.DeliveryDateId = model.DeliveryDateId;
                    product.VisibleIndividually = model.VisibleIndividually;

                    product.ProductTypeId = model.ProductTypeId != 0 ? model.ProductTypeId : product.ProductTypeId;
                    product.ParentGroupedProductId = model.ParentGroupedProductId != 0 ? model.ParentGroupedProductId : product.ParentGroupedProductId;
                    product.AdminComment = model.AdminComment != null ? model.AdminComment : product.AdminComment;

                    product.ProductTemplateId = model.ProductTemplateId != 0 ? model.ProductTemplateId : product.ProductTemplateId;
                    product.VendorId = model.VendorId != 0 ? model.VendorId : product.VendorId;

                    product.ShowOnHomePage = model.ShowOnHomePage;
                    product.MetaKeywords = model.MetaKeywords != null ? model.MetaKeywords : product.MetaKeywords;

                    product.MetaDescription = model.MetaDescription != null ? model.MetaDescription : product.MetaDescription;
                    product.MetaTitle = model.MetaTitle != null ? model.MetaTitle : product.MetaTitle;

                    //product.AllowCustomerReviews = model.AllowCustomerReviews;
                    //product.ApprovedRatingSum = model.ApprovedRatingSum;

                    //product.NotApprovedRatingSum = model.NotApprovedRatingSum;
                    //product.ApprovedTotalReviews = model.ApprovedTotalReviews;

                    //product.NotApprovedTotalReviews = model.NotApprovedTotalReviews;
                    //product.SubjectToAcl = model.SubjectToAcl;


                    product.UpdatedOnUtc = DateTime.UtcNow;
                    _productService.UpdateProduct(product);

                    var pictureId = 0;
                    var displayOrder = 0;

                    if (model.Files != null)
                    {
                        foreach (IFormFile photo in model.Files)
                        {
                            var contentType = photo.ContentType;
                            var vendorPictureBinary = _downloadService.GetDownloadBits(photo);
                            var picture = _pictureService.InsertPicture(vendorPictureBinary, contentType, null);

                            if (picture != null)
                                pictureId = picture.Id;
                            if (pictureId != 0)
                            {
                                _productService.InsertProductPicture(new ProductPicture
                                {
                                    PictureId = pictureId,
                                    ProductId = product.Id,
                                    DisplayOrder = displayOrder
                                });

                            }
                        }
                    }

                    //insert the new product category mapping
                    if (model.CategoryId != 0)
                    {
                        _categoryService.UpdateProductCategory(new ProductCategory
                        {
                            CategoryId = model.CategoryId,
                            ProductId = product.Id,
                            IsFeaturedProduct = false,
                            DisplayOrder = 1
                        });
                    }

                    //activity log
                    _customerActivityService.InsertActivity("EditProduct",
                        string.Format(_localizationService.GetResource("ActivityLog.EditProduct"), product.Name), product);

                }
                catch (Exception ex)
                {

                    baseResponse.StatusCode = (int)ErrorType.NotFound;
                    baseResponse.ErrorList.Add("Error in Update Product " + ex.Message);
                    return Ok(baseResponse);
                }
            }
            else
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Error in Update Product.");
                return Ok(baseResponse);
            }

            return Ok(baseResponse);
        }

        [Route("api/vendor/deleteproductfromvendor/{productId}")]
        [HttpPost]
        public virtual IActionResult DeleteProductFromVendor(string productId)
        {
            var baseResponse = new BaseResponse();

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Access Denied.");
                return Ok(baseResponse);
            }

            //try to get a product with the specified id
            var product = _productService.GetProductById(Convert.ToInt32(productId));
            if (product == null)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Product Not Found.");
                return Ok(baseResponse);
            }

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Access Denied.");
                return Ok(baseResponse);
            }

            _productService.DeleteProduct(product);

            //activity log
            _customerActivityService.InsertActivity("DeleteProduct",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteProduct"), product.Name), product);


            return Ok(baseResponse);
        }

        #region all category
        /// <summary>
        /// Get all categories, languages, currencies
        /// </summary>
        /// <returns></returns>
        [Route("api/vendor/allcategories")]
        [HttpGet]
        public IActionResult Categories()
        {
            var model = _catalogModelFactoryApi.PrepareCategoriesModel();
            int count = model.Count();
            var result = new AllCategoryResponseModel
            {
                Data = model,
                Count = count
            };
            return Ok(result);
        }

        #endregion

        [Route("api/vendor/getproductpicturelistbyid/{productId}")]
        [HttpGet]
        public IActionResult ProductPictureList(string productId)
        {
            var baseResponse = new BaseResponse();
            var intProductId = Convert.ToInt32(productId);
            ProductPictureSearchModel searchModel = new ProductPictureSearchModel();
            searchModel.ProductId = intProductId;

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Access Denied.");
                return Ok(baseResponse);
            }

            //try to get a product with the specified id
            var product = _productService.GetProductById(intProductId)
                ?? throw new ArgumentException("No product found with the specified id");
            if (product == null)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Product Not Availbale.");
                return Ok(baseResponse);
            }

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Access Denied.");
                return Ok(baseResponse);
            }

            //prepare model
            var model = PrepareProductPictureListModel(searchModel, product);
            if (model == null)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Product Image Not Availbale.");
                return Ok(baseResponse);
            }

            var result = new CommonResponseModel<ProductPictureListModel>()
            {
                Data = model
            };

            return Ok(result);

            //return Json(model);

        }

        [Route("api/vendor/updateproductpicture")]
        [HttpPost]
        public virtual IActionResult ProductPictureUpdate([FromBody]ProductPictureModel model)
        {
            var baseResponse = new BaseResponse();
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Access Denied.");
                return Ok(baseResponse);
            }

            //try to get a product picture with the specified id
            var productPicture = _productService.GetProductPictureById(model.Id);
            if (productPicture == null)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("No product picture found with the specified id.");
                return Ok(baseResponse);
            }

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productPicture.ProductId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    baseResponse.StatusCode = (int)ErrorType.NotFound;
                    baseResponse.ErrorList.Add("Access Denied.");
                    return Ok(baseResponse);
                }
            }

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(productPicture.PictureId);
            if (picture == null)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("No picture found with the specified id.");
                return Ok(baseResponse);
            }
            try
            {
                _pictureService.UpdatePicture(picture.Id,
                    _pictureService.LoadPictureBinary(picture),
                    picture.MimeType,
                    picture.SeoFilename,
                    model.OverrideAltAttribute,
                    model.OverrideTitleAttribute);

                productPicture.DisplayOrder = model.DisplayOrder;
                _productService.UpdateProductPicture(productPicture);
            }
            catch (Exception ex)
            {
                baseResponse.StatusCode = (int)ErrorType.NotFound;
                baseResponse.ErrorList.Add("Error in Product Picture Update." + ex.Message);
                return Ok(baseResponse);
            }

            return Ok(baseResponse);
        }


        protected static Product ToProductEntity(ProductsModel model)
        {
            Product entity = new Product();

            //entity.Id = model.Id;
            entity.ProductTypeId = model.ProductTypeId;
            entity.ParentGroupedProductId = model.ParentGroupedProductId;
            entity.VisibleIndividually = model.VisibleIndividually;
            entity.Name = model.Name;
            entity.ShortDescription = model.ShortDescription;

            entity.FullDescription = model.FullDescription;
            entity.AdminComment = model.AdminComment;

            entity.ProductTemplateId = model.ProductTemplateId;
            entity.VendorId = model.VendorId;

            entity.ShowOnHomePage = model.ShowOnHomePage;
            entity.MetaKeywords = model.MetaKeywords;

            entity.MetaDescription = model.MetaDescription;
            entity.MetaTitle = model.MetaTitle;

            entity.AllowCustomerReviews = model.AllowCustomerReviews;
            entity.ApprovedRatingSum = model.ApprovedRatingSum;

            entity.NotApprovedRatingSum = model.NotApprovedRatingSum;
            entity.ApprovedTotalReviews = model.ApprovedTotalReviews;

            entity.NotApprovedTotalReviews = model.NotApprovedTotalReviews;
            entity.SubjectToAcl = model.SubjectToAcl;

            entity.LimitedToStores = model.LimitedToStores;
            entity.Sku = model.Sku;

            entity.ManufacturerPartNumber = model.ManufacturerPartNumber;
            entity.Gtin = model.Gtin;

            entity.IsGiftCard = model.IsGiftCard;
            entity.GiftCardTypeId = model.GiftCardTypeId;

            entity.OverriddenGiftCardAmount = model.OverriddenGiftCardAmount;
            entity.RequireOtherProducts = model.RequireOtherProducts;

            entity.RequiredProductIds = model.RequiredProductIds;
            entity.AutomaticallyAddRequiredProducts = model.AutomaticallyAddRequiredProducts;

            entity.IsDownload = model.IsDownload;
            entity.DownloadId = model.DownloadId;

            entity.UnlimitedDownloads = model.UnlimitedDownloads;
            entity.MaxNumberOfDownloads = model.MaxNumberOfDownloads;

            entity.DownloadExpirationDays = model.DownloadExpirationDays;
            entity.DownloadActivationTypeId = model.DownloadActivationTypeId;

            entity.HasSampleDownload = model.HasSampleDownload;
            entity.SampleDownloadId = model.SampleDownloadId;

            entity.HasUserAgreement = model.HasUserAgreement;
            entity.UserAgreementText = model.UserAgreementText;

            entity.IsRecurring = model.IsRecurring;
            entity.RecurringCycleLength = model.RecurringCycleLength;

            entity.RecurringCyclePeriodId = model.RecurringCyclePeriodId;
            entity.RecurringTotalCycles = model.RecurringTotalCycles;

            entity.IsRental = model.IsRental;
            entity.RentalPriceLength = model.RentalPriceLength;

            entity.RentalPricePeriodId = model.RentalPricePeriodId;
            entity.IsShipEnabled = model.IsShipEnabled;

            entity.IsFreeShipping = model.IsFreeShipping;
            entity.ShipSeparately = model.ShipSeparately;

            entity.AdditionalShippingCharge = model.AdditionalShippingCharge;
            entity.DeliveryDateId = model.DeliveryDateId;

            entity.IsTaxExempt = model.IsTaxExempt;
            entity.TaxCategoryId = model.TaxCategoryId;

            entity.IsTelecommunicationsOrBroadcastingOrElectronicServices = model.IsTelecommunicationsOrBroadcastingOrElectronicServices;
            entity.ManageInventoryMethodId = model.ManageInventoryMethodId;

            entity.ProductAvailabilityRangeId = model.ProductAvailabilityRangeId;
            entity.UseMultipleWarehouses = model.UseMultipleWarehouses;

            entity.WarehouseId = model.WarehouseId;
            entity.StockQuantity = model.StockQuantity;

            entity.DisplayStockAvailability = model.DisplayStockAvailability;
            entity.DisplayStockQuantity = model.DisplayStockQuantity;

            entity.MinStockQuantity = model.MinStockQuantity;
            entity.LowStockActivityId = model.LowStockActivityId;

            entity.NotifyAdminForQuantityBelow = model.NotifyAdminForQuantityBelow;
            entity.BackorderModeId = model.BackorderModeId;

            entity.AllowBackInStockSubscriptions = model.AllowBackInStockSubscriptions;
            entity.OrderMinimumQuantity = model.OrderMinimumQuantity;

            entity.OrderMaximumQuantity = model.OrderMaximumQuantity;
            entity.AllowedQuantities = model.AllowedQuantities;

            entity.AllowAddingOnlyExistingAttributeCombinations = model.AllowAddingOnlyExistingAttributeCombinations;
            entity.NotReturnable = model.NotReturnable;

            entity.DisableBuyButton = model.DisableBuyButton;
            entity.DisableWishlistButton = model.DisableWishlistButton;

            entity.AvailableForPreOrder = model.AvailableForPreOrder;
            entity.PreOrderAvailabilityStartDateTimeUtc = model.PreOrderAvailabilityStartDateTimeUtc;

            entity.CallForPrice = model.CallForPrice;
            entity.Price = model.Price;

            entity.OldPrice = model.OldPrice;
            entity.ProductCost = model.ProductCost;

            entity.CustomerEntersPrice = model.CustomerEntersPrice;
            entity.MinimumCustomerEnteredPrice = model.MinimumCustomerEnteredPrice;

            entity.MaximumCustomerEnteredPrice = model.MaximumCustomerEnteredPrice;
            entity.BasepriceEnabled = model.BasepriceEnabled;

            entity.BasepriceAmount = model.BasepriceAmount;
            entity.BasepriceUnitId = model.BasepriceUnitId;

            entity.BasepriceBaseAmount = model.BasepriceBaseAmount;
            entity.MarkAsNew = model.MarkAsNew;

            entity.MarkAsNewStartDateTimeUtc = model.MarkAsNewStartDateTimeUtc;
            entity.MarkAsNewEndDateTimeUtc = model.MarkAsNewEndDateTimeUtc;

            entity.HasTierPrices = model.HasTierPrices;
            entity.HasDiscountsApplied = model.HasDiscountsApplied;

            entity.Weight = model.Weight;
            entity.Length = model.Length;

            entity.Width = model.Width;
            entity.Height = model.Height;

            entity.AvailableStartDateTimeUtc = model.AvailableStartDateTimeUtc;
            entity.AvailableEndDateTimeUtc = model.AvailableEndDateTimeUtc;

            entity.DisplayOrder = model.DisplayOrder;
            entity.Published = model.Published;

            entity.Deleted = model.Deleted;
            //entity.CreatedOnUtc = model.CreatedOnUtc;

            //entity.[UpdatedOnUtc] = model.[UpdatedOnUtc];
            entity.Barcode = model.Barcode;

            entity.RatingUrlFromGSMArena = model.RatingUrlFromGSMArena;
            entity.IsReserved = model.IsReserved;

            //entity.ReservedCustomerId = model.ReservedCustomerId;
            //entity.ReservedCustomerIds = model.ReservedCustomerIds;


            entity.ReservedQty = model.ReservedQty;
            entity.NotificationToCustomers = model.NotificationToCustomers;

            entity.NotificationToCustomers_1 = model.NotificationToCustomers_1;
            entity.CGMItemID = model.CGMItemID;

            return entity;
        }

        protected static ProductsModel ToProductModel(Product entity)
        {
            ProductsModel product = new ProductsModel();

            //entity.Id = model.Id;
            product.ProductTypeId = entity.ProductTypeId;
            product.ParentGroupedProductId = entity.ParentGroupedProductId;
            product.VisibleIndividually = entity.VisibleIndividually;
            product.Name = entity.Name;
            product.ShortDescription = entity.ShortDescription;

            product.FullDescription = entity.FullDescription;
            product.AdminComment = entity.AdminComment;

            product.ProductTemplateId = entity.ProductTemplateId;
            product.VendorId = entity.VendorId;

            product.ShowOnHomePage = entity.ShowOnHomePage;
            product.MetaKeywords = entity.MetaKeywords;

            product.MetaDescription = entity.MetaDescription;
            product.MetaTitle = entity.MetaTitle;

            product.AllowCustomerReviews = entity.AllowCustomerReviews;
            product.ApprovedRatingSum = entity.ApprovedRatingSum;

            product.NotApprovedRatingSum = entity.NotApprovedRatingSum;
            product.ApprovedTotalReviews = entity.ApprovedTotalReviews;

            product.NotApprovedTotalReviews = entity.NotApprovedTotalReviews;
            product.SubjectToAcl = entity.SubjectToAcl;

            product.LimitedToStores = entity.LimitedToStores;
            product.Sku = entity.Sku;

            product.ManufacturerPartNumber = entity.ManufacturerPartNumber;
            product.Gtin = entity.Gtin;

            product.IsGiftCard = entity.IsGiftCard;
            product.GiftCardTypeId = entity.GiftCardTypeId;

            product.OverriddenGiftCardAmount = entity.OverriddenGiftCardAmount;
            product.RequireOtherProducts = entity.RequireOtherProducts;

            product.RequiredProductIds = entity.RequiredProductIds;
            product.AutomaticallyAddRequiredProducts = entity.AutomaticallyAddRequiredProducts;

            product.IsDownload = entity.IsDownload;
            product.DownloadId = entity.DownloadId;

            product.UnlimitedDownloads = entity.UnlimitedDownloads;
            product.MaxNumberOfDownloads = entity.MaxNumberOfDownloads;

            product.DownloadExpirationDays = entity.DownloadExpirationDays;
            product.DownloadActivationTypeId = entity.DownloadActivationTypeId;

            product.HasSampleDownload = entity.HasSampleDownload;
            product.SampleDownloadId = entity.SampleDownloadId;

            product.HasUserAgreement = entity.HasUserAgreement;
            product.UserAgreementText = entity.UserAgreementText;

            product.IsRecurring = entity.IsRecurring;
            product.RecurringCycleLength = entity.RecurringCycleLength;

            product.RecurringCyclePeriodId = entity.RecurringCyclePeriodId;
            product.RecurringTotalCycles = entity.RecurringTotalCycles;

            product.IsRental = entity.IsRental;
            product.RentalPriceLength = entity.RentalPriceLength;

            product.RentalPricePeriodId = entity.RentalPricePeriodId;
            product.IsShipEnabled = entity.IsShipEnabled;

            product.IsFreeShipping = entity.IsFreeShipping;
            product.ShipSeparately = entity.ShipSeparately;

            product.AdditionalShippingCharge = entity.AdditionalShippingCharge;
            product.DeliveryDateId = entity.DeliveryDateId;

            product.IsTaxExempt = entity.IsTaxExempt;
            product.TaxCategoryId = entity.TaxCategoryId;

            product.IsTelecommunicationsOrBroadcastingOrElectronicServices = entity.IsTelecommunicationsOrBroadcastingOrElectronicServices;
            product.ManageInventoryMethodId = entity.ManageInventoryMethodId;

            product.ProductAvailabilityRangeId = entity.ProductAvailabilityRangeId;
            product.UseMultipleWarehouses = entity.UseMultipleWarehouses;

            product.WarehouseId = entity.WarehouseId;
            product.StockQuantity = entity.StockQuantity;

            product.DisplayStockAvailability = entity.DisplayStockAvailability;
            product.DisplayStockQuantity = entity.DisplayStockQuantity;

            product.MinStockQuantity = entity.MinStockQuantity;
            product.LowStockActivityId = entity.LowStockActivityId;

            product.NotifyAdminForQuantityBelow = entity.NotifyAdminForQuantityBelow;
            product.BackorderModeId = entity.BackorderModeId;

            product.AllowBackInStockSubscriptions = entity.AllowBackInStockSubscriptions;
            product.OrderMinimumQuantity = entity.OrderMinimumQuantity;

            product.OrderMaximumQuantity = entity.OrderMaximumQuantity;
            product.AllowedQuantities = entity.AllowedQuantities;

            product.AllowAddingOnlyExistingAttributeCombinations = entity.AllowAddingOnlyExistingAttributeCombinations;
            product.NotReturnable = entity.NotReturnable;

            product.DisableBuyButton = entity.DisableBuyButton;
            product.DisableWishlistButton = entity.DisableWishlistButton;

            product.AvailableForPreOrder = entity.AvailableForPreOrder;
            product.PreOrderAvailabilityStartDateTimeUtc = entity.PreOrderAvailabilityStartDateTimeUtc;

            product.CallForPrice = entity.CallForPrice;
            product.Price = entity.Price;

            product.OldPrice = entity.OldPrice;
            product.ProductCost = entity.ProductCost;

            product.CustomerEntersPrice = entity.CustomerEntersPrice;
            product.MinimumCustomerEnteredPrice = entity.MinimumCustomerEnteredPrice;

            product.MaximumCustomerEnteredPrice = entity.MaximumCustomerEnteredPrice;
            product.BasepriceEnabled = entity.BasepriceEnabled;

            product.BasepriceAmount = entity.BasepriceAmount;
            product.BasepriceUnitId = entity.BasepriceUnitId;

            product.BasepriceBaseAmount = entity.BasepriceBaseAmount;
            product.MarkAsNew = entity.MarkAsNew;

            product.MarkAsNewStartDateTimeUtc = entity.MarkAsNewStartDateTimeUtc;
            product.MarkAsNewEndDateTimeUtc = entity.MarkAsNewEndDateTimeUtc;

            product.HasTierPrices = entity.HasTierPrices;
            product.HasDiscountsApplied = entity.HasDiscountsApplied;

            product.Weight = entity.Weight;
            product.Length = entity.Length;

            product.Width = entity.Width;
            product.Height = entity.Height;

            product.AvailableStartDateTimeUtc = entity.AvailableStartDateTimeUtc;
            product.AvailableEndDateTimeUtc = entity.AvailableEndDateTimeUtc;

            product.DisplayOrder = entity.DisplayOrder;
            product.Published = entity.Published;

            product.Deleted = entity.Deleted;
            //entity.CreatedOnUtc = model.CreatedOnUtc;

            //entity.[UpdatedOnUtc] = model.[UpdatedOnUtc];
            product.Barcode = entity.Barcode;

            product.RatingUrlFromGSMArena = entity.RatingUrlFromGSMArena;
            product.IsReserved = entity.IsReserved;

            //entity.ReservedCustomerId = model.ReservedCustomerId;
            //entity.ReservedCustomerIds = model.ReservedCustomerIds;


            product.ReservedQty = entity.ReservedQty;
            product.NotificationToCustomers = entity.NotificationToCustomers;

            product.NotificationToCustomers_1 = entity.NotificationToCustomers_1;
            product.CGMItemID = entity.CGMItemID;

            product.CreatedOnUtc = entity.CreatedOnUtc;
            product.UpdatedOnUtc = entity.UpdatedOnUtc;

            return product;
        }

        public virtual SearchProductResponseModel PrepareSearchProductResponseModel(SearchModelApi model)
        {
            if (model == null)
                return null;

            var searchProductResponseModel = new SearchProductResponseModel()
            {
                Products = model.Products,
                TotalPages = model.PagingFilteringContext.TotalPages,
                PriceRange = model.PriceRange,
                //AvailableSortOptions = model.PagingFilteringContext.AvailableSortOptions,
                AvailableSortOptions = _productModelFactoryApi.GetAvailableSortOptions(),
                NotFilteredItems = model.NotFilteredItems// Added by Alexandar Rajavel on 22-Dec-2018
            };
            return searchProductResponseModel;
        }

        public virtual LogInPostResponseModel PrepareLogInPostResponseModel(LoginQueryModel model)
        {
            if (model == null)
                return null;
            var customerLoginModel = new LogInPostResponseModel();
            customerLoginModel.StatusCode = (int)ErrorType.NotOk;
            ValidationExtension.LoginValidatorForDeliveryApp(ModelState, model, _localizationService);
            if (ModelState.IsValid)
            {
                model.Username = model.Username.Trim();
                var loginResult = _customerRegistrationService.ValidateCustomer(model.Username, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerService.GetCustomerByUsername(model.Username);
                            customerLoginModel = _customerModelFactoryApi.PrepareCustomerLoginModel(customerLoginModel, customer);
                            customerLoginModel.StatusCode = (int)ErrorType.Ok;
                            break;
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist")
                        };
                        break;
                    case CustomerLoginResults.Deleted:

                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.Deleted")
                        };
                        break;
                    case CustomerLoginResults.NotActive:

                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.NotActive")
                        };
                        break;
                    case CustomerLoginResults.NotRegistered:

                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered")
                        };
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:

                        customerLoginModel.ErrorList = new List<string>
                        {
                            _localizationService.GetResource("Account.Login.WrongCredentials")
                        };
                        break;
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        customerLoginModel.ErrorList.Add(error.ErrorMessage);
                    }
                }
            }
            return customerLoginModel;
        }

        /// <summary>
        /// Prepare paged product picture list model
        /// </summary>
        /// <param name="searchModel">Product picture search model</param>
        /// <param name="product">Product</param>
        /// <returns>Product picture list model</returns>
        public virtual ProductPictureListModel PrepareProductPictureListModel(ProductPictureSearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get product pictures
            var productPictures = _productService.GetProductPicturesByProductId(product.Id);

            //prepare grid model
            var model = new ProductPictureListModel
            {
                Data = productPictures.PaginationByRequestModel(searchModel).Select(productPicture =>
                {
                    //fill in model values from the entity
                    var productPictureModel = new ProductPictureModel
                    {
                        Id = productPicture.Id,
                        ProductId = productPicture.ProductId,
                        PictureId = productPicture.PictureId,
                        DisplayOrder = productPicture.DisplayOrder
                    };

                    //fill in additional values (not existing in the entity)
                    var picture = _pictureService.GetPictureById(productPicture.PictureId)
                        ?? throw new Exception("Picture cannot be loaded");

                    productPictureModel.PictureUrl = _pictureService.GetPictureUrl(picture);
                    productPictureModel.OverrideAltAttribute = picture.AltAttribute;
                    productPictureModel.OverrideTitleAttribute = picture.TitleAttribute;

                    return productPictureModel;
                }),
                Total = productPictures.Count
            };

            return model;
        }

        #endregion
    }
}

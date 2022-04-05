using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using BS.Plugin.NopStation.MobileWebApi.Models.PickUp;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel.CustomerModel;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    /// <summary>
    /// Represents the PickUpOrder Model factory
    /// </summary>
    public partial class PickUpOrderModelFactoryApi : IPickUpOrderModelFactoryApi
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IStoreService _storeService;
        private readonly CurrencySettings _currencySettings;
        private readonly IVendorService _vendorService;
        private readonly IDownloadService _downloadService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IGiftCardService _giftCardService;

        #endregion

        #region Constructors

        public PickUpOrderModelFactoryApi(IStoreContext storeContext, IOrderService orderService, IWorkContext workContext, IDateTimeHelper dateTimeHelper, ILocalizationService localizationService, IOrderProcessingService orderProcessingService, IProductService productService, IUrlRecordService urlRecordService,
            IPictureService pictureService, MediaSettings mediaSettings, ICurrencyService currencyService, IStoreService storeService, IPriceFormatter priceFormatter, CurrencySettings currencySettings, IVendorService vendorService, IDownloadService downloadService, IReturnRequestService returnRequestService, IGiftCardService giftCardService)
        {
            this._storeContext = storeContext;
            this._orderService = orderService;
            this._workContext = workContext;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._orderProcessingService = orderProcessingService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._storeService = storeService;
            this._currencySettings = currencySettings;
            this._vendorService = vendorService;
            this._downloadService = downloadService;
            this._returnRequestService = returnRequestService;
            this._giftCardService = giftCardService;
        }

        #endregion

        #region Utilities
        protected virtual PictureModel PrepareCartItemPictureModel(OrderItem sci,
             int pictureSize, bool showDefaultPicture, string productName)
        {
            string imageUrl;
            try
            {
                var sciPicture = _pictureService.GetProductPicture(sci.Product, sci.AttributesXml); //sci.Product.GetProductPicture(sci.AttributesXml, _pictureService, _productAttributeParser);
                imageUrl = _pictureService.GetPictureUrl(sciPicture, pictureSize, showDefaultPicture);

            }
            //shopping cart item picture
            catch (Exception)
            {
                imageUrl = _pictureService.GetDefaultPictureUrl(_mediaSettings.CartThumbPictureSize);
            }

            return new PictureModel
            {
                ImageUrl = imageUrl
            };

        }
        #endregion

        #region Methods

        /// <summary>
        /// Prepare the PickUp Order List model
        /// </summary>
        /// <returns>PickUp Order List model</returns>
        public virtual PickUpOrderListModel PrepareCustomerOrderListModel(PickUpOrderSearchModelApi searchModel, int pageNumber, int pageSize)
        {
            //added code from web
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //Hardcode Incomplete Order Status Ids
            List<int> ordersStatusIds = new List<int>();
            ordersStatusIds.Add(10);
            ordersStatusIds.Add(20);

            //get parameters to filter orders
            var orderStatusIds = ordersStatusIds;//(searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
            var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();
            var shippingStatusIds = (searchModel.ShippingStatusIds?.Contains(0) ?? true) ? null : searchModel.ShippingStatusIds.ToList();
            if (_workContext.CurrentVendor != null)
                searchModel.VendorId = _workContext.CurrentVendor.Id;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var product = _productService.GetProductById(searchModel.ProductId);
            var filterByProductId = product != null && (_workContext.CurrentVendor == null || product.VendorId == _workContext.CurrentVendor.Id)
                ? searchModel.ProductId : 0;
            pageSize = pageSize == 0 ? 10 : pageSize;

            //get orders
            var orders = _orderService.SearchOrdersForPickUp1(storeId: searchModel.StoreId,
                vendorId: searchModel.VendorId,
                productId: filterByProductId,
                warehouseId: searchModel.WarehouseId,
                paymentMethodSystemName: searchModel.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingPhone: searchModel.BillingPhone,
                billingEmail: searchModel.BillingEmail,
                billingLastName: searchModel.BillingLastName,
                billingCountryId: searchModel.BillingCountryId,
                orderNotes: searchModel.OrderNotes,
                //Added code By Sunil Kumar at 24-04-2020 for DeviceId filter
                deviceId: searchModel.DeviceId,
                editDateTime:searchModel.EditDateTime,
                editEndDate:searchModel.EditEndDate,
                customOrderNumber: searchModel.CustomOrderNumber, pageIndex: pageNumber - 1, pageSize: pageSize
               );
            //added code from web

         
            //prepare list model
            var model = new PickUpOrderListModel
            {
                //fill in model values from the entity
                Data = orders.Select(order =>
                {
                    //fill in model values from the entity
                    var orderModel = new PickUpOrderModel
                    {
                        Id = order.Id,
                        OrderStatusId = order.OrderStatusId,
                        PaymentStatusId = order.PaymentStatusId,
                        ShippingStatusId = order.ShippingStatusId,
                        CustomerEmail = order.BillingAddress.Email,
                        CustomerFullName = $"{order.BillingAddress.FirstName} {order.BillingAddress.LastName}",
                        CustomOrderNumber = order.CustomOrderNumber,
                        CustomerIp = order.CustomerIp,
                        AllowStoringCreditCardNumber = order.AllowStoringCreditCardNumber,
                        DeviceId = order.DeviceId
                    };

                    //prepare order items
                    PrepareOrderItemModels(orderModel.Items, order);

                    //convert dates to the user time
                    orderModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);

                    //prepare order items
                    // orderModel.CreatedOn = PrepareOrderItemModels(model.Items, order);

                    //fill in additional values (not existing in the entity)
                    orderModel.StoreName = _storeService.GetStoreById(order.StoreId)?.Name ?? "Deleted";
                    orderModel.OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus);
                    orderModel.PaymentStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus);
                    orderModel.ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus);
                    orderModel.OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal, true, false);
                    return orderModel;
                }).OrderByDescending(x=>x.CreatedOn),
                Total = orders.TotalCount
            };

            return model;
        }


        /// <summary>
        /// Prepare order item models
        /// </summary>
        /// <param name="models">List of order item models</param>
        /// <param name="order">Order</param>
        protected virtual void PrepareOrderItemModels(IList<OrderItemModel> models, Order order)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

            //get order items
            var orderItems = order.OrderItems;
            if (_workContext.CurrentVendor != null)
                orderItems = orderItems.Where(orderItem => orderItem.Product.VendorId == _workContext.CurrentVendor.Id).ToList();

            foreach (var orderItem in orderItems)
            {
                //fill in model values from the entity
                var orderItemModel = new OrderItemModel
                {
                    Id = orderItem.Id,
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.Product.Name,
                    Quantity = orderItem.Quantity,
                    IsDownload = orderItem.Product.IsDownload,
                    DownloadCount = orderItem.DownloadCount,
                    DownloadActivationType = orderItem.Product.DownloadActivationType,
                    IsDownloadActivated = orderItem.IsDownloadActivated,
                    UnitPriceInclTaxValue = orderItem.UnitPriceInclTax,
                    UnitPriceExclTaxValue = orderItem.UnitPriceExclTax,
                    DiscountInclTaxValue = orderItem.DiscountAmountInclTax,
                    DiscountExclTaxValue = orderItem.DiscountAmountExclTax,
                    SubTotalInclTaxValue = orderItem.PriceInclTax,
                    SubTotalExclTaxValue = orderItem.PriceExclTax,
                    AttributeInfo = orderItem.AttributeDescription,
                    CGMItemID = orderItem.Product.CGMItemID,
                    Barcode = orderItem.Product.Barcode
                };

                //fill in additional values (not existing in the entity)
                orderItemModel.Sku = _productService.FormatSku(orderItem.Product, orderItem.AttributesXml);
                orderItemModel.VendorName = _vendorService.GetVendorById(orderItem.Product.VendorId)?.Name;

                //picture
                var orderItemPicture = _pictureService.GetProductPicture(orderItem.Product, orderItem.AttributesXml);
                orderItemModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(orderItemPicture, 75);

                //license file
                if (orderItem.LicenseDownloadId.HasValue)
                {
                    orderItemModel.LicenseDownloadGuid = _downloadService
                        .GetDownloadById(orderItem.LicenseDownloadId.Value)?.DownloadGuid ?? Guid.Empty;
                }

                //unit price
                orderItemModel.UnitPriceInclTax = _priceFormatter
                    .FormatPrice(orderItem.UnitPriceInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                orderItemModel.UnitPriceExclTax = _priceFormatter
                    .FormatPrice(orderItem.UnitPriceExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);

                //discounts
                orderItemModel.DiscountInclTax = _priceFormatter.FormatPrice(orderItem.DiscountAmountInclTax, true,
                    primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                orderItemModel.DiscountExclTax = _priceFormatter.FormatPrice(orderItem.DiscountAmountExclTax, true,
                    primaryStoreCurrency, _workContext.WorkingLanguage, false, true);

                //subtotal
                orderItemModel.SubTotalInclTax = _priceFormatter.FormatPrice(orderItem.PriceInclTax, true, primaryStoreCurrency,
                    _workContext.WorkingLanguage, true, true);
                orderItemModel.SubTotalExclTax = _priceFormatter.FormatPrice(orderItem.PriceExclTax, true, primaryStoreCurrency,
                    _workContext.WorkingLanguage, false, true);

                //recurring info
                if (orderItem.Product.IsRecurring)
                {
                    orderItemModel.RecurringInfo = string.Format(_localizationService.GetResource("Admin.Orders.Products.RecurringPeriod"),
                        orderItem.Product.RecurringCycleLength, _localizationService.GetLocalizedEnum(orderItem.Product.RecurringCyclePeriod));
                }

                //rental info
                if (orderItem.Product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalStartDateUtc.Value) : string.Empty;
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalEndDateUtc.Value) : string.Empty;
                    orderItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //prepare return request models
                PrepareReturnRequestBriefModels(orderItemModel.ReturnRequests, orderItem);

                //gift card identifiers
                orderItemModel.PurchasedGiftCardIds = _giftCardService
                    .GetGiftCardsByPurchasedWithOrderItemId(orderItem.Id).Select(card => card.Id).ToList();

                models.Add(orderItemModel);
            }
        }


        /// <summary>
        /// Prepare return request brief models
        /// </summary>
        /// <param name="models">List of return request brief models</param>
        /// <param name="orderItem">Order item</param>
        protected virtual void PrepareReturnRequestBriefModels(IList<OrderItemModel.ReturnRequestBriefModel> models, OrderItem orderItem)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var returnRequests = _returnRequestService.SearchReturnRequests(orderItemId: orderItem.Id);
            foreach (var returnRequest in returnRequests)
            {
                models.Add(new OrderItemModel.ReturnRequestBriefModel
                {
                    CustomNumber = returnRequest.CustomNumber,
                    Id = returnRequest.Id
                });
            }
        }



        #endregion



    }
}

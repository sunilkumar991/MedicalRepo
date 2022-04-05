using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Order;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Seo;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using Nop.Services.Discounts;
using Nop.Services.Vendors;
using Nop.Services.Common;
using Nop.Services.Tax;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    /// <summary>
    /// Represents the order model factory
    /// </summary>
    public partial class OrderModelFactoryApi : IOrderModelFactoryApi
    {
        #region Fields

        private readonly IAddressModelFactoryApi _addressModelFactoryApi;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;
        private readonly ILocalizationService _localizationService;
        private readonly IShippingService _shippingService;
        private readonly ICountryService _countryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IPictureService _pictureService;

        private readonly OrderSettings _orderSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IDiscountService _discountService;
        private readonly ICategoryService _categoryService;
        private readonly IVendorService _vendorService;
        private readonly IAddressService _addressService;
        //private readonly IProductModelFactoryApi _productModelFactoryApi;
        private readonly ITaxService _taxService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ISpecificationAttributeService _specificationAttributeService;

        #endregion

        #region Constructors

        public OrderModelFactoryApi(IAddressModelFactoryApi addressModelFactoryApi,
            IOrderService orderService,
            IWorkContext workContext,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            IOrderProcessingService orderProcessingService,
            IDateTimeHelper dateTimeHelper,
            IPaymentService paymentService,
            ILocalizationService localizationService,
            IShippingService shippingService,
            ICountryService countryService,
            IProductAttributeParser productAttributeParser,
            IDownloadService downloadService,
            IStoreContext storeContext,
            IOrderTotalCalculationService orderTotalCalculationService,
            IRewardPointService rewardPointService,
            IPictureService pictureService,
            CatalogSettings catalogSettings,
            OrderSettings orderSettings,
            TaxSettings taxSettings,
            ShippingSettings shippingSettings,
            AddressSettings addressSettings,
            RewardPointsSettings rewardPointsSettings,
            PdfSettings pdfSettings,
            MediaSettings mediaSettings,
            IProductService productService,
            IUrlRecordService urlRecordService,
            IDiscountService discountService,
            ICategoryService categoryService,
            IVendorService vendorService,
            IAddressService addressService,
            ITaxService taxService,
            IPriceCalculationService priceCalculationService,
            ISpecificationAttributeService specificationAttributeService)
        {
            this._addressModelFactoryApi = addressModelFactoryApi;
            this._orderService = orderService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._dateTimeHelper = dateTimeHelper;
            this._paymentService = paymentService;
            this._localizationService = localizationService;
            this._shippingService = shippingService;
            this._countryService = countryService;
            this._productAttributeParser = productAttributeParser;
            this._downloadService = downloadService;
            this._storeContext = storeContext;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._rewardPointService = rewardPointService;
            this._pictureService = pictureService;

            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._taxSettings = taxSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._pdfSettings = pdfSettings;
            this._mediaSettings = mediaSettings;
            _productService = productService;
            _urlRecordService = urlRecordService;
            this._discountService = discountService;
            _specificationAttributeService = specificationAttributeService;
            _categoryService = categoryService;
            _vendorService = vendorService;
            _addressService = addressService;
            _taxService = taxService;
            _priceCalculationService = priceCalculationService;
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
        /// Prepare the customer order list model
        /// </summary>
        /// <returns>Customer order list model</returns>
        public virtual CustomerOrderListResponseModel PrepareCustomerOrderListModel()
        {
            var model = new CustomerOrderListResponseModel();
            var orders = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id);

            foreach (var order in orders)
            {
                try
                {
                    var orderModel = new CustomerOrderListResponseModel.OrderDetailsModel
                    {
                        Id = order.Id,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                        OrderStatusEnum = order.OrderStatus,
                        OrderDeliveryCode = order.DeliveryCode,
                        OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus),
                        PaymentStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus),
                        ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus),
                        IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order),
                        CustomOrderNumber = order.CustomOrderNumber,
                        ExpectedDeliveryDate = _dateTimeHelper.ConvertDateAsText(order.ExpectedDeliveryDate.Value)//Added By Ankur on 20th October

                    };
                    //Added code By Sunil Kumar at 12-11-18 added order Items collection 
                    List<CustomerOrderListResponseModel.OrderItemModel> orderItems = new List<CustomerOrderListResponseModel.OrderItemModel>();
                    foreach (var orderitem in order.OrderItems)
                    {
                        var orderitemmodel = new CustomerOrderListResponseModel.OrderItemModel
                        {
                            Id = orderitem.Id,
                            OrderItemGuid = orderitem.OrderItemGuid,
                            Sku = _productService.FormatSku(orderitem.Product, orderitem.AttributesXml),
                            ProductId = orderitem.Product.Id,
                            ProductName = _localizationService.GetLocalized(orderitem.Product, x => x.Name),
                            ProductSeName = _urlRecordService.GetSeName(orderitem.Product),
                            Quantity = orderitem.Quantity,
                            AttributeInfo = orderitem.AttributeDescription,
                            Picture = PrepareCartItemPictureModel(orderitem,
                                _mediaSettings.CartThumbPictureSize, true, _localizationService.GetLocalized(orderitem.Product, x => x.Name))
                        };
                        //unit price, subtotal
                        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                        {
                            //including tax
                            var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderitem.UnitPriceInclTax, order.CurrencyRate);
                            orderitemmodel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);

                            var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderitem.PriceInclTax, order.CurrencyRate);
                            orderitemmodel.SubTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                        }
                        else
                        {
                            //excluding tax
                            var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderitem.UnitPriceExclTax, order.CurrencyRate);
                            orderitemmodel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);

                            var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderitem.PriceExclTax, order.CurrencyRate);
                            orderitemmodel.SubTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                        }
                        orderItems.Add(orderitemmodel);
                    }

                    orderModel.Items = orderItems;
                    var orderpaymentfee = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                    orderModel.PaymentMethodFee = _priceFormatter.FormatPrice(orderpaymentfee, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
                    var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                    orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
                    model.Orders.Add(orderModel);


                }
                catch (Exception)
                {

                }
            }

            var recurringPayments = _orderService.SearchRecurringPayments(_storeContext.CurrentStore.Id,
                _workContext.CurrentCustomer.Id);
            foreach (var recurringPayment in recurringPayments)
            {
                var recurringPaymentModel = new CustomerOrderListResponseModel.RecurringOrderModel
                {
                    Id = recurringPayment.Id,
                    StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(),
                    CycleInfo = $"{recurringPayment.CycleLength} {_localizationService.GetLocalizedEnum(recurringPayment.CyclePeriod)}",
                    NextPayment = recurringPayment.NextPaymentDate.HasValue ? _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString() : "",
                    TotalCycles = recurringPayment.TotalCycles,
                    CyclesRemaining = recurringPayment.CyclesRemaining,
                    InitialOrderId = recurringPayment.InitialOrder.Id,
                    InitialOrderNumber = recurringPayment.InitialOrder.CustomOrderNumber,
                    CanCancel = _orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment),
                    CanRetryLastPayment = _orderProcessingService.CanRetryLastRecurringPayment(_workContext.CurrentCustomer, recurringPayment)
                };

                model.RecurringOrders.Add(recurringPaymentModel);
            }

            return model;
        }



        /// <summary>
        /// Prepare the order details model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Order details model</returns>
        //public virtual OrderDetailsResponseModel PrepareOrderDetailsModel(IList<Order> order)
        //{
        //    if (order == null)
        //        throw new ArgumentNullException("order");
        //    var model = new OrderDetailsResponseModel();

        //    var model3 = new OrderDetailsResponseModel.OrderItemModel();
        //    foreach (var orders in order)
        //    {

        //        model.CustomOrderNumber = orders.CustomOrderNumber;
        //        model.Items.Add(model3);
        //        var addressModel = new AddressModel
        //        {

        //            Address1 = orders.BillingAddress.Address1,
        //            Address2 = orders.BillingAddress.Address2
        //        };
        //        model.BillingAddress = addressModel;
        //    }

        //    return model;


        //}


        public virtual IList<OderDetails> PrepareOrderDetailsModel(IList<Order> order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            IList<OderDetails> model = new List<OderDetails>();


            foreach (var orders in order)
            {
                var oderDetailsmodel = new OderDetails();
                oderDetailsmodel.OrderId = orders.CustomOrderNumber;
                var addressModel = new AddressModel
                {
                    FirstName = orders.BillingAddress.FirstName,
                    LastName = orders.BillingAddress.LastName,
                    Email = orders.BillingAddress.Email,

                    CountryId = orders.BillingAddress.CountryId,
                    Company = orders.BillingAddress.Company,
                    StateProvinceId = orders.BillingAddress.StateProvinceId,
                    ZipPostalCode = orders.BillingAddress.ZipPostalCode,

                    PhoneNumber = orders.BillingAddress.PhoneNumber,
                    StateProvinceName = Convert.ToString(orders.BillingAddress.StateProvince.Name),
                    CountryName = Convert.ToString(orders.BillingAddress.Country.Name),
                    FaxNumber = orders.BillingAddress.FaxNumber,
                    Address1 = orders.BillingAddress.Address1,
                    Address2 = orders.BillingAddress.Address2
                };
                if ((orders.BillingAddress.City) != null)
                {
                    addressModel.City = Convert.ToString(orders.BillingAddress.City.Name);
                }
                else
                {
                    addressModel.City = null;

                }
                oderDetailsmodel.OrderAddress = addressModel;
                model.Add(oderDetailsmodel);
            }
            return model;


        }

        /// <summary>
        /// Prepare the order details model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Order details model</returns>
        public virtual OrderDetailsResponseModel PrepareOrderDetailsModel(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            var model = new OrderDetailsResponseModel();

            model.Id = order.Id;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);
            model.OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus);
            model.IsReOrderAllowed = _orderSettings.IsReOrderAllowed;
            model.IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order);
            model.PdfInvoiceDisabled = _pdfSettings.DisablePdfInvoicesForPendingOrders && order.OrderStatus == OrderStatus.Pending;
            model.CustomOrderNumber = order.CustomOrderNumber;
            model.ExpectedDeliveryDate = order.ExpectedDeliveryDate != null ? _dateTimeHelper.ConvertDateAsText(order.ExpectedDeliveryDate.Value) : ""; //Added By Ankur
            model.PickUpInStore = order.PickUpInStore;
            //added by Sunil Kumar at 5/1/19
            model.ReferenceNumber = order.AuthorizationTransactionId;

            //shipping info
            model.ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus);
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                model.IsShippable = true;
                model.PickUpInStore = order.PickUpInStore;

                if (!order.PickUpInStore)
                {
                    _addressModelFactoryApi.PrepareAddressModel(model.ShippingAddress,
                        address: order.ShippingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings);
                }
                else
                {
                    if (order.PickupAddress != null)
                    {
                        _addressModelFactoryApi.PrepareAddressModel(model.PickupAddress,
                        address: order.PickupAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings,
                        loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id)
                        );
                        //model.PickupAddressGoogleMapsUrl = string.Format("http://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q={0}",
                        //    Server.UrlEncode(string.Format("{0} {1} {2} {3}", order.PickupAddress.Address1, order.PickupAddress.ZipPostalCode, order.PickupAddress.City,
                        //        order.PickupAddress.Country != null ? order.PickupAddress.Country.Name : string.Empty)));
                    }
                }

                model.ShippingMethod = order.ShippingMethod;

                //shipments (only already shipped)
                var shipments = order.Shipments.Where(x => x.ShippedDateUtc.HasValue).OrderBy(x => x.CreatedOnUtc).ToList();
                foreach (var shipment in shipments)
                {
                    var shipmentModel = new OrderDetailsResponseModel.ShipmentBriefModel
                    {

                        Id = shipment.Id,
                        TrackingNumber = shipment.TrackingNumber,
                    };
                    if (shipment.ShippedDateUtc.HasValue)
                        shipmentModel.ShippedDate = _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
                    if (shipment.DeliveryDateUtc.HasValue)
                        shipmentModel.DeliveryDate = _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);
                    model.Shipments.Add(shipmentModel);
                }
            }
            if (!order.PickUpInStore)
            {
                model.IsShippable = true;
                _addressModelFactoryApi.PrepareAddressModel(model.ShippingAddress,
                    address: order.BillingAddress,
                    excludeProperties: false,
                    addressSettings: _addressSettings);
            }
            else
            {
                if (order.PickupAddress != null)
                {
                    _addressModelFactoryApi.PrepareAddressModel(model.PickupAddress,
                    address: order.PickupAddress,
                    excludeProperties: false,
                    addressSettings: _addressSettings
                    //loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id)
                    );
                }
            }

            //billing info
            _addressModelFactoryApi.PrepareAddressModel(model.BillingAddress,
                address: order.BillingAddress,
                excludeProperties: false,
                addressSettings: _addressSettings);

            //VAT number
            model.VatNumber = order.VatNumber;

            //payment method
            var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
            var paymentsubmethodname = order.PaymentMethodSubType!=null ? order.PaymentMethodSubType : "";
            model.PaymentMethod = paymentMethod != null ? _localizationService.GetLocalizedFriendlyName(paymentMethod, _workContext.WorkingLanguage.Id) : order.PaymentMethodSystemName;
            model.PaymentMethodSubType = paymentsubmethodname;
            model.PaymentMethodStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus);
            model.CanRePostProcessPayment = _paymentService.CanRePostProcessPayment(order);
            //custom values
            model.CustomValues = _paymentService.DeserializeCustomValues(order);

            //order subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax

                //order subtotal
                var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //order subtotal
                var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax

                //order shipping
                var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //payment method additional fee
                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeInclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //order shipping
                var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                //payment method additional fee
                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeExclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            //tax
            bool displayTax = true;
            bool displayTaxRates = true;
            if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    var taxRates = _orderService.ParseTaxRates(order, order.TaxRates);
                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    //TODO pass languageId to _priceFormatter.FormatPrice
                    model.Tax = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                    foreach (var tr in taxRates)
                    {
                        model.TaxRates.Add(new OrderDetailsResponseModel.TaxRate
                        {
                            Rate = _priceFormatter.FormatTaxRate(tr.Key),
                            //TODO pass languageId to _priceFormatter.FormatPrice
                            Value = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(tr.Value, order.CurrencyRate), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                        });
                    }
                }
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoOrderDetailsPage;
            model.PricesIncludeTax = order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax;

            //discount (applied to order total)
            var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
            if (orderDiscountInCustomerCurrency > decimal.Zero)
                model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);


            //gift cards
            foreach (var gcuh in order.GiftCardUsageHistory)
            {
                model.GiftCards.Add(new OrderDetailsResponseModel.GiftCard
                {
                    CouponCode = gcuh.GiftCard.GiftCardCouponCode,
                    Amount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                });
            }

            //reward points           
            if (order.RedeemedRewardPointsEntry != null)
            {
                model.RedeemedRewardPoints = -order.RedeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(order.RedeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
            }

            //total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            model.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

            //checkout attributes
            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;

            //order notes
            foreach (var orderNote in order.OrderNotes
                .Where(on => on.DisplayToCustomer)
                .OrderByDescending(on => on.CreatedOnUtc)
                .ToList())
            {
                model.OrderNotes.Add(new OrderDetailsResponseModel.OrderNote
                {
                    Id = orderNote.Id,
                    HasDownload = orderNote.DownloadId > 0,
                    Note = _orderService.FormatOrderNoteText(orderNote),
                    //CreatedOn = _dateTimeHelper.ConvertDateAsText(orderNote.CreatedOnUtc)
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }


            //purchased products
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            var orderItems = order.OrderItems;
            foreach (var orderItem in orderItems)
            {
                var orderItemModel = new OrderDetailsResponseModel.OrderItemModel
                {
                    Id = orderItem.Id,
                    OrderItemGuid = orderItem.OrderItemGuid,
                    Sku = _productService.FormatSku(orderItem.Product, orderItem.AttributesXml),
                    ProductId = orderItem.Product.Id,
                    ProductName = _localizationService.GetLocalized(orderItem.Product, x => x.Name),
                    ProductSeName = _urlRecordService.GetSeName(orderItem.Product),
                    Quantity = orderItem.Quantity,
                    AttributeInfo = orderItem.AttributeDescription,
                    Picture = PrepareCartItemPictureModel(orderItem,
                        _mediaSettings.CartThumbPictureSize, true, _localizationService.GetLocalized(orderItem.Product, x => x.Name))
                };
                //rental info
                if (orderItem.Product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalEndDateUtc.Value) : "";
                    orderItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }
                ProductOverViewModelApi.ProductReviewOverviewModel productReview = new ProductOverViewModelApi.ProductReviewOverviewModel
                {
                    ProductId = orderItem.Product.Id,
                    //RatingSum = product.ApprovedRatingSum,
                    TotalReviews = orderItem.Product.ApprovedTotalReviews,
                    AllowCustomerReviews = orderItem.Product.AllowCustomerReviews
                };
                //Added By Sunil Kumar at 28-11-18
                //Review Details
                if (orderItem.Product.ApprovedTotalReviews != 0)
                {
                    double RatingSumval = (double)orderItem.Product.ApprovedRatingSum / orderItem.Product.ApprovedTotalReviews;
                    productReview.RatingSum = Math.Round(RatingSumval, 1);
                    productReview.TotalReviews = orderItem.Product.ApprovedTotalReviews;
                }
                else
                {
                    productReview.RatingSum = orderItem.Product.ApprovedRatingSum;
                }

                orderItemModel.ProductReviewOverview = productReview;

                //Added by Alexandar Rajavel on 17-Mar-2019
                var userRating = orderItem.Product.ProductReviews.FirstOrDefault(u => u.CustomerId == _workContext.CurrentCustomer.Id);
                if (userRating != null)
                {
                    orderItemModel.UserRating.Comments = userRating.ReviewText;
                    orderItemModel.UserRating.Rating = userRating.Rating;
                }
                //Added By Sunil Kumar at 28-11-18
                orderItemModel.DiscountOfferName = string.Empty;
                //Discount Percentage
                if (orderItem.Product.HasDiscountsApplied)
                {
                    var percentage = orderItem.Product.DiscountProductMappings.Select(x => new { x.Discount.DiscountPercentage, x.Discount.Name, x.Discount.DiscountAmount }).ToList();
                    if (percentage.Count > 0)
                    {
                        //Added by Sunil Kumar at 28-02-19
                        var discount = _discountService.GetDiscountPercentageAndName(orderItem.Product);
                        orderItemModel.DiscountPercentage = discount.Item1;
                        orderItemModel.DiscountOfferName = discount.Item2;
                        orderItemModel.DiscountAmount = Convert.ToString(percentage[0].DiscountAmount);
                        decimal discountAmt = 0;
                        //decimal discountAmount = 0;
                        if (orderItemModel.DiscountAmount == "0.0000")
                        {
                            discountAmt = (orderItem.Product.Price * orderItemModel.DiscountPercentage) / 100;

                            //added code By Sunil Kumar at  10-04-2020 For 10% Discount start

                      //Commented by Sunil Kumar on 30122020 for disable Disocunt on VISA
                            //if (order.PaymentMethodSystemName != string.Empty && order.PaymentMethodSubType=="VISA")
                            //{
                            //    discountAmount = 0;
                            //    //if ordertotal is >20000 and its discount(10%)>3000 = 3000  else  10% discount only
                            //    if (order.OrderSubtotalInclTax > 20000)
                            //    {
                            //        var appliedDiscount = (order.OrderSubtotalInclTax * (decimal)0.1) > 3000 ? 3000 : (order.OrderSubtotalInclTax * (decimal)0.1);

                            //        discountAmount = appliedDiscount;
                            //    }
                            //    discountAmt = discountAmount;
                            //}
                            //added code By Sunil Kumar at  10-04-2020 For 10% Discount start End


                            //Ended by sunil kumar commenting
                            orderItemModel.DiscountAmount = _priceFormatter.FormatPrice(discountAmt, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
                        }
                        orderItemModel.PriceAfterDiscount = _priceFormatter.FormatPrice(orderItem.Product.Price - discountAmt, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
                    }
                    //Added by Alexandar Rajavel on 09-Jan-2019
                    orderItemModel.ActualPrice = _priceFormatter.FormatPrice(orderItem.Product.Price, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
                }
                //Added by Sunil Kumar on 23-Jan-2019
                orderItemModel.OldPrice = _priceFormatter.FormatPrice(orderItem.Product.OldPrice, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
                model.Items.Add(orderItemModel);
                //unit price, subtotal
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);

                    var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                    orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);

                    var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                    orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                }

                //downloadable products
                if (_downloadService.IsDownloadAllowed(orderItem))
                    orderItemModel.DownloadId = orderItem.Product.DownloadId;
                if (_downloadService.IsLicenseDownloadAllowed(orderItem))
                    orderItemModel.LicenseId = orderItem.LicenseDownloadId.HasValue ? orderItem.LicenseDownloadId.Value : 0;
            }

            return model;
        }



        /// <summary>
        /// Prepare the shipment details model
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>Shipment details model</returns>
        public virtual ShipmentDetailsResponseModel PrepareShipmentDetailsModel(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            var order = shipment.Order;
            if (order == null)
                throw new Exception("order cannot be loaded");
            var model = new ShipmentDetailsResponseModel();

            model.Id = shipment.Id;
            if (shipment.ShippedDateUtc.HasValue)
                model.ShippedDate = _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
            if (shipment.DeliveryDateUtc.HasValue)
                model.DeliveryDate = _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);

            //tracking number and shipment information
            if (!String.IsNullOrEmpty(shipment.TrackingNumber))
            {
                model.TrackingNumber = shipment.TrackingNumber;
                var srcm = _shippingService.LoadShippingRateComputationMethodBySystemName(order.ShippingRateComputationMethodSystemName);
                if (srcm != null &&
                    srcm.PluginDescriptor.Installed &&
                   _shippingService.IsShippingRateComputationMethodActive(srcm))
                {
                    var shipmentTracker = srcm.ShipmentTracker;
                    if (shipmentTracker != null)
                    {
                        model.TrackingNumberUrl = shipmentTracker.GetUrl(shipment.TrackingNumber);
                        if (_shippingSettings.DisplayShipmentEventsToCustomers)
                        {
                            var shipmentEvents = shipmentTracker.GetShipmentEvents(shipment.TrackingNumber);
                            if (shipmentEvents != null)
                            {
                                foreach (var shipmentEvent in shipmentEvents)
                                {
                                    var shipmentStatusEventModel = new ShipmentDetailsResponseModel.ShipmentStatusEventModel();
                                    var shipmentEventCountry = _countryService.GetCountryByTwoLetterIsoCode(shipmentEvent.CountryCode);
                                    shipmentStatusEventModel.Country = shipmentEventCountry != null
                                        ? _localizationService.GetLocalized(shipmentEventCountry, x => x.Name) : shipmentEvent.CountryCode;
                                    shipmentStatusEventModel.Date = shipmentEvent.Date;
                                    shipmentStatusEventModel.EventName = shipmentEvent.EventName;
                                    shipmentStatusEventModel.Location = shipmentEvent.Location;
                                    model.ShipmentStatusEvents.Add(shipmentStatusEventModel);
                                }
                            }
                        }
                    }
                }
            }

            //products in this shipment
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            foreach (var shipmentItem in shipment.ShipmentItems)
            {
                var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                if (orderItem == null)
                    continue;

                var shipmentItemModel = new ShipmentDetailsResponseModel.ShipmentItemModel
                {
                    Id = shipmentItem.Id,
                    Sku = _productService.FormatSku(orderItem.Product, orderItem.AttributesXml),
                    ProductId = orderItem.Product.Id,
                    ProductName = _localizationService.GetLocalized(orderItem.Product, x => x.Name),
                    ProductSeName = _urlRecordService.GetSeName(orderItem.Product),
                    AttributeInfo = orderItem.AttributeDescription,
                    QuantityOrdered = orderItem.Quantity,
                    QuantityShipped = shipmentItem.Quantity,
                };
                //rental info
                if (orderItem.Product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalEndDateUtc.Value) : "";
                    shipmentItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }
                model.Items.Add(shipmentItemModel);
            }

            //order details model
            model.Order = PrepareOrderDetailsModel(order);

            return model;
        }

        public OrderDetailsForWHM PrepareOrderDetailsModelForWHM(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var model = new OrderDetailsForWHM();
            model.OrderId = order.Id;
            model.CustomOrderNumber = order.CustomOrderNumber;
            var orderItems = order.OrderItems;
            decimal taxRate;
            var priceWtihDiscount = string.Empty;
            foreach (var orderItem in orderItems)
            {
                var category = _categoryService.GetProductCategoriesByProductId(orderItem.Product.Id).ToList();
                var pickupAddress = new PickupAddress();
                var vendorDetail = _vendorService.GetVendorById(orderItem.Product.VendorId);


                if (vendorDetail != null)
                {
                    var vendorAddress = _addressService.GetAddressById(vendorDetail.AddressId);
                    pickupAddress.MobileNumber = vendorAddress.PhoneNumber;
                    pickupAddress.SupplierName = vendorDetail.Name;
                    pickupAddress.Address = _addressModelFactoryApi.AddressConcatenate(vendorAddress);
                    pickupAddress.Latitude = vendorAddress.Latitude ?? 0;
                    pickupAddress.Longitude = vendorAddress.Longitude ?? 0;
                }
                taxRate = 0M;
                priceWtihDiscount = string.Empty;
                decimal finalPriceWithoutDiscountBase = _taxService.GetProductPrice(orderItem.Product, _priceCalculationService.GetFinalPrice(orderItem.Product, _workContext.CurrentCustomer, includeDiscounts: false), out taxRate);
                decimal finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _workContext.WorkingCurrency);
                decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(orderItem.Product, _priceCalculationService.GetFinalPrice(orderItem.Product, _workContext.CurrentCustomer, includeDiscounts: true), out taxRate);
                decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);
                if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                {
                    priceWtihDiscount = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                }
                var itemModel = new ProductList
                {
                    AttributeInfo = orderItem.AttributeDescription,
                    Category = category.FirstOrDefault().Category.Name,
                    ImageUrl = PrepareCartItemPictureModel(orderItem, _mediaSettings.CartThumbPictureSize, true, _localizationService.GetLocalized(orderItem.Product, x => x.Name)).ImageUrl,
                    ProductId = orderItem.Product.Id,
                    ProductName = _localizationService.GetLocalized(orderItem.Product, x => x.Name),
                    Quantity = orderItem.Quantity,
                    //SubCategory = ""
                    PickupAddress = pickupAddress,
                    Price = _priceFormatter.FormatPrice(finalPriceWithoutDiscount),
                    PriceWithDiscount = priceWtihDiscount
                };
                if (vendorDetail != null && vendorDetail.Name.Trim() == "OneStop Mart")
                {
                    itemModel.IsOneStopMartVendor = true;
                }
                //added code by Sunil Kumar At 23-11-19
                var specificationAttributes = orderItem.Product.ProductSpecificationAttributes;
                foreach (var item in specificationAttributes)
                {
                    var psa = _specificationAttributeService.GetSpecificationAttributeById(item.SpecificationAttributeOption.SpecificationAttributeId);
                    if (psa != null && psa.Name != string.Empty)
                    {
                        if (psa.Name == "Special Requirements")
                        {
                            itemModel.IsSpecialRequirement = true;
                            itemModel.IsSpecialRequirementDesc = !string.IsNullOrEmpty(item.CustomValue) ? item.CustomValue : string.Empty;
                        }
                    }
                }
                itemModel.IsSpecialRequirementDesc = itemModel.IsSpecialRequirementDesc == null ? string.Empty : itemModel.IsSpecialRequirementDesc;
                model.ProductList.Add(itemModel);
            }

            //Delivery Address
            model.DeliveryAddress.CustomerName = order.BillingAddress.FirstName;
            model.DeliveryAddress.MobileNumber = order.BillingAddress.PhoneNumber;
            model.DeliveryAddress.Address = _addressModelFactoryApi.AddressConcatenate(order.BillingAddress);
            model.DeliveryAddress.Latitude = (order.BillingAddress.Latitude) != null ? ((decimal)order.BillingAddress.Latitude) : (decimal.Zero);
            model.DeliveryAddress.Longitude = (order.BillingAddress.Longitude) != null ? ((decimal)order.BillingAddress.Longitude) : (decimal.Zero);
            return model;
        }


        #endregion
    }
}

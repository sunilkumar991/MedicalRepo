using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Review;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.DeliveryBoy;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the Payment Transaction History model factory implementation
    /// </summary>
    public partial class PaymentTransactionHistoryModelFactory : IPaymentTransactionHistoryModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IReviewTypeService _reviewTypeService;
        private readonly IWorkContext _workContext;
        private readonly IPaymentTransactionHistoryService _paymentTransactionHistoryService;
        private readonly ICustomerService _customerService;
        private readonly IPriceFormatter _priceFormatter;

        #endregion

        #region Ctor

        public PaymentTransactionHistoryModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IReviewTypeService reviewTypeService,
            IWorkContext workContext,
            IPaymentTransactionHistoryService paymentTransactionHistoryService, ICustomerService customerService, IPriceFormatter priceFormatter)
        {
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._reviewTypeService = reviewTypeService;
            this._workContext = workContext;
            this._paymentTransactionHistoryService = paymentTransactionHistoryService;
            this._customerService = customerService;
            this._priceFormatter = priceFormatter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare PaymentTransaction History search model
        /// </summary>
        /// <param name="searchModel">PaymentTransaction History search model</param>
        /// <returns>>PaymentTransaction History search model</returns>
        public virtual PaymentTransactionHistorySearchModel PreparePaymentTransactionHistorySearchModel(PaymentTransactionHistorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare "approved" property(0 - all; 1 - OKDollar; 2 - 2C2P)
            searchModel.AvailablePaymentMethod.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.PaymentTransactionHistory.list.PaymentMethodType.All"),
                Value = "0"
            });
            searchModel.AvailablePaymentMethod.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.PaymentTransactionHistory.list.PaymentMethodType.OKDollar"),
                Value = "1"
            });
            searchModel.AvailablePaymentMethod.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.PaymentTransactionHistory.list.PaymentMethodType.2C2P"),
                Value = "2"
            });

            //prepare "approved" property(0 - all; 1 - Success; 2 - Failed)
            searchModel.AvailableTransactionStatus.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.PaymentTransactionHistory.list.TransactionStatus.All"),
                Value = "0"
            });
            searchModel.AvailableTransactionStatus.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.PaymentTransactionHistory.list.TransactionStatus.Success"),
                Value = "1"
            });
            searchModel.AvailableTransactionStatus.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.PaymentTransactionHistory.list.TransactionStatus.Failed"),
                Value = "2"
            });

            searchModel.AvailableTransactionStatus.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.PaymentTransactionHistory.list.TransactionStatus.Pending"),
                Value = "3"
            });


            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }


        /// <summary>
        /// Prepare PaymentTransaction History List Model
        /// </summary>
        /// <param name="searchModel">PaymentTransaction History search model</param>
        /// <returns>PaymentTransaction History list model</returns>
        public virtual PaymentTransactionHistoryListModel PreparePaymentTransactionHistoryListModel(PaymentTransactionHistorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));


            var paymentMethod = (PaymentTransactionMethodTypeEnum)searchModel.PaymentMethodId;

            //var paymentStatus = (PaymentTransactionStatusTypeEnum)searchModel.TransactionStatusId;

            //get parameters to filter reviews
            var createdOnFromValue = !searchModel.CreatedOnFrom.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);
            var createdToFromValue = !searchModel.CreatedOnTo.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);


            //get Payment Transaction History
            var paymentTransactionHistory = _paymentTransactionHistoryService.GetAllPaymentTransactionHistory(
                CustomerId: searchModel.CustomerId,
                TransactionId: searchModel.TransactionId,
                TransactionStatus: searchModel.TransactionStatusId,
                PaymentMethod: paymentMethod.ToString(),
                fromUtc: createdOnFromValue,
                toUtc: createdToFromValue,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            //prepare list model
            var model = new PaymentTransactionHistoryListModel
            {
                Data = paymentTransactionHistory.Select(paymentTransactionHistoryList =>
                {
                    //fill in model values from the entity
                    var paymentTransactionHistoryModel = new PaymentTransactionHistoryModel
                    {
                        Id = paymentTransactionHistoryList.Id,
                        PaymentMethod = paymentTransactionHistoryList.PaymentMethod,
                        TransactionId = paymentTransactionHistoryList.TransactionId,

                        TransactionAmount = _priceFormatter.FormatPrice(paymentTransactionHistoryList.TransactionAmount, true, false),

                        UpdatedBy = paymentTransactionHistoryList.UpdatedBy,
                        CreatedOnUtc = paymentTransactionHistoryList.CreatedOnUtc,
                        UpdatedOnUtc = paymentTransactionHistoryList.UpdatedOnUtc,
                        Comments = paymentTransactionHistoryList.Comments
                    };
                    if (paymentTransactionHistoryModel.PaymentMethod != null)
                    {
                        paymentTransactionHistoryModel.PaymentMethod = paymentTransactionHistoryModel.PaymentMethod.Replace("Payments.", "");
                    }
                    var paymentMethodTransactionStatus = (TransactionStatusType)paymentTransactionHistoryList.TransactionStatus;
                    paymentTransactionHistoryModel.TransactionMessage = paymentTransactionHistoryList.TransactionDescription;

                    paymentTransactionHistoryModel.IsTransactionSuccess = paymentMethodTransactionStatus.ToString() == "0" ? "" : paymentMethodTransactionStatus.ToString();

                    paymentTransactionHistoryModel.IssueStatus = (paymentTransactionHistoryList.IssueStatus != null && paymentTransactionHistoryList.IssueStatus != 0) ? ((IssueStatusType)paymentTransactionHistoryList.IssueStatus).ToString() : null;

                    paymentTransactionHistoryModel.IssueStatus = paymentTransactionHistoryModel.IssueStatus == "0" ? "" : paymentTransactionHistoryModel.IssueStatus;

                    paymentTransactionHistoryModel.CustomerName = paymentTransactionHistoryList.Customer.BillingAddress != null ? paymentTransactionHistoryList.Customer.BillingAddress.FirstName : paymentTransactionHistoryList.Customer.Username;

                    return paymentTransactionHistoryModel;
                }),
                Total = paymentTransactionHistory.TotalCount
            };

            return model;
        }

        #endregion
    }
}

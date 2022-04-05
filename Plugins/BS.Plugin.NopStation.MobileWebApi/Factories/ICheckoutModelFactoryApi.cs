﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Services.Payments;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Checkout;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    public partial interface ICheckoutModelFactoryApi
    {
        /// <summary>
        /// Prepare billing address model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="selectedCountryId">Selected country identifier</param>
        /// <param name="prePopulateNewAddressWithCustomerFields">Pre populate new address with customer fields</param>
        /// <param name="overrideAttributesXml">Override attributes xml</param>
        /// <returns>Billing address model</returns>
        CheckoutBillingAddressResponseModel PrepareBillingAddressModel(int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false,
            string overrideAttributesXml = "");

        /// <summary>
        /// Prepare shipping address model
        /// </summary>
        /// <param name="selectedCountryId">Selected country identifier</param>
        /// <param name="prePopulateNewAddressWithCustomerFields">Pre populate new address with customer fields</param>
        /// <param name="overrideAttributesXml">Override attributes xml</param>
        /// <returns>Shipping address model</returns>
        //CheckoutShippingAddressModel PrepareShippingAddressModel(int? selectedCountryId = null,
        //    bool prePopulateNewAddressWithCustomerFields = false, string overrideAttributesXml = "");

        /// <summary>
        /// Prepare shipping method model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <returns>Shipping method model</returns>
        CheckoutShippingMethodResponseModel PrepareShippingMethodModel(IList<ShoppingCartItem> cart, Address shippingAddress);

        /// <summary>
        /// Prepare payment method model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="filterByCountryId">Filter by country identifier</param>
        /// <returns>Payment method model</returns>
        CheckoutPaymentMethodResponseModel PreparePaymentMethodModel(IList<ShoppingCartItem> cart, int filterByCountryId);

        /// <summary>
        /// Prepare payment info model
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <returns>Payment info model</returns>
        //CheckoutPaymentInfoModel PreparePaymentInfoModel(IPaymentMethod paymentMethod);

        /// <summary>
        /// Prepare confirm order model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>Confirm order model</returns>
        //CheckoutConfirmModel PrepareConfirmOrderModel(IList<ShoppingCartItem> cart);

        /// <summary>
        /// Prepare checkout completed model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Checkout completed model</returns>
        CompleteResponseModel PrepareCheckoutCompletedModel(Order order);

        /// <summary>
        /// Prepare checkout progress model
        /// </summary>
        /// <param name="step">Step</param>
        /// <returns>Checkout progress model</returns>
        //CheckoutProgressModel PrepareCheckoutProgressModel(CheckoutProgressStep step);

        /// <summary>
        /// Prepare one page checkout model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>One page checkout model</returns>
        //OnePageCheckoutModel PrepareOnePageCheckoutModel(IList<ShoppingCartItem> cart);
    }
}
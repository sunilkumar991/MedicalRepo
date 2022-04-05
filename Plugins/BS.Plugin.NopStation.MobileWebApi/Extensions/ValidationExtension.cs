using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Product;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Common;
using FluentValidation;
using NUglify.Helpers;
using Nop.Web.Framework.Validators;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{
    public static class ValidationExtension
    {

        #region Utility
        private static bool IsNotValidEmail(this string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return !Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        private static bool IsNull(this object value)
        {
            return value == null;
        }

        private static bool IsNotRightLength(this string value, int min, int max)
        {
            if (value == null)
                return false;

            return !(value.Length >= min && value.Length <= max);
        }

        private static bool IsNotEqual(this string value, string checkValue)
        {
            if (checkValue == null)
                return false;

            return !value.Equals(checkValue);
        }
        private static void WithMessage(this bool flag, ModelStateDictionary modelState, string message)
        {
            if (flag == true)
            {
                modelState.AddModelError("", message);
            }
        }
        #endregion
        #region validation helpers
        public static void AddressValidator(ModelStateDictionary modelState, Address model, ILocalizationService localizationService, AddressSettings addressSettings, IStateProvinceService stateProvinceService)
        {
            model.FirstName.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.FirstName.Required"));
            model.LastName.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.LastName.Required"));
            if (!string.IsNullOrEmpty(model.Email))
            {
                //model.Email.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Email.Required"));
                model.Email.IsNotValidEmail().WithMessage(modelState, localizationService.GetResource("Common.WrongEmail"));
            }

            if (addressSettings.CountryEnabled)
            {

                model.CountryId.IsNull().
                    WithMessage(modelState, localizationService.GetResource("Address.Fields.Country.Required"));
                model.CountryId.Equals(0)
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Country.Required"));
            }
            if (addressSettings.CountryEnabled && addressSettings.StateProvinceEnabled)
            {

                //does selected country has states?
                var countryId = model.CountryId.HasValue ? model.CountryId.Value : 0;
                var hasStates = stateProvinceService.GetStateProvincesByCountryId(countryId).Count > 0;

                if (hasStates)
                {
                    //if yes, then ensure that state is selected
                    if (!model.StateProvinceId.HasValue || model.StateProvinceId.Value == 0)
                    {
                        modelState.AddModelError("StateProvinceId", localizationService.GetResource("Address.Fields.StateProvince.Required"));
                    }
                }

            }
            if (addressSettings.CompanyRequired && addressSettings.CompanyEnabled)
            {
                model.Company.IsNullOrWhiteSpace()
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Company.Required"));
            }
            if (addressSettings.StreetAddressRequired && addressSettings.StreetAddressEnabled)
            {
                model.Address1.IsNullOrWhiteSpace().
                    WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress.Required"));

            }
            if (addressSettings.StreetAddress2Required && addressSettings.StreetAddress2Enabled)
            {
                model.Address2.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress2.Required"));

            }
            if (addressSettings.ZipPostalCodeRequired && addressSettings.ZipPostalCodeEnabled)
            {
                model.ZipPostalCode.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.ZipPostalCode.Required"));
            }
            if (addressSettings.CityRequired && addressSettings.CityEnabled)
            {
                model.CityId.IsNull().WithMessage(modelState, localizationService.GetResource("Address.Fields.City.Required"));  //Changed By Ankur on 31/8/2018 
            }
            if (addressSettings.PhoneRequired && addressSettings.PhoneEnabled)
            {
                model.PhoneNumber.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Phone.Required"));
            }
            if (addressSettings.FaxRequired && addressSettings.FaxEnabled)
            {
                model.FaxNumber.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Fax.Required"));
            }
        }



        public static void CustomerInfoValidator(ModelStateDictionary modelState, CustomerInfoQueryModel model, ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            CustomerSettings customerSettings)
        {
            model.FirstName.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.FirstName.Required"));
            model.LastName.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.LastName.Required"));
            //model.Email.IsNullOrWhiteSpace().
            //    WithMessage(modelState, localizationService.GetResource("Address.Fields.Email.Required"));
            //model.Email.IsNotValidEmail().
            //    WithMessage(modelState, localizationService.GetResource("Common.WrongEmail"));

            if (customerSettings.UsernamesEnabled && customerSettings.AllowUsersToChangeUsernames)
            {
                model.Username.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Fields.Username.Required"));
            }

            //form fields
            if (customerSettings.CountryEnabled && customerSettings.CountryRequired)
            {
                model.CountryId.Equals(0)
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Country.Required"));
            }
            if (customerSettings.CountryEnabled &&
                customerSettings.StateProvinceEnabled &&
                customerSettings.StateProvinceRequired)
            {

                var hasStates = stateProvinceService.GetStateProvincesByCountryId(model.CountryId).Count > 0;

                if (hasStates)
                {
                    //if yes, then ensure that state is selected
                    if (model.StateProvinceId == 0)
                    {
                        modelState.AddModelError("StateProvinceId", localizationService.GetResource("Address.Fields.StateProvince.Required"));
                    }
                }
            }
            if (customerSettings.DateOfBirthRequired && customerSettings.DateOfBirthEnabled)
            {
                var dateOfBirth = model.ParseDateOfBirth();
                if (dateOfBirth == null)
                {
                    modelState.AddModelError("", localizationService.GetResource("Account.Fields.DateOfBirth.Required"));
                }

            }

            if (customerSettings.CompanyRequired && customerSettings.CompanyEnabled)
            {
                model.Company.IsNullOrWhiteSpace()
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Company.Required"));
            }
            if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled)
            {
                model.StreetAddress.IsNullOrWhiteSpace().
                    WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress.Required"));

            }
            if (customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled)
            {
                model.StreetAddress2.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress2.Required"));

            }
            if (customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled)
            {
                model.ZipPostalCode.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.ZipPostalCode.Required"));
            }
            if (customerSettings.CityRequired && customerSettings.CityEnabled)
            {
                model.City.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.City.Required"));
            }
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
            {
                model.Phone.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Phone.Required"));
            }
            if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
            {
                model.Fax.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Fax.Required"));
            }
        }

        public static void RegisterValidator(ModelStateDictionary modelState, RegisterQueryModel model, ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            CustomerSettings customerSettings)
        {
            model.FirstName.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.FirstName.Required"));
            model.LastName.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.LastName.Required"));
            model.Email.IsNullOrWhiteSpace().
                WithMessage(modelState, localizationService.GetResource("Address.Fields.Email.Required"));
            model.Email.IsNotValidEmail().
                WithMessage(modelState, localizationService.GetResource("Common.WrongEmail"));

            if (customerSettings.UsernamesEnabled && customerSettings.AllowUsersToChangeUsernames)
            {
                model.Username.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Fields.Username.Required"));
            }


            model.Password.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Fields.Password.Required"));
            model.Password.IsNotRightLength(customerSettings.PasswordMinLength, 999).WithMessage(modelState, string.Format(localizationService.GetResource("Account.Fields.Password.LengthValidation"), customerSettings.PasswordMinLength));
            model.ConfirmPassword.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Fields.ConfirmPassword.Required"));
            model.ConfirmPassword.IsNotEqual(model.Password).WithMessage(modelState, localizationService.GetResource("Account.Fields.Password.EnteredPasswordsDoNotMatch"));

            if (customerSettings.CountryEnabled && customerSettings.CountryRequired)
            {
                model.CountryId.Equals(0)
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Country.Required"));
            }
            if (customerSettings.CountryEnabled &&
                customerSettings.StateProvinceEnabled &&
                customerSettings.StateProvinceRequired)
            {

                var hasStates = stateProvinceService.GetStateProvincesByCountryId(model.CountryId).Count > 0;

                if (hasStates)
                {
                    //if yes, then ensure that state is selected
                    if (model.StateProvinceId == 0)
                    {
                        modelState.AddModelError("StateProvinceId", localizationService.GetResource("Address.Fields.StateProvince.Required"));
                    }
                }
            }
            if (customerSettings.DateOfBirthRequired && customerSettings.DateOfBirthEnabled)
            {
                var dateOfBirth = model.ParseDateOfBirth();
                if (dateOfBirth == null)
                {
                    modelState.AddModelError("", localizationService.GetResource("Account.Fields.DateOfBirth.Required"));
                }

            }

            if (customerSettings.CompanyRequired && customerSettings.CompanyEnabled)
            {
                model.Company.IsNullOrWhiteSpace()
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Company.Required"));
            }
            if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled)
            {
                model.StreetAddress.IsNullOrWhiteSpace().
                    WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress.Required"));

            }
            if (customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled)
            {
                model.StreetAddress2.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress2.Required"));

            }
            if (customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled)
            {
                model.ZipPostalCode.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.ZipPostalCode.Required"));
            }
            if (customerSettings.CityRequired && customerSettings.CityEnabled)
            {
                model.City.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.City.Required"));
            }
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
            {
                model.Phone.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Phone.Required"));
            }
            if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
            {
                model.Fax.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Fax.Required"));
            }
        }

        public static void RegisterValidator(ModelStateDictionary modelState, RegisterFromOKDollarQueryModel model, ILocalizationService localizationService,
    IStateProvinceService stateProvinceService,
    CustomerSettings customerSettings)
        {
            model.FirstName.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.FirstName.Required"));
            model.LastName.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.LastName.Required"));

            //Added by Alexandar Rajavel on 07-Mar-2019
            if (!string.IsNullOrEmpty(model.Email))
            {
                //model.Email.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Email.Required"));
                model.Email.IsNotValidEmail().WithMessage(modelState, localizationService.GetResource("Common.WrongEmail"));
            }
            if (customerSettings.UsernamesEnabled && customerSettings.AllowUsersToChangeUsernames)
            {
                model.UserName.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Fields.Username.Required"));
            }


            model.Password.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Fields.Password.Required"));
            model.Password.IsNotRightLength(customerSettings.PasswordMinLength, 999).WithMessage(modelState, string.Format(localizationService.GetResource("Account.Fields.Password.LengthValidation"), customerSettings.PasswordMinLength));
            //model.ConfirmPassword.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Fields.ConfirmPassword.Required"));
            //model.ConfirmPassword.IsNotEqual(model.Password).WithMessage(modelState, localizationService.GetResource("Account.Fields.Password.EnteredPasswordsDoNotMatch"));

            //if (customerSettings.CountryEnabled && customerSettings.CountryRequired)
            //{
            //    model.CountryId.Equals(0)
            //        .WithMessage(modelState, localizationService.GetResource("Address.Fields.Country.Required"));
            //}
            //if (customerSettings.CountryEnabled &&
            //    customerSettings.StateProvinceEnabled &&
            //    customerSettings.StateProvinceRequired)
            //{

            //    var hasStates = stateProvinceService.GetStateProvincesByCountryId(model.CountryId).Count > 0;

            //    if (hasStates)
            //    {
            //        //if yes, then ensure that state is selected
            //        if (model.StateProvinceId == 0)
            //        {
            //            modelState.AddModelError("StateProvinceId", localizationService.GetResource("Address.Fields.StateProvince.Required"));
            //        }
            //    }
            //}
            if (customerSettings.DateOfBirthRequired && customerSettings.DateOfBirthEnabled)
            {
                var dateOfBirth = model.ParseDateOfBirth();
                if (dateOfBirth == null)
                {
                    modelState.AddModelError("", localizationService.GetResource("Account.Fields.DateOfBirth.Required"));
                }

            }

            //if (customerSettings.CompanyRequired && customerSettings.CompanyEnabled)
            //{
            //    model.Company.IsNullOrWhiteSpace()
            //        .WithMessage(modelState, localizationService.GetResource("Address.Fields.Company.Required"));
            //}
            if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled)
            {
                model.Address1.IsNullOrWhiteSpace().
                    WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress.Required"));

            }
            if (customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled)
            {
                model.Address2.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress2.Required"));

            }
            //if (customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled)
            //{
            //    model.ZipPostalCode.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.ZipPostalCode.Required"));
            //}
            if (customerSettings.CityRequired && customerSettings.CityEnabled)
            {
                model.City.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.City.Required"));
            }
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
            {
                model.MobileNumber.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Phone.Required"));
            }
            //if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
            //{
            //    model.Fax.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Fax.Required"));
            //}
        }


        public static void LoginValidator(ModelStateDictionary modelState, LoginQueryModel model, ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            //if (!customerSettings.UsernamesEnabled)
            //{
            //    //login by email
            //    model.Email.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Login.Fields.Email.Required"));
            //    model.Email.IsNotValidEmail().WithMessage(modelState, localizationService.GetResource("Common.WrongEmail"));
            //    model.Password.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Fields.Password.Required"));
            //    model.Password.IsNotRightLength(customerSettings.PasswordMinLength, 999).WithMessage(modelState, string.Format(localizationService.GetResource("Account.Fields.Password.LengthValidation"), customerSettings.PasswordMinLength));
            //}
            if (!customerSettings.UsernamesEnabled)
            {
                //login by email
                model.Email.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Login.Fields.Email.Required"));
                model.Email.IsNotValidEmail().WithMessage(modelState, localizationService.GetResource("Common.WrongEmail"));
            }
            else
            {
                if (model.Username != null)
                {
                    //login by username 
                    model.Username.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Login.Fields.Username.Required"));
                }
                else
                {
                    //login by email
                    model.Email.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Login.Fields.Email.Required"));
                    model.Email.IsNotValidEmail().WithMessage(modelState, localizationService.GetResource("Common.WrongEmail"));
                }


            }
            model.Password.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Fields.Password.Required"));
        }

        //Added by Alexandar Rajavel on 17-Jan-2019
        public static void LoginValidatorForDeliveryApp(ModelStateDictionary modelState, LoginQueryModel model, ILocalizationService localizationService)
        {
            model.Username.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Login.Fields.Username.Required"));
            model.Password.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Fields.Password.Required"));
        }

        public static void WriteReviewValidator(ModelStateDictionary modelState, ProductReviewQueryModel model, ILocalizationService localizationService)
        {

            model.Title.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Reviews.Fields.Title.Required"));
            model.Title.IsNotRightLength(1, 200)
                .WithMessage(modelState,
                    string.Format(localizationService.GetResource("Reviews.Fields.Title.MaxLengthValidation"), 200));
            model.ReviewText.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Reviews.Fields.ReviewText.Required"));
        }

        public static void WriteReviewRequestValidator(ModelStateDictionary modelState, ReviewRequest model, ILocalizationService localizationService)
        {
            model.Rating.IsNull().WithMessage(modelState, localizationService.GetResource("Reviews.Fields.Rating.Required"));
            model.Rating.Equals(0).WithMessage(modelState, localizationService.GetResource("Reviews.Fields.Rating.Required"));
            model.OrderNo.Equals(0).WithMessage(modelState, localizationService.GetResource("Reviews.Fields.OrderNo.Required"));
            model.OrderNo.IsNull().WithMessage(modelState, localizationService.GetResource("Reviews.Fields.OrderNo.Required"));
            model.ReviewType.Equals(0).WithMessage(modelState, localizationService.GetResource("Reviews.Fields.ReviewType.Required"));
            model.ReviewType.IsNull().WithMessage(modelState, localizationService.GetResource("Reviews.Fields.ReviewType.Required"));
            if (!string.IsNullOrEmpty(model.ReviewText))
            {
                model.ReviewText.IsNotRightLength(1, 200)
                .WithMessage(modelState,
                    string.Format(localizationService.GetResource("Reviews.Fields.ReviewText.MaxLengthValidation"), 200));
            }
        }

        public static void SavePaymentTransactionHistoryValidator(ModelStateDictionary modelState, Models._QueryModel.Payment.SavePaymentTransactionHistory model, ILocalizationService localizationService)
        {
            if (!model.IsNull())
            {
                model.PaymentMethod.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Checkout.Fields.PaymentMethod.Required"));
                model.TransactionId.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Checkout.Fields.TransactionId.Required"));
                model.TransactionStatusCode.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Checkout.Fields.TransactionStatus.Required"));
                if (model.TransactionAmount.IsNull() || model.TransactionAmount <= 0)
                    model.TransactionAmount.ToString().IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Checkout.Fields.TransactionAmount.Required"));
            }
            else
            {
                modelState.AddModelError("", localizationService.GetResource("Checkout.Fields.InvalidData"));
            }

        }
        #endregion
    }
}

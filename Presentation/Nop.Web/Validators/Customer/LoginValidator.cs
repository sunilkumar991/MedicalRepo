﻿using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer
{
    public partial class LoginValidator : BaseNopValidator<LoginModel>
    {
        public LoginValidator(ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            if (!customerSettings.UsernamesEnabled)
            {
                //login by email
                RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.Login.Fields.Email.Required"));
                RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            }
            else
            {
                //login by username (mobile number) EC-200 By Ankur on 5 Nov.
                RuleFor(x => x.Username).NotEmpty().WithMessage(localizationService.GetResource("Account.Login.Fields.Username.Required"));
                
            }
            RuleFor(x => x.Password).NotEmpty().WithMessage(localizationService.GetResource("Account.Login.Fields.Password.Required"));
        }
    }
}
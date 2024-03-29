﻿using FluentValidation;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;


namespace Nop.Web.Areas.Admin.Validators.Directory
{

    public partial class CityValidator : BaseNopValidator<CityModel>
    {
        public CityValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Countries.States.Fields.Name.Required"));
            SetDatabaseValidationRules<City>(dbContext);
        }
    }
}

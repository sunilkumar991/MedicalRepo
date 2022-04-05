using FluentValidation;
using Nop.Web.Areas.Admin.Models.Shipping;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Shipping
{
    public partial class WarehouseValidator : BaseNopValidator<WarehouseModel>
    {
        public WarehouseValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Shipping.Warehouses.Fields.Name.Required"));
            RuleFor(x => x.Address.CityId).NotEqual(0).WithMessage(localizationService.GetResource("Admin.Address.Fields.City.Required"));
            //RuleFor(x => x.Address.City).NotEmpty().WithMessage(localizationService.GetResource("Admin.Address.Fields.City.Required"));
            RuleFor(x => x.Latitude).Must(ValidatorUtilities.LatitudeandLongitudeValidator).WithMessage(localizationService.GetResource("Admin.Address.Fields.Latitude.Invalid"));
            RuleFor(x => x.Longitude).Must(ValidatorUtilities.LatitudeandLongitudeValidator).WithMessage(localizationService.GetResource("Admin.Address.Fields.Longitude.Invalid"));
            SetDatabaseValidationRules<Warehouse>(dbContext);
        }
    }
}
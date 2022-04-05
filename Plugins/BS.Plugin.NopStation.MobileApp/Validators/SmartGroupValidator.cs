using BS.Plugin.NopStation.MobileApp.Models;
using BS.Plugin.NopStation.MobileApp.Services;
using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace BS.Plugin.NopStation.MobileApp.Validators
{
    public class SmartGroupValidator : BaseNopValidator<CriteriaModel>
    {
        public SmartGroupValidator(ILocalizationService localizationService, ISmartGroupService smartGroupsService)
        {
            RuleFor(x => x.Name)
                    .NotNull()
                    .WithMessage(localizationService.GetResource("Admin.Other.EmailMarketing.Groups.Fields.Name.Required"))
                    .Must((x, name) => !smartGroupsService.GroupNameIsExist(name, x.Id))
                    .WithMessage("This Group Name allready exist");
        }
    }
}

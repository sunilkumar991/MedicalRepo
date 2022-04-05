using FluentValidation;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Catalog
{
    public partial class ProductValidator : BaseNopValidator<ProductModel>
    {
        public ProductValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Name.Required"));
            RuleFor(x => x.ERPItemCode).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.ERPItemCode.Required"));
            RuleFor(x => x.skucodeerp).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.skucodeerp.Required"));
            RuleFor(x => x.DeliveryDateId).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.DelivaryDate.Required"));
            //Added code at 31-10-19 By Sunil Kumar 
            RuleFor(x => x.VendorId).GreaterThanOrEqualTo(1).WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Vendor.Required"));
            RuleFor(x => x.SeName).Length(0, NopSeoDefaults.SearchEngineNameLength)
                .WithMessage(string.Format(localizationService.GetResource("Admin.SEO.SeName.MaxLengthValidation"), NopSeoDefaults.SearchEngineNameLength));

            SetDatabaseValidationRules<Product>(dbContext);
        }
    }
}
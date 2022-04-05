using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial interface IAppAndAddressReviewModelFactory
    {
        /// <summary>
        /// Prepare App and Address review search model
        /// </summary>
        /// <param name="searchModel">App And Address review search model</param>
        /// <returns>App And Address review search model</returns>
        AppAddressReviewSearchModel PrepareAppAndAddressReviewSearchModel(AppAddressReviewSearchModel searchModel);

        /// <summary>
        /// Prepare paged App And Address review list model
        /// </summary>
        /// <param name="searchModel">App and Address review search model</param>
        /// <returns>>App And Address review list model</returns>
        AppAndAddressReviewListModel PrepareAppAndAddressReviewListModel(AppAddressReviewSearchModel searchModel);
    }
}

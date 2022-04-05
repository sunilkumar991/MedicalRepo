using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Core.Domain.Medicine;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial interface IMedicineRequestModelFactory
    {
        /// <summary>
        /// Prepare Medicine Request search model
        /// </summary>
        /// <param name="searchModel">App And Address review search model</param>
        /// <returns>App And Address review search model</returns>
        MedicineRequestSearchModel PrepareMedicineRequestSearchModel(MedicineRequestSearchModel searchModel);

        /// <summary>
        /// Prepare paged Medicine Request list model
        /// </summary>
        /// <param name="searchModel">App and Address review search model</param>
        /// <returns>>App And Address review list model</returns>
        MedicineRequestListModel PrepareMedicineRequestListModel(MedicineRequestSearchModel searchModel);

        /// <summary>
        /// Prepare paged medicine request list model
        /// </summary>
        /// <param name="searchModel">medicine request search model</param>
        /// <param name="medicineRequestWithId">Order</param>


        List<MedicineRequestItemModel> MedicineRequestItemModel(List<MedicineRequestItemModel> models, MedicineRequest medicineRequestWithId);

        /// <summary>
        /// Prepare medicine request search model
        /// </summary>
        /// <param name="searchModel">medicine request search model</param>
        /// <returns>>medicine request search model</returns>
        MedicineRequestItemModel PrepareMedicineRequestItemModel(MedicineRequestItemModel searchModel);


       
    }
}

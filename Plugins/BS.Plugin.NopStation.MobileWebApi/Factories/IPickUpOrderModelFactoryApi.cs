using BS.Plugin.NopStation.MobileWebApi.Models.PickUp;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    /// <summary>
    /// Represents the interface of the PickUp Order Model Factory Api
    /// </summary>
    public partial interface IPickUpOrderModelFactoryApi
    {
        /// <summary>
        /// Prepare the PickUp Order List model
        /// </summary>
        /// <returns>PickUp Order List model</returns>


        PickUpOrderListModel PrepareCustomerOrderListModel(PickUpOrderSearchModelApi searchModel, int pageNumber, int pageSize);


       

    }
}

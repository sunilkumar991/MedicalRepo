using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
   
    public class ShippingandDeliveryEnabledCitiesResponse : BaseResponse
    {
        public ShippingandDeliveryEnabledCitiesResponse()
        {
            TownShipDetails = new List<TownShipDetail>();
        }

        public List<TownShipDetail> TownShipDetails { get; set; }
    }
}

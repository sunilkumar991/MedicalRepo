using System;
using System.Collections.Generic;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Order
{
    public partial  class OderDetails : BaseResponse
    {
        public string OrderId { get; set; }
        public AddressModel OrderAddress { get; set; }
    }
}

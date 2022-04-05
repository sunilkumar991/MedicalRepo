using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer
{
    public class NewRegisterResponseModel<TResult> : BaseResponse
    {
        public TResult Data { get; set; }
    }
}

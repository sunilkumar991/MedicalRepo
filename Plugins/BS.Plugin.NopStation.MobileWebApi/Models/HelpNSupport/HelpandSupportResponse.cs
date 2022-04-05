using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.HelpNSupport
{
    public class HelpandSupportResponse
    {
        public HelpandSupportResponse()
        {
            Data = new List<ResultData>();
        }
        public IList<ResultData> Data { get; set; }
    }
}

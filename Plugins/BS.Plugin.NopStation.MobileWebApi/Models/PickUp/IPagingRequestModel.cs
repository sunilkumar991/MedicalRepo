using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.PickUp
{
    public partial interface IPagingRequestModel
    {
        /// <summary>
        /// Gets or sets a page number
        /// </summary>
        int Page { get; set; }

        /// <summary>
        /// Gets or sets a page size
        /// </summary>
        int PageSize { get; set; }
    }
}

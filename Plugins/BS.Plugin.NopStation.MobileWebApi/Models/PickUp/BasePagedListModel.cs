using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.PickUp
{
    public abstract partial class BasePagedListModel<T> : BaseNopModel, IPagedModel<T> where T : BaseNopModel
    {
        /// <summary>
        /// Gets or sets data records
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// Gets or sets total records number
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets an extra data
        /// </summary>
        public object ExtraData { get; set; }

        /// <summary>
        /// Gets or sets an errors
        /// </summary>
        public object Errors { get; set; }
    }
}

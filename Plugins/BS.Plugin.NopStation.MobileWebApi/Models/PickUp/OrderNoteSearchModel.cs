using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.PickUp
{
    public partial class OrderNoteSearchModel : BaseSearchModel
    {
        #region Properties

        public int OrderId { get; set; }

        #endregion
    }
}

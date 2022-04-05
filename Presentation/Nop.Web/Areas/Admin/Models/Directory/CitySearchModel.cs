using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a City Search Model
    /// </summary>
    public partial class CitySearchModel : BaseSearchModel
    {
        #region Properties

        public int StateId { get; set; }

        #endregion
    }
}
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    public partial class CountryController : BasePublicController
	{
        #region Fields

        private readonly ICountryModelFactory _countryModelFactory;
        
        #endregion
        
        #region Ctor

        public CountryController(ICountryModelFactory countryModelFactory)
		{
            this._countryModelFactory = countryModelFactory;
		}
        
        #endregion
        
        #region States / provinces

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult GetStatesByCountryId(string countryId, bool addSelectStateItem)
        {
            var model = _countryModelFactory.GetStatesByCountryId(countryId, addSelectStateItem);
            return Json(model);
        }

        #endregion

        #region City

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult GetCitiesByStateIdForShipping(string stateId, bool addSelectStateItem)
        {
            var model = _countryModelFactory.GetCitiesByStateIdForShipping(stateId, addSelectStateItem);
            return Json(model);
        }

        [CheckAccessPublicStore(true)]
        public virtual IActionResult GetCitiesByStateId(string stateId, bool addSelectStateItem)
        {
            var model = _countryModelFactory.GetCitiesByStateId(stateId, addSelectStateItem);
            return Json(model);
        }

        #endregion
    }
}
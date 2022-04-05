using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models.AppUpgradeDetails;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.AppUpgrade;
using Nop.Services.Customers;
using Microsoft.Extensions.Configuration;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    // Added by Sunil Kumar on 03-Jan-2020
    public class AppUpgradeController : BaseApiController
    {
        private IConfiguration _configuration;
        private readonly IAppUpgradeService _appUpgradeService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;

        public AppUpgradeController(IAppUpgradeService appUpgradeService, IWorkContext workContext,
             ICustomerService customerService, IStoreContext storeContext , IConfiguration iconfig)
        {
            _appUpgradeService = appUpgradeService;
            _workContext = workContext;
            _customerService = customerService;
            _storeContext = storeContext;
            _configuration = iconfig;
        }

        #region Action methods
        [Route("api/appupgrade/getappupgradedetails")]
        [HttpGet]
        public IActionResult GetMyMedicineRequest()
        {
            
            string messageforupdate = string.Empty;
            var result = new AppUpgradeDetailsResponse();
            var appUpgradeDetail = _appUpgradeService.GetAppUpgradeDetail();

            if (_workContext.WorkingLanguage.Id == 1)
            {
                messageforupdate = _configuration.GetValue<string>("English:Message");
            }
            else if (_workContext.WorkingLanguage.Id == 2)
            {
                messageforupdate = _configuration.GetValue<string>("Zawgi:Message");
            }
            else
            {
                messageforupdate = _configuration.GetValue<string>("Unicode:Message");
            }

            if (appUpgradeDetail != null)
            {
                result.AppID = appUpgradeDetail.Id;
                result.AppLatestVersionName = appUpgradeDetail.AppLatestVersionName;
                result.AppLatestVersionCode = appUpgradeDetail.AppLatestVersionCode;
                result.AppURL = appUpgradeDetail.AppURL;
                result.IsUpdateRequired = appUpgradeDetail.IsUpdateRequired;
                result.IsBackgroundDownload = appUpgradeDetail.IsBackgroundDownload;
                result.message = messageforupdate;
                result.IsProduction = appUpgradeDetail.IsProduction;
                result.playStoreUrl = appUpgradeDetail.playStoreUrl;
                result.apkName = appUpgradeDetail.apkName;
                result.IsUpdateForceRequired = appUpgradeDetail.IsUpdateForceRequired;
                result.DeviceId = appUpgradeDetail.DeviceId;
            }
            else
            {
                var orderResult = new AppUpgradeDetailsResponse();
                orderResult.StatusCode = (int)ErrorType.Ok;
                orderResult.ErrorList.Add(NO_DATA);
                return Ok(orderResult);


            }
            return Ok(result);
        }


        [Route("api/appupgrade/getappupgradedetails/{deviceid}")]
        [HttpGet]
        public IActionResult getappupgradedetails(int deviceid)
        {

            string messageforupdate = string.Empty;
            var result = new AppUpgradeDetailsResponse();
            var appUpgradeDetail = _appUpgradeService.GetAppUpgradeDetailBasedonDevicerid(deviceid);

            if (_workContext.WorkingLanguage.Id == 1)
            {
                messageforupdate = _configuration.GetValue<string>("English:Message");
            }
            else if (_workContext.WorkingLanguage.Id == 2)
            {
                messageforupdate = _configuration.GetValue<string>("Zawgi:Message");
            }
            else
            {
                messageforupdate = _configuration.GetValue<string>("Unicode:Message");
            }

            if (appUpgradeDetail != null)
            {
                result.AppID = appUpgradeDetail.Id;
                result.AppLatestVersionName = appUpgradeDetail.AppLatestVersionName;
                result.AppLatestVersionCode = appUpgradeDetail.AppLatestVersionCode;
                result.AppURL = appUpgradeDetail.AppURL;
                result.IsUpdateRequired = appUpgradeDetail.IsUpdateRequired;
                result.IsBackgroundDownload = appUpgradeDetail.IsBackgroundDownload;
                result.message = messageforupdate;
                result.IsProduction = appUpgradeDetail.IsProduction;
                result.playStoreUrl = appUpgradeDetail.playStoreUrl;
                result.apkName = appUpgradeDetail.apkName;
                result.IsUpdateForceRequired = appUpgradeDetail.IsUpdateForceRequired;
                result.DeviceId = appUpgradeDetail.DeviceId;
            }
            else
            {
                var orderResult = new AppUpgradeDetailsResponse();
                orderResult.StatusCode = (int)ErrorType.Ok;
                orderResult.ErrorList.Add(NO_DATA);
                return Ok(orderResult);


            }
            return Ok(result);
        }



        #endregion
    }
}

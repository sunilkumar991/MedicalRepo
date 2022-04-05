using System;
using System.Linq;
using System.Text.RegularExpressions;
using Nop.Web.Areas.Admin.Models.Settings;

namespace Nop.Web.Areas.Admin.Validators
{
    public class ValidatorUtilities
    {
        public static bool PageSizeOptionsValidator(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
            var notValid = value.Split(',').Select(p => p.Trim()).GroupBy(p => p).Any(p => p.Count() > 1);
            return !notValid;
        }

        public static bool PageSizeOptionsInAdvancedSettingsValidator(SettingModel model, string value)
        {
            if (model.Name.ToLower().Contains("pagesizeoptions"))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }

                var notValid = value.Split(',').Select(p => p.Trim()).GroupBy(p => p).Any(p => p.Count() > 1);
                return !notValid;
            }
            return true;
        }
        public static bool LatitudeandLongitudeValidator(Decimal value)
        {
            Regex regex = new Regex(@"^([-+]?)([\d]{1,3})(((\.)(\d+)))");
            Match match = regex.Match(Convert.ToString(value));
            if (match.Success&& match.Captures.Count==1)
            {
            //    while (Regex.IsMatch(Convert.ToString(value), @"[\d]{1,3}([.][\d]{1,9})?"))
            //{
                return true;
            }

            return false;
        }
    }
}
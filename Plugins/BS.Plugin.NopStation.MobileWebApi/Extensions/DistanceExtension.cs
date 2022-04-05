using System;
using System.Collections.Generic;

namespace BS.Plugin.NopStation.MobileApp.Extensions
{
    //Added by Sunil Kumar at 3/1/19
    public static class DistanceExtension
    {
       
        static public double distance(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1))
                            * Math.Sin(deg2rad(lat2))
                            + Math.Cos(deg2rad(lat1))
                            * Math.Cos(deg2rad(lat2))
                            * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            return (dist);
        }

       static public double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

       static public double rad2deg(double rad)
        {
            return (rad * 180.0 / Math.PI);
        }

    }
}

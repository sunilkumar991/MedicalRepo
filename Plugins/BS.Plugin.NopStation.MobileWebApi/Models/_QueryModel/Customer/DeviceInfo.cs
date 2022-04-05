using System;
using System.Collections.Generic;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer
{
    public class DeviceInfo
    {
        public string BSSID { get; set; }
        public string BluetoothAddress { get; set; }
        public string BluetoothName { get; set; }
        public string ConnectedNetworkType { get; set; }
        public string DeviceSoftwareVersion { get; set; }
        public bool HiddenSSID { get; set; }
        public string IPAddress { get; set; }
        public int LinkSpeed { get; set; }
        public string MACAddress { get; set; }
        public string NetworkCountryIso { get; set; }
        public int NetworkID { get; set; }
        public string NetworkOperator { get; set; }
        public string NetworkOperatorName { get; set; }
        public int NetworkSignal { get; set; }
        public string NetworkType { get; set; }
        public string PhoneType { get; set; }
        public string SIMCountryIso { get; set; }
        public string SIMOperator { get; set; }
        public string SIMOperatorName { get; set; }
        public string SSID { get; set; }
        public string VoiceMailNo { get; set; }
        public bool isNetworkRoaming { get; set; }
        public string Cellid { get; set; }
        public string Simid1 { get; set; }
        public string Msid1 { get; set; }
        public string Simid2 { get; set; }
        public string Msid2 { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}

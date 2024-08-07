using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Common
{
    //<root>
    //    <item>
    //    <devmode>0</devmode>
    //    <version>1.03.24</version>
    //    <url>http://192.168.10.82:2000/Production/PC.Launcher.zip</url>
    //    </item>
    //    <item>
    //    <devmode>1</devmode>
    //    <version>1.03.24</version>
    //    <url>http://192.168.10.82:2000/Development/PC.Launcher.zip</url>
    //    </item>
    //</root>
    public static class VersionHelper
    {

        const string ApplicationKey = @"SOFTWARE\OneGateMB\OGSpider";
        const string VersionKey = "Version";
        static string UrlCheckVersionOnline = ConfigurationManager.AppSettings["UrlCheckUpdate"];
        public static string GetCurrentVersion()
        {
            try
            {
                var tempKey = Registry.LocalMachine.OpenSubKey(ApplicationKey);
                foreach (string valueName in tempKey.GetValueNames())
                {
                    var version = tempKey.GetValue(valueName).ToString();

                    return version;
                }

                return "0.0.0.0";
            }
            catch (Exception)
            {
                return "0.0.0.0";
            }
        }
        public static void SetVersionRegistry(string version = "1.0.0.0")
        {
            try
            {
               
               
                var key = Registry.LocalMachine.CreateSubKey(ApplicationKey);
                key.SetValue(VersionKey, version);
                var versionInstalled = key.GetValue(VersionKey).ToString();
            }
            catch (Exception ex)
            {
                LogHelper.LogInfo($"SetVersionRegistry Exception:{ex.Message}");
            }
        }
        public static VersionInfo GetCurrentVersionOnline()
        {
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = client.GetAsync(UrlCheckVersionOnline).Result)
                    {
                        var streamTo = response.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(streamTo))
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(streamTo);
                            var allItem = xmlDoc.SelectNodes("/root/item");
                            var lt = new List<VersionInfo>();
                            foreach (XmlNode item in allItem)
                            {
                                lt.Add(new VersionInfo()
                                {
                                    DevMode = int.Parse(item.SelectSingleNode("devmode").InnerText),
                                    Url = item.SelectSingleNode("url").InnerText,
                                    Version = item.SelectSingleNode("version").InnerText
                                });
                            }
                            //DevMode PC
                            int DevMode = 0;

                            return lt.FirstOrDefault(x => x.DevMode == DevMode);
                        }
                        else
                            return null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogInfo($"GetCurrentVersionOnline Exception:{ex.Message}");
                return null;
            }
        }

        public static bool CheckNewVersionOnline()
        {
            try
            {
                var versionInstalled = GetCurrentVersion();
                var versionOnline = GetCurrentVersionOnline();
                if (versionOnline != null && versionInstalled != null)
                {
                    return new Version(versionOnline.Version) > new Version(versionInstalled);
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static ComputerInfo GetCurrentComputerInfo()
        {
            try
            {
                ComputerInfo currentComputer = new ComputerInfo();
                currentComputer.computer_name = System.Environment.GetEnvironmentVariable("COMPUTERNAME");

                ManagementObjectSearcher mosProcessor = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

                foreach (ManagementObject moProcessor in mosProcessor.Get())
                {
                    if (moProcessor["name"] != null)
                    {
                        currentComputer.cpu = moProcessor["name"].ToString().Trim();
                        break;
                    }
                }

                mosProcessor.Query = new ObjectQuery("Select * From Win32_ComputerSystem");
                foreach (ManagementObject obj in mosProcessor.Get())
                {
                    currentComputer.ram = $"{Math.Round(Convert.ToDouble(obj["TotalPhysicalMemory"]) / (1024 * 1024 * 1024))} GB".Trim();
                    break;

                }

                mosProcessor.Query = new ObjectQuery("Select * From Win32_VideoController");
                foreach (ManagementObject moProcessor in mosProcessor.Get())
                {
                    if (moProcessor["name"] != null)
                    {
                        currentComputer.video_card = moProcessor["name"].ToString().Trim();
                        break;
                    }
                }

                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct");
                ManagementObjectCollection moc = mos.Get();

                foreach (ManagementObject mo in moc)
                {
                    currentComputer.uuid = ((string)mo["UUID"]).Trim();
                    break;
                }

                currentComputer.public_ip = GetPublicIpInternet().Trim();

                return currentComputer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetPublicIpInternet()
        {
            try
            {
                List<string> services = new List<string>()
                {
                    "https://ipinfo.io/ip"
                };
                using (var webclient = new WebClient())
                    foreach (var service in services)
                    {
                        try
                        {
                            return IPAddress.Parse(webclient.DownloadString(service)).ToString();
                        }
                        catch (Exception)
                        {
                            //ConstHelper.LogService.Info($"GetPublicIp-->WebClient Exception: {ex.Message}");
                        }
                    }
                return null;
            }
            catch (Exception)
            {
                //ConstHelper.LogService.Info($"GetPublicIpInternet Exception: {ex.Message}");
                return null;
            }
        }

        public static async void UpdateComputerInfo(ComputerInfo body)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("");
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("/api/computer/start", content);
                }
            }
            catch (Exception)
            {

            }
        }
    }

    public class VersionInfo
    {
        public string Version { get; set; } = "1.0.0.0";
        public string Url { get; set; }
        public int DevMode { get; set; } = 0;
    }
    public class ComputerInfo
    {
        public string computer_name { get; set; }
        public string cpu { get; set; }
        public string ram { get; set; }
        public string video_card { get; set; }
        public string uuid { get; set; }
        public string public_ip { get; set; }
        public DateTime? created_at_utc { get; set; }
        public DateTime? lastused_at_utc { get; set; }
    }
}

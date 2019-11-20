using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopAppTraceLog
    {
        public RopAppTraceLog()
        {
            this.device = new Device();
            this.appActions = new List<AppAction>();
            this.pages = new List<Page>();
            this.events = new List<Event>();
            this.exceptionInfos = new List<ExceptionInfo>();
        }

        public Device device { get; set; }

        public List<AppAction> appActions { get; set; }
        public List<Page> pages { get; set; }
        public List<Event> events { get; set; }

        public List<ExceptionInfo> exceptionInfos { get; set; }

        public class AppAction
        {
            public string action_time { get; set; }
            public int action_type { get; set; }
            public string action_desc { get; set; }
        }

        public class Page
        {
            public string page_id { get; set; }
            public string referer_page_id { get; set; }
            public string page_start_time { get; set; }
            public string page_end_time { get; set; }
            public string city_id { get; set; }
            public string uid { get; set; }
        }

        public class Event
        {
            public string page_id { get; set; }
            public string referer_page_id { get; set; }
            public string uid { get; set; }
            public string city_id { get; set; }
            public string event_name { get; set; }
            public string action_time { get; set; }
        }


        public class Device
        {
            public Device()
            {
                this.appinfo = new AppInfo();
                this.deviceinfo = new DeviceInfo();
                this.networkinfo = new NetworkInfo();

            }

            public AppInfo appinfo { get; set; }
            public DeviceInfo deviceinfo { get; set; }
            public NetworkInfo networkinfo { get; set; }

        }

        public class AppInfo
        {
            public string appChannel { get; set; }
            public string appId { get; set; }
            public string appVersion { get; set; }
        }

        public class DeviceInfo
        {
            public string deviceDensity { get; set; }
            public string deviceId { get; set; }
            public string deviceLocale { get; set; }
            public string deviceMacAddr { get; set; }
            public string deviceModel { get; set; }
            public string deviceOsVersion { get; set; }
            public string devicePlatform { get; set; }
            public string deviceScreen { get; set; }
        }

        public class NetworkInfo
        {
            public string ipAddr { get; set; }
            public bool wifi { get; set; }
        }

        public class ExceptionInfo
        {
            public string phoneModel { get; set; }
            public string systemModel { get; set; }
            public string systemVersion { get; set; }
            public string exceptionString { get; set; }
        }
    }
}

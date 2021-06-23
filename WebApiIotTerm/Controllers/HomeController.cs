
using LocalS.Service.Api.StoreTerm;
using Lumos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebApiIotTerm.Controllers
{
    public class HomeController : Controller
    {
        private string merch_id = "d17df2252133478c99104180e8062230";
        private string secret = "6ZB97cdVz211O08EKZ6yriAYrHXFBowC";
        private long timespan = (long)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds;
        //private long timespan = 1620465964;

        private string host = "http://api.iot.17fanju.com";

        Dictionary<string, string> model = new Dictionary<string, string>();

        public ActionResult Index()
        {
            model.Add("设备信息", DeviceList());
            model.Add("设备库存", DeviceStock());

            return View(model);
        }

        public string DeviceList()
        {
            string data = "{\"page\":0,\"limit\":10}";
            string sign = GetSign(data);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", string.Format("merch_id={0},timestamp={1},sign={2}", merch_id, timespan, sign));
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/device/list", data, headers);
            return result;
        }

        public string DeviceStock()
        {
            string data = "{\"device_id\":\"202004220056\",\"data_format\":\"sku\", \"cabinet_id\":\"\", \"is_need_detail\":true }";
            string sign = GetSign(data);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", string.Format("merch_id={0},timestamp={1},sign={2}", merch_id, timespan, sign));
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/device/stock", data, headers);
            return result;
        }

        public string GetSign(string data)
        {
            var sb = new StringBuilder();

            sb.Append(merch_id);
            sb.Append(secret);
            sb.Append(timespan.ToString());
            sb.Append(data);

            var material = string.Concat(sb.ToString().OrderBy(c => c));

            var input = Encoding.UTF8.GetBytes(material);

            var hash = SHA256Managed.Create().ComputeHash(input);

            StringBuilder sb2 = new StringBuilder();
            foreach (byte b in hash)
                sb2.Append(b.ToString("x2"));

            string str = sb2.ToString();

            return str;
        }

    }
}
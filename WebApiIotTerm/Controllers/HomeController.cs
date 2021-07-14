
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
        public const short MaxTimeDiff = 30;

        private string merch_id = "87596751";
        private string secret = "6ZB97cdVz211O08EKZ6yriAYrHXFBowC";
        private long timespan = (long)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds;
        //private long timespan = 1620465964;

        private string host = "http://api.iot.17fanju.com";

        Dictionary<string, string> model = new Dictionary<string, string>();

        public bool IsRequestTimeout(long app_timestamp)
        {
            System.DateTime time = System.DateTime.MinValue;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime app_requestTime = startTime.AddMilliseconds(app_timestamp);

            var ts = DateTime.Now - app_requestTime;
            if (System.Math.Abs(ts.TotalMinutes) > MaxTimeDiff)
            {
                return true;
            }

            return false;
        }

        public ActionResult Index()
        {
            string a = timespan.ToString();
            IsRequestTimeout(1626142536385);

            //model.Add("设备信息", DeviceList());
            //model.Add("设备库存", DeviceStock());
            model.Add("订单下单", OrderReserve());
            //model.Add("订单查看", OrderQuery());
            //model.Add("订单取消", OrderCancle());
            //model.Add("商品添加", ProductAdd());
            //model.Add("商品修改", ProductEdit());
            //model.Add("订单销售记录", OrderSaleRecords());
            return View(model);
        }

        public string DeviceList()
        {
            string data = "{\"page\":0,\"limit\":10,\"device_cum_code\":\"test\"}";
            string sign = GetSign(data);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", string.Format("merch_id={0},timestamp={1},sign={2}", merch_id, timespan, sign));
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/device/list", data, headers);
            return result;
        }

        public string DeviceStock()
        {
            string data = "{\"device_cum_code\":\"test1\",\"data_format\":\"sku\", \"cabinet_id\":\"\", \"is_need_detail\":false }";
            string sign = GetSign(data);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", string.Format("merch_id={0},timestamp={1},sign={2}", merch_id, timespan, sign));
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/device/stock", data, headers);
            return result;
        }

        public string OrderReserve()
        {
            string data = "{\"device_id\":\"202004220011\",\"low_order_id\":\"6100137202001221450573323\",\"is_im_ship\":false,\"notify_url\":\"http://www.xxxx.com/xxxxx/xxx\",\"detail\":[{\"sku_id\":\"0a8cc495b3714c6eb8fff32043801ed5\",\"sku_cum_code \":\"xxxxx\",\"quantity\":1}]}";
            string sign = GetSign(data);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", string.Format("merch_id={0},timestamp={1},sign={2}", merch_id, timespan, sign));
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/order/reserve", data, headers);
            return result;
        }

        public string OrderQuery()
        {
            string data = "{\"low_order_id\":\"6100137202001221450573321\",\"business_type\":\"shipment\"}";
            string sign = GetSign(data);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", string.Format("merch_id={0},timestamp={1},sign={2}", merch_id, timespan, sign));
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/order/query", data, headers);
            return result;
        }

        public string OrderCancle()
        {
            string data = "{\"low_order_id\":\"6100137202001221450573321\"}";
            string sign = GetSign(data);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", string.Format("merch_id={0},timestamp={1},sign={2}", merch_id, timespan, sign));
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/order/cancle", data, headers);
            return result;
        }

        public string OrderSaleRecords()
        {
            string data = "{\"page\":0,\"limit\":10,\"sale_date\":\"2021-05-12\"}";
            string sign = GetSign(data);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", string.Format("merch_id={0},timestamp={1},sign={2}", merch_id, timespan, sign));
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/order/salerecords", data, headers);
            return result;
        }

        public string ProductAdd()
        {
            string data = "{\"name\":\"格力高百醇（草莓味）\",\"spu_code\":\"00210526021xx\",\"spec_items\":[\"份量\"],\"spec_skus\":[{\"cum_code\":\"YC024xxaa\",\"bar_code\":\"1233211234567\",\"sale_price\":6.00,\"spec_val\":[\"大份\"]},{\"cum_code\":\"YC025xxx\",\"bar_code\":\"1233211234568\",\"sale_price\":4.00,\"spec_val\":[\"小份\"]}],\"kind_ids\":[101,10104,1010401],\"brief_des\":\"外层香脆，内芯柔软\",\"display_img_urls\":[\"https://file.17fanju.com/Upload/product/58378e81-a947-463c-8c11-8642f1982da6_O.jpg\",\"https://file.17fanju.com/Upload/product/58378e81-a947-463c-8c11-8642f1982da6_O.jpg\"],\"details_des\":[\"https://file.17fanju.com/Upload/product/58378e81-a947-463c-8c11-8642f1982da6_O.jpg\",\"https://file.17fanju.com/Upload/product/58378e81-a947-463c-8c11-8642f1982da6_O.jpg\",\"https://file.17fanju.com/Upload/product/58378e81-a947-463c-8c11-8642f1982da6_O.jpg\"]}";
            string sign = GetSign(data);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", string.Format("merch_id={0},timestamp={1},sign={2}", merch_id, timespan, sign));
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/product/add", data, headers);
            return result;
        }

        public string ProductEdit()
        {
            string data = "{\"spu_id\":\"e63ee7723c57496a8935bb5e557b545f\", \"name\":\"格力高百醇（草莓味）\",\"spu_code\":\"00210526021x\",\"spec_items\":[\"份量\"],\"spec_skus\":[{\"sku_id\":\"95822e1a77cb47c3b2b31f6b992aa7c1\",\"cum_code\":\"YC024xxaffff\",\"bar_code\":\"1233211234567\",\"sale_price\":6.00,\"spec_val\":[\"大份\"]},{\"sku_id\":\"418f480e539d4235abdfce81ed07a5db\",\"cum_code\":\"YC025xxfffx\",\"bar_code\":\"1233211234568\",\"sale_price\":4.00,\"spec_val\":[\"小份\"]}],\"kind_ids\":[101,10104,1010401],\"brief_des\":\"外层香脆，内芯柔软\",\"display_img_urls\":[\"https://file.17fanju.com/Upload/product/58378e81-a947-463c-8c11-8642f1982da6_O.jpg\",\"https://file.17fanju.com/Upload/product/58378e81-a947-463c-8c11-8642f1982da6_O.jpg\"],\"details_des\":[\"https://file.17fanju.com/Upload/product/58378e81-a947-463c-8c11-8642f1982da6_O.jpg\",\"https://file.17fanju.com/Upload/product/58378e81-a947-463c-8c11-8642f1982da6_O.jpg\",\"https://file.17fanju.com/Upload/product/58378e81-a947-463c-8c11-8642f1982da6_O.jpg\"]}";
            string sign = GetSign(data);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", string.Format("merch_id={0},timestamp={1},sign={2}", merch_id, timespan, sign));
            HttpUtil http = new HttpUtil();
            string result = http.HttpPostJson("" + host + "/api/product/edit", data, headers);
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
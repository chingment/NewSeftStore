
using LocalS.BLL;
using LocalS.DAL;
using LocalS.Service.Api.StoreApp;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebApiStoreApp.Controllers
{
    public class HomeController : Controller
    {
        private string key = "test";
        private string secret = "6ZB97cdVz211O08EKZ6yriAYrHXFBowC";
        private long timespan = (long)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds;
        private string host = "";

        Dictionary<string, string> model = new Dictionary<string, string>();

        public ActionResult Index()
        {
            try
            {
                //CacheServiceFactory.SellChannelStock.ReSet();

                //RopOrderReserve rop = new RopOrderReserve();
                //rop.StoreId = "21ae9399b1804dbc9ddd3c29e8b5c670";
                //rop.Source = LocalS.Entity.E_OrderSource.Machine;
                //rop.ProductSkus.Add(new RopOrderReserve.ProductSku { Id = "833448c77b8b4563b3682e1113907fba", CartId = "b25ead6767a6436f9375176cef4004e5", Quantity = 2, ReceptionMode = LocalS.Entity.E_ReceptionMode.Machine });
                //StoreAppServiceFactory.Order.Reserve("e170b69479c14804a38b089dac040740", "e170b69479c14804a38b089dac040740", rop);

                //DbContext db = new DbContext();
                //var s = db.Store.Where(m=>m.IsDelete==false).ToList();


            }
            catch (Exception ex)
            {
                var a = "ad";
            }
            //object isTest = ConfigurationManager.AppSettings["custom:IsTest"];
            //if (isTest == null)
            //{
            //    isTest = "false";
            //}

            //host = "https://demo.res.17fanju.com";

            //if (isTest.ToString() == "false")
            //{
            //    host = "https://demo.res.17fanju.com";
            //}
            //else
            //{
            //     host = "http://localhost:16665";
            //}


            //string clientId = "00000000000000000000000000000000";
            //string storeId = "be9ae32c554d4942be4a42fa48446210";



            ////model.Add("获取全局数据", GlobalDataSet(clientId, storeId, DateTime.Parse("2018-04-09 15:14:28")));
            ////model.Add("获取全局数据", ShippingAddress(clientId, storeId));
            ////model.Add("获取地址", GetShippingAddress(1215));
            //string aa = "rnx2OLcr5wRVFkLrDuv6hypG+VNHtCr+r5DrsKIrPUrEbG4NGTH7UnirjzbZKrYkxdxYRWl/ei+6dLhNMJh+kzM8NPYwfxclqh0kzHXyuopZ/RHNcJy9qIBb0gFKLOH/p7+/QHRvAdBStDz9gmuTf3DhtpUcw1/U2OJvQtjW4B3sr095619fniJsSok+O5XESbKfgU9AsTPdGgGn6DEpfgt3zjvycN1EhPKlkx68NJoGjP9oIFvrOxW7fXjfv6+o6Q2/X4A8buLrpRVYsQ+8qfp+JYSLnZDoXkR+XBEx+sn3iETilxtDDNsEGHBlR+2MKbj51RiRmxDIAkwYvNCfD/O79X9AnIEavL129Oxib0Gb4Br6MwAvugiGpTcFnpDjC7zssH9LmetCXPUjWUPZ1fidcSHtMIBwpMwQpl7oBaGX8ftU5vs3GFz2yASGQHanoJeT1OAl/Mdu/p+Muq6+0vL2Ven8GJEMtnPpzgF2v0c=";
            //string xx = "0239gQNL1xGz651x9XOL1VaSNL19gQNK";
            //string bb = "Dz8+EgdBeZqX4EOl8r/yxQ==";
            //model.Add("用户小程序授权", LoginByMinProgram(aa, xx, bb));
            //model.Add("订单确认", OrderConfirm("100e4a2715244d749a08aa88c51a5153","a1d1740312b34691b243453db81bf007"));
            return View(model);
        }

        public static string stringSort(string str)
        {
            char[] chars = str.ToCharArray();
            List<string> lists = new List<string>();
            foreach (char s in chars)
            {
                lists.Add(s.ToString());
            }
            lists.Sort();//sort默认是从小到大的。显示123456789      

            str = "";
            foreach (string item in lists)
            {
                str += item;
            }
            return str;
        }

        //public string GlobalDataSet(string userId, string storeId, DateTime datetime)
        //{
        //    Dictionary<string, string> parames = new Dictionary<string, string>();
        //    parames.Add("userId", userId);
        //    parames.Add("storeId", storeId.ToString());
        //    parames.Add("datetime", datetime.ToUnifiedFormatDateTime());
        //    string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

        //    Dictionary<string, string> headers = new Dictionary<string, string>();
        //    headers.Add("key", key);
        //    headers.Add("timestamp", timespan.ToString());
        //    headers.Add("sign", signStr);
        //    HttpUtil http = new HttpUtil();
        //    string result = http.HttpGet("" + host + "/api/Global/DataSet?userId=" + userId + "&storeId=" + storeId + "&datetime=" + HttpUtility.UrlEncode(datetime.ToUnifiedFormatDateTime(), UTF8Encoding.UTF8).ToUpper(), headers);

        //    return result;

        //}

        //public string LoginByMinProgram(string encryptedDataStr, string code, string iv)
        //{
        //    RopLoginByMinProgram model = new RopLoginByMinProgram();

        //    model.EncryptedData = encryptedDataStr;
        //    model.Code = code;
        //    model.Iv = iv;

        //    string a1 = JsonConvert.SerializeObject(model);

        //    string signStr = Signature.Compute(key, secret, timespan, a1);


        //    Dictionary<string, string> headers = new Dictionary<string, string>();
        //    headers.Add("key", key);
        //    headers.Add("timestamp", timespan.ToString());
        //    headers.Add("sign", signStr);
        //    headers.Add("version", "1.3.0.7");
        //    HttpUtil http = new HttpUtil();
        //    string result = http.HttpPostJson("" + host + "/api/User/LoginByMinProgram", a1, headers);

        //    return result;

        //}
        //public string OrderConfirm(string accessToken, string orderId)
        //{
        //    RopOrderConfirm model = new RopOrderConfirm();

        //    model.OrderId = orderId;

        //    string a1 = JsonConvert.SerializeObject(model);

        //    string signStr = Signature.Compute(key, secret, timespan, a1);


        //    Dictionary<string, string> headers = new Dictionary<string, string>();
        //    headers.Add("key", key);
        //    headers.Add("timestamp", timespan.ToString());
        //    headers.Add("sign", signStr);
        //    headers.Add("version", "1.3.0.7");
        //    HttpUtil http = new HttpUtil();
        //    string result = http.HttpPostJson("" + host + "/api/Order/Confirm?accessToken=" + accessToken, a1, headers);

        //    return result;

        //}

        //public string ShippingAddress(string userId, string storeId)
        //{
        //    Models.ShippingAddress.EditModel model = new Models.ShippingAddress.EditModel();

        //    model.UserId = userId;
        //    model.PhoneNumber = "15989287032";
        //    model.Address = "3123";
        //    model.AreaCode = "1";
        //    model.AreaName = "2";
        //    model.Consignee = "Sda";

        //    string a1 = JsonConvert.SerializeObject(model);

        //    string signStr = Signature.Compute(key, secret, timespan, a1);


        //    Dictionary<string, string> headers = new Dictionary<string, string>();
        //    headers.Add("key", key);
        //    headers.Add("timestamp", timespan.ToString());
        //    headers.Add("sign", signStr);
        //    headers.Add("version", "1.3.0.7");
        //    HttpUtil http = new HttpUtil();
        //    string result = http.HttpPostJson("" + host + "/api/ShippingAddress/Edit?userId=1&storeId=2", a1, headers);

        //    return result;

        //}


        //public string OrderConfirm(string userId, string storeId)
        //{
        //    RopOrderConfirm model = new RopOrderConfirm();

        //    model.StoreId = "BE9AE32C554D4942BE4A42FA48446210";
        //    model.Skus.Add(new OrderConfirmSkuModel {  Id})
        //    model.PhoneNumber = "15989287032";
        //    model.Address = "3123";
        //    model.AreaCode = "1";
        //    model.AreaName = "2";
        //    model.Consignee = "Sda";

        //    string a1 = JsonConvert.SerializeObject(model);

        //    string signStr = Signature.Compute(key, secret, timespan, a1);


        //    Dictionary<string, string> headers = new Dictionary<string, string>();
        //    headers.Add("key", key);
        //    headers.Add("timestamp", timespan.ToString());
        //    headers.Add("sign", signStr);
        //    headers.Add("version", "1.3.0.7");
        //    HttpUtil http = new HttpUtil();
        //    string result = http.HttpPostJson("" + host + "/api/ShippingAddress/Edit?userId=1&storeId=2", a1, headers);

        //    return result;

        //}
    }
}
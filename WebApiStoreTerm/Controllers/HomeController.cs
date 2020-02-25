
using LocalS.Service.Api.StoreTerm;
using Lumos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebApiStoreTerm.Controllers
{
    public class HomeController : Controller
    {

        [DllImport("BioVein.x64.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_MatchFeature([MarshalAs(UnmanagedType.LPArray)] byte[] featureDataMatch, [MarshalAs(UnmanagedType.LPArray)]  byte[] featureDataReg, byte RegCnt, byte flag, byte securityLevel, int[] diff, [MarshalAs(UnmanagedType.LPArray)] byte[] AIDataBuf, int[] AIDataLen);


        private string key = "test";
        private string secret = "6ZB97cdVz211O08EKZ6yriAYrHXFBowC";
        private long timespan = (long)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds;

        private string host = "";

        public static string GetQueryString(Dictionary<string, string> parames)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parames);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");  //签名字符串
            StringBuilder queryStr = new StringBuilder(""); //url参数
            if (parames == null || parames.Count == 0)
                return "";

            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key))
                {
                    queryStr.Append("&").Append(key).Append("=").Append(value);
                }
            }

            string s = queryStr.ToString().Substring(1, queryStr.Length - 1);

            return s;
        }


        public static string GetQueryString2(Dictionary<string, string> parames)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parames);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");  //签名字符串
            StringBuilder queryStr = new StringBuilder(""); //url参数
            if (parames == null || parames.Count == 0)
                return "";

            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key))
                {
                    queryStr.Append("&").Append(key).Append("=").Append(value);
                }
            }



            string s = queryStr.ToString().Substring(1, queryStr.Length - 1);

            return s;
        }

        private decimal GetDecimal(decimal d)
        {
            return Math.Round(d, 2);
        }

        Dictionary<string, string> model = new Dictionary<string, string>();


        public ActionResult Index()
        {

            string s = "EREAMXoMq83/////yo/YBBoAAABeeVVwcHlweXl5eUN5eSh5dW15Xnl5eXleXnkNeShweXl5eXl5KGd5UmzcWXl5eXl5eXl2eaZ0DfBZeXl5eXl5eXkod3lndnl5cHl5eXl5r3h5dXl5cHl5eXkoeXl5r3l5eXV5DXl5Z3l5eVJveSh1eWx58aF5eXnqG9ry8vIa8vLya3nwC/Dy8vIA8qF53PLy8nlOdSh0eXl5eSjqAE55eXkodHl5eSh5bXlJXutsylB5eaN4eVV5SS3yd6bcCABQeXl4eaAo8HpYefCheXlteXlrebUpdHnreHl5dXnlSnncUHl5cnl5yngoGqB56n15KHh5eXl67nhtedt3eXl5eXncDnR5KHjld3l5eXko8vJ6ea9synp5eXko8vJQeXkodeV5eXl58Gx5SXl5o3mfeXl564Z5eUl5eU7y8gh4KIZ1eXnueO7yJtjy8vKuenl563fZbnl5AeqGr3h5devyDXl5eXkoeuV33Cd9n3l8HHh5Q3mv8lDYdWx5eOt5eQ15KOwmadx3eXkZDUhweXkoeX15eXl5e/EA/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAREQAxcCqZWv/////Kj9gEGgAAACh5Z3lVeXl5eXl5KHl5eXV2gnl5eXl5eXl2eQ15eXt5VXl5eXl5bHlScKZ0yoZ5eXl5eXp52Xd5GAR1eXl5eXl5ecp6eU55eV55eXl5eXlDeXl9eXlSeXl5eXl5eXkNcHl5dXl5eXlweXl5o3V5KFp5dXnxU3l5XvA12PLy8hrq8vJ0KPAp7vLyUAzyoXnr8vKheQd4r3d5eXl5DfAIGnkNKMp3eXl5SXl2eU95Om3KdHl5o3QodnlOLfJ13E4dAFB4KHh5GRJQeWmv8qF5eXZ5KGt5GK5seZ95eXl3eevyeU9peXlyeXnKeK9Zmnnwd3l5d3l5ynhPeXAoUHh5eXl5eevbfXl55aV5eXl5ea/y8nl5eWQBeXl5ea/yCGl5eSh4nXl5eXnwcnlpeXmveYZ5eXnxdHl5n3l5TvDyAnkoqnh5eet47vKk8vLyUOV4eXDreOJueXkA2IZDeXl16xoNeXl5eSh4nXnrefCheXhbeXlVee7y8vJTbCh563l5cHmv8ntpKHh5eZ8Nn3l5eXl5W3l5eXl7SAD8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABERADE8jwsM/////8qP2AQaAAAAeXV5eV55eXl5eXlDeXlVVXl1eXl5eXl5DXl5cHkoAjN5eXl5eXl4eQRsFtgseXl5eXkoeHkNa3kNXnp5eXl5eXl5eXl5diR4KHl5eXl5eXl5Da96eVV5Xnl5eXl5eaPyeXlteXl5BAZ4eXlpeXYJIwBrAfDy8tryfeXy8vLy8vJ5DfDy8ndMGK5QeXl5eXkEeMosdXl5hnl5eSh4XnnKd3l5eRh3ee50eXl55XgHdygj8qJQeCh4eetuxX2meevyeXlDecryfaoma3k6eXl5VXmgQ3imqnh5Z3l5eW3KfWd53KF5eXx5eSh3T3l/edx3eXl5eXnZ8nR5dnkTdXl5eXnc8vJ9eSh5BHd5eXkoa3nKfXl5Usp4eXl58HV5yn15eXTreHl57mt5ecp3KPHy8lBueRh2eV7Kd+XyPtjy8vJ6eXl55VnldHl5XgGhDXp5eevyyXh5eXkoeK9s3KKG8Sh5eW95KHmv8vLadVV5edx3eV553FBR2Hl5eXnld9x3DXQoeW15eXl5ZPCheXl5DXl1eXl5eXcIAP8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";


            string s1 = "EREAMevSann/////yo/YBBoAAAB5eXl5Z3l5eXl5eQ15eXlVeX95eXl5eXUNeXkoUnlOdnl5eSh5VXl5r2lR24Z1eXB5eXl2eXlYea94eXkreXl5eXl5eHmsdyh5dXl5eXl1KHkN8Hp5Xnl5eXkoeXl56/J5eVJ5eXnc8nJ53Gsn8vIa2FANTtjy8vJ35fLy8rzy8nl56msoeAR4eU55eXl5eW15r3h5VXlkeXl5FnkNebV42XR1T2R57gh5bXZPefF3eVAYAPLIKHis8oDyeq942fJ5eTp5q/IsTlmaea94eXlk5QLyeKO8d3mveXl5dToIanmvUHl5VXl5KHigeXR5eRh5eXl5eeXydA1sdXl0eXl5eSjy8nd5JyGufXl5eXnwWax3eV6Uynh5eXncdHnKfXkBZ9x5eXmva3l5r3d5ovLy8gJ5T215ecp6r/IIr/Ly8nd5eV6vSqZQeHl5Cp95eXl5AKUnd3leeXlzeXl5JH55DShRbXl5bHl3eSh3eVV53Hp5eXgocHnleH95eeV6eXl5eXl1SA19eXmLKHR5eXbeSm15eXlw62sE8QAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";

            string s2 = "EREAMbWy3bb/////yo/YBBoAAAB5eW15eXl5eW15dXl5eXh5eXl5eXl1eXV4eSh4eXl5eXl5eHlteXkNeXl5eXl5KHl5bXl5eXl5eXl5eXl5eXlVbnl5eXl5BCgeeHl5AHl5eXledvFQTnl53IZ5mnl5eXmi8lDy8lB52XdeeK94KOzyCADy8fJ06nTcKCivUHl58gCi8vLy8gtReWx5eVx5KAMk8vIUdCh5eAR5eShyKHQEr3UNeXl5eXkoWV55ee56eXnreHl5r2t17m5OeXkodHl5ea+GefB6mnl53HV5eXkfctnyeJp5eGt5eXl5DQHYq3maeeV4eXl5eToAaZR56/KheXl5eXm1Am6CeU/weXJ5eXkECAQkfXlyeXl0eXl58XgBrnooeHl5d3l5yoYoU4t53Hl5eV55ylB463J5r/JTeSh5DfJseeqG5Rje8vICefJ0dgQY6V11DOXy8vJ4KOBQBNp4dnl5KPCGeQ0BfQTweWx5eXlkeSh5eVUooSh4eXl5fHlVeXl2rydpeXl5eW15dXl5eXl5dCh5eXl1KHp5eXlVeXZJeXkA/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";
            List<string> userm = new List<string>();
            userm.Add(s1);
            userm.Add(s2);


            byte[] regFeature = Convert.FromBase64String(s);
            byte[] aiFeature = new byte[512];
            int diff = 0;
            int aiBufLen = 0;

            for (var i = 0; i < s1.Length; i++)
            {
                byte[] matchFeature = Convert.FromBase64String(userm[i]);

                //  var ret = FV_MatchFeatureEx(matchFeature, regFeature, 3, aiFeature, 1, 0x03, 0x04, ref diff, aiBufLen);

                int[] diff2 = new int[1];
                byte[] AIDataBuf = new byte[matchFeature.Length];
                int[] AIDataLen = new int[1];
                var ret = FV_MatchFeature(matchFeature, regFeature, (byte)0x03, (byte)0x03, (byte)4, diff2, AIDataBuf, AIDataLen);
            }
            if (ConfigurationManager.AppSettings["custom:IsTest"] == null)
            {
                host = "http://localhost:18665/";
            }
            else
            {
                host = "http://demo.api.term.17fanju.com";
            }

            //host = "http://demo.api.term.17fanju.com";

            string machineId = "000000000000000";

            
            //Dictionary<string, string> parames = new Dictionary<string, string>();
            //parames.Add("machineId", machineId.ToString());
            //parames.Add("key", "http%3A%2F%2Fqr.weibo.cn%2Fg%2F3iv86t");
            //string signStr = Signature.Compute("test", "6ZB97cdVz211O08EKZ6yriAYrHXFBowC", 1573793412, Signature.GetQueryData(parames));

            //RopOrderReserve rop = new RopOrderReserve();
            //rop.MachineId = "000000000000000";
            //rop.ProductSkus.Add(new RopOrderReserve.ProductSku { Id = "0cea859026154bbd80c1e6f98d6d8853", Quantity = 2});
            //StoreTermServiceFactory.Order.Reserve(rop);

            //model.Add("获取机器初始数据", MachineInitData(machineId));
            //model.Add("预定商品", OrderReserve(machineId));
            // model.Add("生成支付二维码", OrderPayUrlBuild(machineId, "bc427a99e2e94e338f9bcea16d67a062"));

            //model.Add("登陆机器", MachineLogin(machineId,"a","b"));

            //HttpUtil http = new HttpUtil();

            //http.HttpUploadFile(host+ "/Api/Machine/UpLoadLog", "d:\\a.txt");


            OwnUploadFingerVeinData();

            return View(model);
        }



        //public string MachineInitData(string machineId)
        //{
        //    Dictionary<string, string> parames = new Dictionary<string, string>();
        //    parames.Add("machineId", machineId.ToString());
        //    string signStr = Signature.Compute(key, secret, timespan, Signature.GetQueryData(parames));

        //    Dictionary<string, string> headers = new Dictionary<string, string>();
        //    headers.Add("key", key);
        //    headers.Add("timestamp", timespan.ToString());
        //    headers.Add("sign", signStr);
        //    HttpUtil http = new HttpUtil();
        //    string result = http.HttpGet("" + host + "/api/Machine/InitData?machineId=" + machineId, headers);

        //    return result;

        //}

        //public string OrderReserve(string machineId)
        //{

        //    RopOrderReserve pms = new RopOrderReserve();
        //    pms.MachineId = machineId;
        //    pms.Skus.Add(new RopOrderReserve.Sku() { Id = "3cbddffaf84148279bd91551db238ca3", Quantity = 1 });
        //    pms.Skus.Add(new RopOrderReserve.Sku() { Id = "44b2d4ae88e24b76a8a744f582214513", Quantity = 1 });
        //    pms.Skus.Add(new RopOrderReserve.Sku() { Id = "e8bb8685ed8d483fa60225fc750f3a79", Quantity = 1 });


        //    string a1 = JsonConvert.SerializeObject(pms);

        //    string signStr = Signature.Compute(key, secret, timespan, a1);

        //    Dictionary<string, string> headers1 = new Dictionary<string, string>();
        //    headers1.Add("key", key);
        //    headers1.Add("timestamp", (timespan.ToString()).ToString());
        //    headers1.Add("sign", signStr);

        //    HttpUtil http = new HttpUtil();
        //    string respon_data4 = http.HttpPostJson("" + host + "/api/Order/Reserve", a1, headers1);

        //    return respon_data4;

        //}

        //public string OrderPayUrlBuild(string machineId, string orderId)
        //{

        //    RopOrderPayUrlBuild pms = new RopOrderPayUrlBuild();
        //    pms.MachineId = machineId;
        //    pms.PayWay = PayWay.Wx;
        //    pms.OrderId = orderId;


        //    string a1 = JsonConvert.SerializeObject(pms);

        //    string signStr = Signature.Compute(key, secret, timespan, a1);

        //    Dictionary<string, string> headers1 = new Dictionary<string, string>();
        //    headers1.Add("key", key);
        //    headers1.Add("timestamp", (timespan.ToString()).ToString());
        //    headers1.Add("sign", signStr);

        //    HttpUtil http = new HttpUtil();
        //    string respon_data4 = http.HttpPostJson("" + host + "/api/Order/PayUrlBuild", a1, headers1);

        //    return respon_data4;

        //}

        //public string MachineLogin(string machineId, string userName, string password)
        //{

        //    RopMachineLogin pms = new RopMachineLogin();
        //    pms.MachineId = machineId;
        //    pms.UserName = userName;
        //    pms.Password = password;


        //    string a1 = JsonConvert.SerializeObject(pms);

        //    string signStr = Signature.Compute(key, secret, timespan, a1);

        //    Dictionary<string, string> headers1 = new Dictionary<string, string>();
        //    headers1.Add("key", key);
        //    headers1.Add("timestamp", (timespan.ToString()).ToString());
        //    headers1.Add("sign", signStr);

        //    HttpUtil http = new HttpUtil();
        //    string respon_data4 = http.HttpPostJson("" + host + "/api/Machine/Login", a1, headers1);

        //    return respon_data4;

        //}

        public string OwnUploadFingerVeinData()
        {

            LocalS.Service.Api.Account.RopUploadFingerVeinData pms = new LocalS.Service.Api.Account.RopUploadFingerVeinData();

            byte[] s = new byte[5];
            s[0] = 1;
            s[1] = 0;
            s[2] = 1;
            s[3] = 0;
            s[4] = 1;
            pms.VeinData = "dasdad";


            string a1 = JsonConvert.SerializeObject(pms);

            string signStr = Signature.Compute(key, secret, timespan, a1);

            Dictionary<string, string> headers1 = new Dictionary<string, string>();
            headers1.Add("key", key);
            headers1.Add("timestamp", (timespan.ToString()).ToString());
            headers1.Add("sign", signStr);

            HttpUtil http = new HttpUtil();
            string respon_data4 = http.HttpPostJson("" + host + "/api/Own/UploadFingerVeinData", a1, headers1);

            return respon_data4;

        }
    }
}
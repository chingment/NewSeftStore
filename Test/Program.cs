using EasemobSdk;
using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq.MqByRedis;
using LocalS.BLL.Task;
using LocalS.Entity;
using LocalS.Service;
using LocalS.Service.Api.Merch;
using LocalS.Service.Api.StoreApp;
using LocalS.Service.Api.StoreTerm;
using log4net;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using MyPushSdk;
using MyWeiXinSdk;
using Newtonsoft.Json.Linq;
using NPinyin;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TgPaySdk;
using XrtPaySdk;

namespace Test
{
    class Program
    {


        public static sbyte[] a(byte[] myByte)
        {
            sbyte[] mySByte = new sbyte[myByte.Length];

            for (int i = 0; i < myByte.Length; i++)
            {
                if (myByte[i] > 127)
                    mySByte[i] = (sbyte)(myByte[i] - 256);
                else
                    mySByte[i] = (sbyte)myByte[i];
            }

            return mySByte;
        }
        public static string UrlEncode1(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (HttpUtility.UrlEncode(c.ToString()).Length > 1)
                {
                    sb.Append(HttpUtility.UrlEncode(c.ToString()).ToUpper());
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string RemoveNotLetter(string title)
        {
            var listSign = new List<string> { "|", "'", ",", "&", ".", "!" };
            var notLetter = Regex.Split(title, @"[a-zA-Z]/[0-9]", RegexOptions.IgnoreCase).Where(r => r.Trim() != string.Empty).ToList();
            var newLetter = new List<string>();

            for (int i = notLetter.Count - 1; i >= 0; i--)
            {
                if (notLetter[i].Trim().Length == 0)
                {
                    notLetter.RemoveAt(i);
                    continue;
                }

                if (notLetter[i].Trim().Length > 1)
                {
                    for (int j = 0; j < notLetter[i].Trim().Length; j++)
                    {
                        newLetter.Add(notLetter[i].Trim().Substring(j, 1));
                    }
                    notLetter.RemoveAt(i);
                }
            }

            notLetter.AddRange(newLetter);
            foreach (string sign in notLetter)
            {
                if (sign.Trim().Length == 0) continue;

                if (!listSign.Contains(sign.Trim()))
                {
                    title = title.Replace(sign.Trim(), "");
                }
            }
            return title;
        }

        public static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public static DateTime TicksToDate(string time)
        {
            return new DateTime((Convert.ToInt64(time) * 10000) + 621355968000000000).AddHours(8);
        }

        public static string GetHourText(double hour)
        {
            if (hour <= 0)
                return "0";

            return hour.ToString("0.00");
        }

        public static DateTime TicksToDate(long time)
        {
            return new DateTime((Convert.ToInt64(time) * 10000) + 621355968000000000).AddHours(8);

        }

        public class DateValuePoint
        {
            public List<long> DataTime { get; set; }

            public List<int> DataValue { get; set; }
        }

        static void Main(string[] args)
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.Append("test");
            sb1.Append("6ZB97cdVz211O08EKZ6yriAYrHXFBowC");
            sb1.Append("1620465964GpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKYGpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKY");
            sb1.Append("{\"device_id\":\"202101200206\",\"data_format\":\"slot\",\"cabinet_id\":\"dsx01n01\",\"is_need_detail\":false}");

            var sortStr = string.Concat(sb1.ToString().OrderBy(c1 => c1));

            var input = Encoding.UTF8.GetBytes(sortStr);

            var hash = SHA256Managed.Create().ComputeHash(input);

            StringBuilder sb2 = new StringBuilder();
            foreach (byte b in hash)
                sb2.Append(b.ToString("x2"));

            var input2 = Encoding.UTF8.GetBytes(sb2.ToString());

            var output = Convert.ToBase64String(input2);

            //var aaaa2 = Math.Round(17.272972972972973,2);

            // string st1 = "{\"dataTime\":[1616781021,1616781081,1616781141,1616781201,1616781261,1616781321,1616781381,1616781441,1616781501,1616781561,1616781621,1616781681,1616781741,1616781801,1616781861,1616781921,1616781981,1616782041,1616782101,1616782161,1616782221,1616782281,1616782341,1616782401,1616782461,1616782521,1616782581,1616782641,1616782701,1616782761,1616782821,1616782881,1616782941,1616783001,1616783061,1616783121,1616783181,1616783241,1616783301,1616783361,1616783421,1616783481,1616783541,1616783601,1616783661,1616783721,1616783781,1616783841,1616783901,1616783961,1616784021,1616784081,1616784141,1616784201,1616784261,1616784321,1616784381,1616784441,1616784501,1616784561,1616784621,1616784681,1616784741,1616784801,1616784861,1616784921,1616784981,1616785041,1616785101,1616785161,1616785221,1616785281,1616785341,1616785401,1616785461,1616785521,1616785581,1616785641,1616785701,1616785761,1616785821,1616785881,1616785941,1616786001,1616786061,1616786121,1616786181,1616786241,1616786301,1616786361,1616786421,1616786481,1616786541,1616786601,1616786661,1616786721,1616786781,1616786841,1616786901,1616786961,1616787021,1616787081,1616787141,1616787201,1616787261,1616787321,1616787381,1616787441,1616787501,1616787561,1616787621,1616787681,1616787741,1616787801,1616787861,1616787921,1616787981,1616788041,1616788101,1616788161,1616788221,1616788281,1616788341,1616788401,1616788461,1616788521,1616788581,1616788641,1616788701,1616788761,1616788821,1616788881,1616788941,1616789001,1616789061,1616789121,1616789181,1616789241,1616789301,1616789361,1616789421,1616789481,1616789541,1616789601,1616789661,1616789721,1616789781,1616789841,1616789901,1616789961,1616790021,1616790081,1616790141,1616790201,1616790261,1616790321,1616790381,1616790441,1616790501,1616790561,1616790621,1616790681,1616790741,1616790801,1616790861,1616790921,1616790981,1616791041,1616791101,1616791161,1616791221,1616791281,1616791341,1616791401,1616791461,1616791521,1616791581,1616791641,1616791701,1616791761,1616791821,1616791881,1616791941,1616792001,1616792061,1616792121,1616792181,1616792241,1616792301,1616792361,1616792421,1616792481,1616792541,1616792601,1616792661,1616792721,1616792781,1616792841,1616792901,1616792961,1616793021,1616793081,1616793141,1616793201,1616793261,1616793321,1616793381,1616793441,1616793501,1616793561,1616793621,1616793681,1616793741,1616793801,1616793861,1616793921,1616793981,1616794041,1616794101,1616794161,1616794221,1616794281,1616794341,1616794401,1616794461,1616794521,1616794581,1616794641,1616794701,1616794761,1616794821,1616794881,1616794941,1616795001,1616795061,1616795121,1616795181,1616795241,1616795301,1616795361,1616795421,1616795481,1616795541,1616795601,1616795661,1616795721,1616795781,1616795841,1616795901,1616795961,1616796021,1616796081,1616796141,1616796201,1616796261,1616796321,1616796381,1616796441,1616796501,1616796561,1616796621,1616796681,1616796741,1616796801,1616796861,1616796921,1616796981,1616797041,1616797101,1616797161,1616797221,1616797281,1616797341,1616797401,1616797461,1616797521,1616797581,1616797641,1616797701,1616797761,1616797821,1616797881,1616797941,1616798001,1616798061,1616798121,1616798181,1616798241,1616798301,1616798361,1616798421,1616798481,1616798541,1616798601,1616798661,1616798721,1616798781,1616798841,1616798901,1616798961,1616799021,1616799081,1616799141,1616799201,1616799261,1616799321,1616799381,1616799441,1616799501,1616799561,1616799621,1616799681,1616799741,1616799801,1616799861,1616799921,1616799981,1616800041,1616800101,1616800161,1616800221,1616800281,1616800341,1616800401,1616800461,1616800521,1616800581,1616800641,1616800701,1616800761,1616800821,1616800881,1616800941,1616801001,1616801061,1616801121,1616801181,1616801241,1616801301,1616801361,1616801421,1616801481,1616801541,1616801601,1616801661,1616801721,1616801781,1616801841,1616801901,1616801961,1616802021,1616802081,1616802141,1616802201,1616802261,1616802321,1616802381],\"dataValue\":[16,20,19,10,10,16,14,17,18,20,20,19,19,19,18,17,17,19,18,18,18,18,13,17,20,17,19,12,18,15,18,20,20,18,16,16,20,9,22,20,0,0,0,16,21,16,11,16,19,20,17,17,17,18,16,18,18,16,17,20,19,17,19,20,21,17,20,19,19,17,17,18,21,11,19,16,18,20,14,14,19,21,8,20,21,20,19,19,19,20,17,18,19,18,18,19,18,13,15,16,16,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,19,15,20,19,19,18,19,18,18,17,18,17,9,19,20,19,18,17,18,18,18,17,19,17,17,18,18,17,18,18,17,18,18,17,17,17,18,18,17,18,17,17,17,17,16,17,17,18,19,17,18,18,17,18,18,19,20,16,18,17,18,17,18,19,17,14,18,18,18,17,16,16,16,18,17,16,17,17,17,18,18,16,17,16,16,17,18,18,17,17,17,17,18,17,17,17,18,17,17,17,16,17,16,15,16,16,16,16,16,17,15,17,17,18,18,17,17,17,17,17,17,17,17,17,13,17,17,17,17,17,17,17,17,16,17,16,17,16,17,17,18,18,18,18,18,17,18,18,19,19,19,20,19,21,19,19,18,18,20,19,16,13,15,16,15,14,16,15,14,15,15,17,17,18,17,17,16,0,0,16,13,18,17,16,7,16,16,15,17,18,18,14,16,17,17,16,15,15,16,16,17,15,16,14,15,16,19]}";

            //var sss = st1.ToJsonObject<DateValuePoint>();

            var s22 = TicksToDate(16168015410000);

            bool s77 = Lumos.CommonUtil.GetTimeSpan("2017-2-18 1:00:00", "23:00", "1:30");

            // double scoreRatio =3 / 22;
            double r = Math.Round((Convert.ToDouble(21) / Convert.ToDouble(22)), 2) * 100;
            var scoreRatio = Convert.ToInt32(r);
            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            var getAccessTokenRequest = new SenvivSdk.GetAccessTokenRequest("\"w8RlypEyYP1g6jctLFI3bNjS9bJn0bf9f+KSm9p94S9HPS1M6ij8bnCQJY7Epcg1Nacx0i51L2sHnpnWkUnPP9FXO9vDkG6HPo20BLbURis=\"", new { deptid = "32" });

            var result = api.DoPost(getAccessTokenRequest);


            StringBuilder sb = new StringBuilder();
            sb.Append("{\"touser\":\"on0dM51JLVry0lnKT4Q8nsJBRXNs\",");
            sb.Append("\"template_id\":\"GpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKY\",");
            sb.Append("\"url\":\"http://health.17fanju.com/#/report/month/monitor?rptId=a83045abb4444b9089edefcc195be998\", ");
            sb.Append("\"data\":{");
            sb.Append("\"first\":{ \"value\":\"张三您好，您的最近一期健康报告已生成，详情如下。\",\"color\":\"#173177\" },");
            sb.Append("\"keyword1\":{ \"value\":\"2014年7月21日 18:36\",\"color\":\"#173177\" },");
            sb.Append("\"keyword2\":{ \"value\":\"各项指标均正常，请进一步保持。总体评分4分\",\"color\":\"#173177\" },");
            sb.Append("\"remark\":{ \"value\":\"感谢您的支持，如需查看往期报告信息请点击\",\"color\":\"#173177\"}");
            sb.Append("}}");

            WxAppInfoConfig config = new WxAppInfoConfig();
            config.AppId = "wxf0d98b28bebd0c82";
            config.AppSecret = "fee895c9923da26a4d42d9c435202b37";

            string access_token = result.Data.Data.access_token;

            WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(access_token, WxPostDataType.Text, sb.ToString());
            WxApi c = new WxApi();

            c.DoPost(templateSend);

            List<decimal> smTags2 = new List<decimal>();
            smTags2.Add(1200m);
            smTags2.Add(3600m);

            string cccc1 = smTags2.Select(m => Math.Round(m / 3600m, 2)).ToJsonString();//

            List<DateTime> smTags1 = new List<DateTime>();
            smTags1.Add(DateTime.Now);
            smTags1.Add(DateTime.Now);

            string cccc = smTags1.Select(m => m.ToUnifiedFormatDate()).ToJsonString();//


            List<string> smTags = new List<string>();

            smTags.Add("A");
            smTags.Add("A2");
            smTags.Add("A3");
            smTags.Add("AG");
            smTags.Add("A2");

            var quary = smTags
            .GroupBy(s => s)
            .Select(group => new
            {
                name = group.Key,
                Count = group.Count()
            });

            //   var smTags_Count =( smTags.GroupBy(s => s).OrderByDescending(s => s.Count()).ToList();

            string asss = quary.OrderByDescending(m => m.Count).ToJsonString();

            var SmSdsmsc = SvDataJdUtil.GetSmSdsmsc(6906);

            string a = GetHourText(1.9393333);


            //DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            //long lTime = long.Parse("1615914256");
            //TimeSpan toNow = new TimeSpan(lTime);
            //DateTime dtResult = dtStart.Add(toNow);


            //var s = TicksToDate("1615914256000");

            //var s2 = TicksToDate("1615941148000");
            //1615914256000
            //1615941148000

            //var list = new List<SenvivSdk.UserListResult.DataModel>();

            //SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            //int page = 1;
            //int size = 10;
            //string token = "\"w8RlypEyYP1g6jctLFI3bNjS9bJn0bf9f+KSm9p94S9HPS1M6ij8bnCQJY7Epcg1fAweHovdeMHEuIhsUuZYugG3vSHJadHnarEkXBFKWAg=\"";
            //var userListRequest = new SenvivSdk.UserListRequest(token, new { deptid = "32", size = size, page = page });
            //var result = api.DoPost(userListRequest);
            //if (result.Result == ResultType.Success)
            //{
            //    var data = result.Data;
            //    if (data != null)
            //    {
            //        int count = data.Data.count;
            //        int pageCount = (count + size - 1) / size;

            //        list.AddRange(data.Data.data);

            //        if (pageCount >= 2)
            //        {
            //            for (var i = 2; i <= pageCount; i++)
            //            {
            //                var userListRequest2 = new SenvivSdk.UserListRequest(token, new { deptid = "32", size = size, page = i });
            //                var result2 = api.DoPost(userListRequest2);
            //                if (result2.Result == ResultType.Success)
            //                {
            //                    var data2 = result2.Data;
            //                    if (data2 != null)
            //                    {
            //                        list.AddRange(data2.Data.data);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}




            //DateTime dt = DateTime.Parse("0001-01-01T00:00:00+08:00");
            ////1004B23B4DFF_1615332079038
            ////1004B23B4DFF_1615332079038
            //SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();
            //var userListRequest2 = new SenvivSdk.ReportDetailListRequest("\"w8RlypEyYP1g6jctLFI3bNjS9bJn0bf9f+KSm9p94S9HPS1M6ij8bnCQJY7Epcg1jddwMgWP8qiJ3WE+h+sBf0ivW39asBWM9y2ooYwxx8Y=\"", new { deptid = "32", userid = "321x847d11280EA34A", size = 1, page = 1 });
            //var result2 = api.DoPost(userListRequest2);
            //if (result2.Result == ResultType.Success)
            //{

            //}

            //SdkFactory.Senviv.GetUserList();

            //var loginRequest = new SenvivSdk.LoginRequest("", new { name = "qxtadmin", pwd = "zkxz123" });

            //SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            //var result = api.DoPost(loginRequest);

            //string token = "\"w8RlypEyYP1g6jctLFI3bNjS9bJn0bf9f+KSm9p94S9HPS1M6ij8bnCQJY7Epcg1HubkVijncfUY//nxrv9MTYYYEibRRgmB3cPu5p/8+Xo=\"";
            //string token = result.Data.Data.AuthorizationCode;

            //var deptSubordinateListRequest = new SenvivSdk.DeptSubordinateListRequest(token, "{\"deptid\":\"32\"}");

            //32

            //var result3 = api.DoPost(deptSubordinateListRequest);


            //var getAccessTokenRequest = new SenvivSdk.GetAccessTokenRequest(token, new { deptid = "32" });


            //var result2 = api.DoPost(getAccessTokenRequest);



            //var userListRequest = new SenvivSdk.UserListRequest(token, new { deptid = "32", size = "10", page = "1" });


            //var result5 = api.DoPost(userListRequest);

            //string data = "{\"name\":\"qxtadmin\",\"pwd\":\"zkxz123\"}";


            // string accessToken = "42_wgKB4dYOUKxUlrEJgnBPZH4J2_96l_P7CWtqGv2aHqkweKJPWlprEqFMV35RlD1cOFbHNa09zOLg9nPY2mm4_lcv-mPEsKXEeSgK99orWf3ZPrgBzr_4cDVNHxhGWybvroeMs6w8EwNsy7uMDUSjAGAOFD";

            //var openIds = OAuthApi.GetUserOpenIds(accessToken);


            //foreach (var openId in openIds)
            //{
            //    var info = OAuthApi.GetUserInfoByApiToken(accessToken, openId).ToJsonString();
            //    Console.WriteLine(info);
            //}

            string openId = "on0dM51JLVry0lnKT4Q8nsJBRXNs";
            string first = "您好，您的净水设备租约即使到期";
            string keyword1 = "123";
            string keyword2 = "智能净水器";
            string keyword3 = "2016年6月23日到期";
            string remark = "请尽快充值续费，以免影响您的设备使用！";
            //on0dM51JLVry0lnKT4Q8nsJBRXNs


            //string mp_AppId = "wx80caad9ea41a00fc";
            //string mp_PagePath = "pages/orderconfirm/orderconfirm?skus=%5B%7B%22cartId%22%3A0%2C%22id%22%3A%22722b4d565604489fa1f40c548e0bc114%22%2C%22quantity%22%3A1%2C%22shopMode%22%3A1%2C%22shopMethod%22%3A5%2C%22shopId%22%3A%220%22%7D%5D&shopMethod=5&action=rentfee&pOrderId=610464520210226113146434";
            //StringBuilder sb = new StringBuilder();
            //sb.Append("{\"touser\":\"" + openId + "\",");
            //sb.Append("\"template_id\":\"xCwBMd_h0ekopGsYIj7fpi7-qAY54qbuROTzmS7odhQ\",");
            //sb.Append("\"url\":\"\", ");
            //sb.Append("\"miniprogram\":{");
            //sb.Append("\"appid\":\""+ mp_AppId + "\",");
            //sb.Append("\"pagepath\":\""+ mp_PagePath + "\"");
            //sb.Append("},");
            //sb.Append("\"data\":{");
            //sb.Append("\"first\":{ \"value\":\"" + first + "。\",\"color\":\"#173177\" },");
            //sb.Append("\"keyword1\":{ \"value\":\"" + keyword1 + "\",\"color\":\"#173177\" },");
            //sb.Append("\"keyword2\":{ \"value\":\"" + keyword2 + "\",\"color\":\"#173177\" },");
            //sb.Append("\"keyword3\":{ \"value\":\"" + keyword3 + "\",\"color\":\"#FF3030\" },");
            //sb.Append("\"remark\":{ \"value\":\""+ remark + "\",\"color\":\"#173177\"}");
            //sb.Append("}}");


            //WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(accessToken, WxPostDataType.Text, sb.ToString());
            //WxApi c = new WxApi();

            //c.DoPost(templateSend);




            Console.ReadLine();
        }



        private static sbyte[] SendCmd(sbyte[] frameCmd, sbyte[] frameCmdParms)
        {
            sbyte[] frameHead = new sbyte[] { 0x24 };
            sbyte[] frameEnd = new sbyte[] { 0x0D, 0x0A };
            if (frameCmd == null)
            {
                frameCmd = new sbyte[] { };
            }

            if (frameCmdParms == null)
            {
                frameCmdParms = new sbyte[] { };
            }

            //长度码=命令码字节数+命令参数字节数+校验码字节数
            sbyte framLengthCode = Convert.ToSByte(frameCmd.Length + frameCmdParms.Length + 1);

            //int xorAndLength = 1 + i_framLenth;

            sbyte[] xorAnd = new sbyte[1 + frameCmd.Length + frameCmdParms.Length];

            xorAnd[0] = framLengthCode;

            for (int i = 0; i < frameCmd.Length; i++)
            {
                xorAnd[i + 1] = frameCmd[i];
            }

            for (int i = 0; i < frameCmdParms.Length; i++)
            {
                xorAnd[frameCmd.Length + 1] = frameCmdParms[i];
            }


            sbyte framXorCode = 0;
            for (int i = 0; i < xorAnd.Length; i++)
            {
                framXorCode ^= xorAnd[i];
            }

            List<sbyte> frame = new List<sbyte>();

            frame.AddRange(frameHead);
            frame.Add(framLengthCode);
            frame.AddRange(frameCmd);
            frame.AddRange(frameCmdParms);
            frame.Add(framXorCode);
            frame.AddRange(frameEnd);

            return frame.ToArray();
        }


    }
}

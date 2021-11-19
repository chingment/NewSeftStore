using EasemobSdk;
using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq.MqByRedis;
using LocalS.BLL.Push;
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


        public class Stock
        {
            public string StoreName
            {
                get; set;
            }
            public string ShopName
            {
                get; set;
            }

            public string DeviceCode
            {
                get; set;
            }

            public string SkuName
            {
                get; set;
            }

            public int Quantity
            {
                get; set;
            }

        }


        public static string GetSign(string app_id, string app_key, string secret, long timespan, string data)
        {
            var sb = new StringBuilder();

            sb.Append(app_id);
            sb.Append(app_key);
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


        static void Main(string[] args)
        {
            double s = double.Parse("114.35808563232422");
            var senvivUsers = SdkFactory.Senviv.GetBoxList();
            //"w8RlypEyYP1g6jctLFI3bNjS9bJn0bf9f+KSm9p94S9HPS1M6ij8bnCQJY7Epcg13C0pSC2kyEYsSAyVACHZYTqsWr3w9CnC55XasyUWJC0="

            string accessToken = SdkFactory.Senviv.GetApiAccessToken();
            WxAppInfoConfig config = new WxAppInfoConfig();
            config.AppId = "wxf0d98b28bebd0c82";
            config.AppSecret = "fee895c9923da26a4d42d9c435202b37";
            string sss = SdkFactory.Senviv.GetWxPaAccessToken("32");
            //config.AppId = "wxc6e80f8c575cf3f5";
            //config.AppSecret = "fee895c9923da26a4d42d9c435202b37";
            config.TrdAccessToken = "w8RlypEyYP1g6jctLFI3bNjS9bJn0bf9f+KSm9p94S9HPS1M6ij8bnCQJY7Epcg13C0pSC2kyEYsSAyVACHZYbodUOlsik14ZSQtyb3gJ6E=".Replace("\"", "");

            var configParams = SdkFactory.Wx.GetJsApiConfigParams(config, "dasd");

            var result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", configParams);

            //OAuthApi.GetUserOpenIds(config.TrdAccessToken);

            //string openId = "on0dM51JLVry0lnKT4Q8nsJBRXNs";
            //string first = "您好，您的净水设备租约即使到期";
            //string keyword1 = "123";
            //string keyword2 = "智能净水器";
            //string keyword3 = "2016年6月23日到期";
            //string remark = "请尽快充值续费，以免影响您的设备使用！";
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

using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq.MqByRedis;
using LocalS.Entity;
using LocalS.Service.Api.Merch;
using LocalS.Service.Api.StoreApp;
using log4net;
using Lumos;
using Lumos.Redis;
using NPinyin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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


        static void Main(string[] args)
        {
            log.InfoFormat("程序开始");

            XrtPayInfoConfg payInfo = new XrtPayInfoConfg();

            payInfo.Mch_id = "86144035999J054";
            payInfo.Key = "FBC8B4396940E0969048767F53CB649A";
            payInfo.PayResultNotifyUrl = "http://api.m.17fanju.com/Api/Order/PayResultNotifyByWx";


            //BizFactory.Order.BuildPayParams(GuidUtil.Empty(),new LocalS.BLL.Biz.RopOrderBuildPayParams {   })

            //XrtPayUtil xrtPayUtil = new XrtPayUtil(payInfo);
            //wx969a817779af7b53
            //ooHy45d93BhpXkDlyE-mdpHdE_Hs
            //xrtPayUtil.WxPayBuildByJs("wx969a817779af7b53", "ooHy45d93BhpXkDlyE-mdpHdE_Hs", "610010720200115143310372", "1", "测试支付", "", "127.0.0.1", "", DateTime.Now.AddMinutes(5).ToString("yyyyMMddHHmmss"));
            //SdkFactory.XrtPay.PayBuildQrCode(payInfo, E_OrderPayCaller.WxByNt, "", "", "", "610010720200115143310369", 0.01m, "", "127.0.0.1", "测试支付", DateTime.Now.AddMinutes(5));
            //SdkFactory.XrtPay.PayQuery(payInfo, "610010720200115143310368");
            //string PinYinName = CommonUtil.GetPingYin("格力高百醇（草莓味）");
            //string PinYinIndex = CommonUtil.GetPingYinIndex("格力高百醇（草莓味）");



            //List<string[]> s = new List<string[]>();
            //s.Add
            //[[1],[1,7],[17],[17,21]]
            //            string[,] s = new string[2, 5];


            //string extension = Path.GetExtension("Dsadad/dsada.jpg");

            //MerchServiceFactory.Order.GetSonStatus(E_OrderDetailsChildSonStatus.SendPickupCmd);

            // { "status":100,"channelId":"WX","state":"4","settlementChannel":"038","payTime":"2019-12-08 17:00:43","payoffType":null,"lowOrderId":"6100054201910231627169351",
            //"sign":"D20589C3F539B0FC9D2BC4A48B6426A5","message":"找不到交易","payMoney":"0.01"
            //    ,"upOrderId":"91203600163013136384","payType":"0","account":"13974747474","openid":null,"openId":null}

            //  15589D446B76FD679E3A6D7D34FF9A56
            //{"status":100,
            //"channelId":"WX",
            //"state":"4",
            //"settlementChannel":"038",
            //"payTime":"2019-12-08 17:00:43",
            //"payoffType":null,
            //"lowOrderId": "6100054201910231627169351",
            //"sign":"D20589C3F539B0FC9D2BC4A48B6426A5","message":"找不到交易",
            //"payMoney":"0.01","upOrderId":"91203600163013136384",
            //"payType":"0","account":"13974747474","openid":null,"openId":null}
            //List<PayOption> payOptions = new List<PayOption>();

            //List<E_OrderPayWay> payWay = new List<E_OrderPayWay>();
            //payWay.Add(E_OrderPayWay.Wx);

            //payOptions.Add(new PayOption { Caller = E_OrderPayCaller.WxByBuildQrCode, Partner = E_OrderPayPartner.Wx, SupportWays = payWay });
            //payOptions.Add(new PayOption { Caller = E_OrderPayCaller.ZfbByBuildQrCode, Partner = E_OrderPayPartner.Zfb, SupportWays = payWay });

            //string a22 = payOptions.ToJsonString();


            // [{"caller":10,"partner":1,"supportWays":[1]},{"caller":20,"partner":2,"supportWays":[2]}]


            //  [{"caller":90,"partner":91,"supportWays":[2,1]}]

            //TgPayInfoConfg config = new TgPayInfoConfg();
            //config.PayResultNotifyUrl = "http://api.m.17fanju.com/Api/Order/PayResultNotifyByTg";
            //config.Account = "15675830166";
            //config.Key = "ffd50c4bf658b619c53e246926af8e48";
            ////config.Account = "13974747474";
            ////config.Key = "5f61d7f65b184d19a1e006bc9bfb6b2f";
            //TgUtil tgUtil = new TgUtil(config);
            //decimal amount = 0.01m;
            //tgUtil.AllQrcodePay("6100054201911231627169353", amount.ToString("#0.00"), "自助商品", "867184037089830");
            //tgUtil.AllQrcodePay("6100054201911231627169357", "0.01", "自助商品", "867184037089830");
            ////tgUtil.OrderQuery("6100054201910231627169351我");
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //dic.Add("status", "100");
            //dic.Add("channelId", "WX");
            //dic.Add("state", "4");
            //dic.Add("settlementChannel", "038");
            //dic.Add("payTime", "2019-12-08 17:00:43");
            //dic.Add("lowOrderId", "6100054201910231627169351");
            //dic.Add("payMoney", "0.01");
            //dic.Add("upOrderId", "91203600163013136384");
            //dic.Add("payType", "0");
            //dic.Add("account", "13974747474");
            //dic.Add("message", "找不到交易");
            //string c = tgUtil.GetSign(dic);
            //  api.OrderQuery("610005420191023162716933");
            //PushService.SendUpdateMachineHomeLogo("1104a89792cdeb53a97", "dsad");

            //string s23 = System.Web.HttpUtility.UrlDecode("%7B%22appinfo%22%3A%7B%22appChannel%22%3A%22com.uplink.selfstore%22%2C%22appId%22%3A%221%22%2C%22appVersion%22%3A%221.0.0.0%22%7D%2C%22deviceinfo%22%3A%7B%22deviceDensity%22%3A%221.0%22%2C%22deviceId%22%3A%224675a39a-d9b5-420b-bca5-45b767346088%22%2C%22deviceLocale%22%3A%22en%22%2C%22deviceMacAddr%22%3A%2202%3A00%3A00%3A00%3A00%3A00%22%2C%22deviceModel%22%3A%22Android+SDK+built+for+x86%22%2C%22deviceOsVersion%22%3A%227.1%22%2C%22devicePlatform%22%3A%22Android%22%2C%22deviceScreen%22%3A%221080*1872%22%7D%2C%22networkinfo%22%3A%7B%22ipAddr%22%3A%22fe80%3A%3A5054%3Aff%3Afe12%3A3456%25eth0%22%2C%22wifi%22%3Afalse%7D%7D");
            //string s = UrlEncode1("http://demo.api.term.17fanju.com");
            //var b = RedisManager.Db.HashScan("aaa", "a1*");
            //var b1 = b.ToList();

            //foreach (var b2 in b1)
            //{
            //    string b3 = b2.Name;
            //}
            //RedisManager.Db.HashScan("aaa", "a1*");

            // var s1 = new { a = 1, b = 2 };
            //.Db.HashSetAsync("aaa","a1xxx", Newtonsoft.Json.JsonConvert.SerializeObject(s1), StackExchange.Redis.When.Always);
            //RedisManager.Db.HashSetAsync("aaa", "a2ss", Newtonsoft.Json.JsonConvert.SerializeObject(s1), StackExchange.Redis.When.Always);
            //RedisManager.Db.HashSetAsync("aaa", "b2ww", Newtonsoft.Json.JsonConvert.SerializeObject(s1), StackExchange.Redis.When.Always);


            // var s = CabineRowColLayoutModel.Convert("4,2,3,4,7");

            //bool a1 = CommonUtil.IsNumber("1312331333x13133123");

            //StringBuilder sql = new StringBuilder();
            //sql.Append(" select a1.datef,isnull(sumCount,0) as sumCount, isnull(sumTradeAmount,0) as  sumTradeAmount from (  ");
            //for (int i = 0; i < 7; i++)
            //{
            //    string datef = DateTime.Now.AddDays(double.Parse((-i).ToString())).ToUnifiedFormatDate();

            //    sql.Append(" select '" + datef + "' datef union");
            //}
            //sql.Remove(sql.Length - 5, 5);

            //sql.Append(" ) a1 left join ");
            //sql.Append(" (    select datef, sum(sumCount),sum(sumTradeAmount) from ( select CONVERT(varchar(100),TradeTime, 23) datef,count(*) as sumCount ,sum(TradeAmount) as sumTradeAmount from RptOrder   where  merchId='d17df2252133478c99104180e8062230' and DateDiff(dd, TradeTime, getdate()) <= 7  group by TradeTime ) tb  group by datef ) b1 ");
            //sql.Append(" on  a1.datef=b1.datef  ");
            //sql.Append(" order by a1.datef desc  ");


            //string a = sql.ToString();

            //sbyte[] orig = a(new byte[] { 0x82 });



            //SendCmd(orig, new sbyte[] { 0x1 });
            //for (int i = 0; i < 1000; i++)
            //{
            //    string threadName = "thread " + i;
            //    int secondsToWait = 2 + 2 * i;
            //    var t = new Thread(new ThreadStart(DoWork));
            //    t.Start();

            //}

            Console.ReadLine();
        }

        public static void DoWork()
        {

            //RopOrderReserve rop = new RopOrderReserve();

            //rop.Source = LocalS.Entity.E_OrderSource.Wxmp;
            //rop.StoreId = "21ae9399b1804dbc9ddd3c29e8b5c670";
            //rop.ProductSkus.Add(new RopOrderReserve.ProductSku { Id = "ec2209ac9a3f4cc5b45d928c96b80287", Quantity = 2, ReceptionMode = LocalS.Entity.E_ReceptionMode.Machine });
            //rop.ProductSkus.Add(new RopOrderReserve.ProductSku { Id = "2b239e36688e4910adffe36848921015", Quantity = 2, ReceptionMode = LocalS.Entity.E_ReceptionMode.Machine });
            //var result = StoreAppServiceFactory.Order.Reserve(GuidUtil.Empty(), "e170b69479c14804a38b089dac040740", rop);
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
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

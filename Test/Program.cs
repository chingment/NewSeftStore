using LocalS.BLL.Biz;
using LocalS.BLL.Mq.MqByRedis;
using LocalS.Service.Api.StoreApp;
using Lumos;
using Lumos.Redis;
using MyPushSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TongGuanPaySdk;

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


        static void Main(string[] args)
        {
        
            TongGuanUtil api = new TongGuanUtil(null);


            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("ac", "xxx");
            dic.Add("ab", "xxx");
            dic.Add("c", "xxx");
            dic.Add("b", "xxx");
            api.GetSign(dic);

            api.AllQrcodePay("610005420191023162716933", "0.01", "自助商品", "867184037089830");
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

            var s1 = new { a = 1, b = 2 };
            //.Db.HashSetAsync("aaa","a1xxx", Newtonsoft.Json.JsonConvert.SerializeObject(s1), StackExchange.Redis.When.Always);
            //RedisManager.Db.HashSetAsync("aaa", "a2ss", Newtonsoft.Json.JsonConvert.SerializeObject(s1), StackExchange.Redis.When.Always);
            //RedisManager.Db.HashSetAsync("aaa", "b2ww", Newtonsoft.Json.JsonConvert.SerializeObject(s1), StackExchange.Redis.When.Always);


           // var s = CabineRowColLayoutModel.Convert("4,2,3,4,7");

            bool a1 = CommonUtil.IsNumber("1312331333x13133123");

            StringBuilder sql = new StringBuilder();
            sql.Append(" select a1.datef,isnull(sumCount,0) as sumCount, isnull(sumTradeAmount,0) as  sumTradeAmount from (  ");
            for (int i = 0; i < 7; i++)
            {
                string datef = DateTime.Now.AddDays(double.Parse((-i).ToString())).ToUnifiedFormatDate();

                sql.Append(" select '" + datef + "' datef union");
            }
            sql.Remove(sql.Length - 5, 5);

            sql.Append(" ) a1 left join ");
            sql.Append(" (    select datef, sum(sumCount),sum(sumTradeAmount) from ( select CONVERT(varchar(100),TradeTime, 23) datef,count(*) as sumCount ,sum(TradeAmount) as sumTradeAmount from RptOrder   where  merchId='d17df2252133478c99104180e8062230' and DateDiff(dd, TradeTime, getdate()) <= 7  group by TradeTime ) tb  group by datef ) b1 ");
            sql.Append(" on  a1.datef=b1.datef  ");
            sql.Append(" order by a1.datef desc  ");


            string a = sql.ToString();

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

            //rop.Source = LocalS.Entity.E_OrderSource.WechatMiniProgram;
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

using EasemobSdk;
using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq.MqByRedis;
using LocalS.BLL.Task;
using LocalS.Entity;
using LocalS.Service.Api.Merch;
using LocalS.Service.Api.StoreApp;
using LocalS.Service.Api.StoreTerm;
using log4net;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using MyPushSdk;
using Newtonsoft.Json.Linq;
using NPinyin;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

            string data = "{\"name\":\"qxtadmin\",\"pwd\":\"zkxz123\"}";

            byte[] buffer = System.Text.UTF8Encoding.UTF8.GetBytes(data);
            //压缩后的byte数组
            byte[] compressedbuffer = null;
            //Compress buffer,压缩缓存
            MemoryStream ms = new MemoryStream();
            using (GZipStream zs = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zs.Write(buffer, 0, buffer.Length);

                //下面两句被注释掉的代码有问题, 对应的compressedbuffer的长度只有10--该10字节应该只是压缩buffer的header

                //zs.Flush();
                //compressedbuffer = ms.ToArray();           

            }

            //只有GZipStream在Dispose后调应对应MemoryStream.ToArray()所得到的Buffer才是我们需要的结果
            compressedbuffer = ms.ToArray();
            //将压缩后的byte数组basse64字符串
            string text64 = Convert.ToBase64String(compressedbuffer);
            Console.WriteLine(text64);
            Console.ReadKey();



            Console.ReadLine();

            //var js = JToken.Parse("{\"rop\":{\"payTransId\":\"620443720210202091327829\",\"payPartner\":1,\"payPartnerPayTransId\":\"4200000909202102029437511778\",\"payWay\":1,\"completedTime\":\"2021-02-02T09:13:42.4383424+08:00\",\"pms\":{\"clientUserName\":null}},\"stockChangeRecords\":[{\"merchId\":\"d17df2252133478c99104180e8062230\",\"storeId\":\"21ae9399b1804dbc9ddd3c29e8b5c670\",\"shopId\":\"f748e0fb203a4f82bd271f4349f663c1\",\"machineId\":\"202004220011\",\"cabinetId\":\"dsx01n01\",\"slotId\":\"r0c7\",\"skuId\":\"24fbddc8b80348759c3b12012e068216\",\"shopMode\":0,\"eventCode\":\"OrderPaySuccess\",\"changeQuantity\":1,\"sumQuantity\":3,\"waitPayLockQuantity\":0,\"waitPickupLockQuantity\":1,\"sellQuantity\":2},{\"merchId\":\"d17df2252133478c99104180e8062230\",\"storeId\":\"21ae9399b1804dbc9ddd3c29e8b5c670\",\"shopId\":\"f748e0fb203a4f82bd271f4349f663c1\",\"machineId\":\"202004220011\",\"cabinetId\":\"dsx01n01\",\"slotId\":\"r0c9\",\"skuId\":\"5f34dd53523b4824bb9f6303d529d4d8\",\"shopMode\":0,\"eventCode\":\"OrderPaySuccess\",\"changeQuantity\":1,\"sumQuantity\":3,\"waitPayLockQuantity\":0,\"waitPickupLockQuantity\":1,\"sellQuantity\":2}]}");

            //if (js["stockChangeRe44cords"] != null)
            //{
            //    var s = js["stockChangeRecords"].ToObject<List<StockChangeRecordModel>>();
            //}

            // string id = Lumos.CommonUtil.ConvetMD5IN32B("/pages/productdetails/productdetails?reffSign=o176Z5HZazGSxw_yY5A0k4ccVxpA&skuId=722b4d565604489fa1f40c548e0bc114&shopMode=1&shopMethod=1&storeId=4117916edd39468fb153666a55b47165&merchId=35129159f53249efabd4f0bc9a65810c");

            //string a66 = CommonUtil.GetCnWeekDayName(DateTime.Parse("2021-01-06 08:00:00.000"));


            //&..00111122==IIaabcccdddeeeefhiiikllmmnnnnooprsssttux
            // 9b6e056b87057a3c173312ba5b2a418e109bc39eecd400086cad89909ded2ad4
            //OWI2ZTA1NmI4NzA1N2EzYzE3MzMxMmJhNWIyYTQxOGUxMDliYzM5ZWVjZDQwMDA4NmNhZDg5OTA5ZGVkMmFkNA==
            //log.InfoFormat("程序开始");
            // string assss = Signature.Compute("com.uplink.selfstore", "fanju", "7460e6512f1940f68c00fe1fdb2b7eb1", 1611646484, "{\"deviceId\":\"C0:84:7D:2C:06:10\",\"eventRemark\":\"商品取货\",\"appId\":\"com.uplink.selfstore\",\"content\":{\"orderId\":\"610409520210126153257757\",\"uniqueId\":\"6104095202101261532577571\",\"productSkuId\":\"36d68083bbdf4120961fe9cf2f3ff764\",\"cabinetId\":\"dsx01n01\",\"slotId\":\"r2c0\",\"pickupStatus\":4000,\"actionId\":1,\"actionName\":\"回到原点\",\"actionStatusCode\":2,\"actionStatusName\":\"动作执行完成\",\"pickupUseTime\":39553,\"imgId2\":\"0d5a0638-57b5-48be-8be6-65adfe0f9a1e\",\"remark\":\"取货完成\"},\"machineId\":\"202101040205\",\"lat\":0,\"lng\":0,\"eventCode\":\"Pickup\"}");
            //string a11 = "dasdsad";
            //string a22 = null;
            //string a33 = a11 + a22;

            //var p1 = new List<BuildOrderSub.ProductSku>();

            //BuildOrderSub.ProductSku a1 = new BuildOrderSub.ProductSku();
            //a1.Id = "1";
            //a1.Quantity = 3;
            //a1.ShopMode = E_SellChannelRefType.Mall;


            //BuildOrderSub.ProductSku a2 = new BuildOrderSub.ProductSku();
            //a2.Id = "2";
            //a2.Quantity = 5;
            //a2.ShopMode = E_SellChannelRefType.Mall;


            //BuildOrderSub.ProductSku a3 = new BuildOrderSub.ProductSku();
            //a3.Id = "3";
            //a3.Quantity = 10;
            //a3.ShopMode = E_SellChannelRefType.Machine;


            //var buildOrderSubs = BizFactory.Order.BuildOrderSubs(p1);

            //string ab = "[{\"cartId\":\"0ee2e0c85a314d6c9d5df6a2f0ce4aea\",\"id\":\"0a8cc495b3714c6eb8fff32043801ed5\",\"quantity\":2,\"receptionMode\":3},{\"cartId\":\"1fe7be781a5248ea9aa2b77bfcbf47dd\",\"id\":\"0a8cc495b3714c6eb8fff32043801ed5\",\"quantity\":3,\"receptionMode\":1}]";
            // string cs = "[{\"id\":\"0a8cc495b3714c6eb8fff32043801ed5\",\"receptionMode\":3,\"barCode\":\"\",\"cumCode\":\"343\",\"pinYinIndex\":\"KKKLGZ250ML\",\"productId\":\"f1b911de7b28445184b1ad89a0eade1b\",\"name\":\"可口可乐 灌装 250ML\",\"mainImgUrl\":\"http://file.17fanju.com/Upload/product/3d78ab2404c743cfa30ea634d217dd0f_O.jpg\",\"displayImgUrls\":[{\"url\":\"http://file.17fanju.com/Upload/product/3d78ab2404c743cfa30ea634d217dd0f_O.jpg\",\"isMain\":false,\"name\":\"3dbf6644-c267-46f8-bba2-7d9d1735366d_o.jpg\",\"priority\":0}],\"specDes\":[{\"name\":\"形状\",\"value\":\"灌装\"},{\"name\":\"容量\",\"value\":\"250ML\"}],\"briefDes\":\"#美味可口\",\"specItems\":[{\"name\":\"形状\",\"value\":[{\"name\":\"圆形\"},{\"name\":\"灌装\"}]},{\"name\":\"容量\",\"value\":[{\"name\":\"100ML\"},{\"name\":\"250ML\"}]}],\"specIdx\":\"灌装,250ML\",\"isTrgVideoService\":true,\"charTags\":[\"精致美品\",\"优选\"],\"specIdxSkus\":[{\"skuId\":\"0a8cc495b3714c6eb8fff32043801ed5\",\"specIdx\":\"灌装,250ML\"},{\"skuId\":\"27a780d03d364197a657cf4e05ac000d\",\"specIdx\":\"灌装,100ML\"},{\"skuId\":\"88eae5cf68f84471a35b71c453d8f806\",\"specIdx\":\"圆形,100ML\"},{\"skuId\":\"af08c84c45e04fc4b9e467b52a1671f5\",\"specIdx\":\"圆形,250ML\"}],\"stocks\":[{\"refType\":3,\"refId\":\"202004220011\",\"cabinetId\":\"dsx01n01\",\"slotId\":\"r4c4\",\"sumQuantity\":4,\"lockQuantity\":2,\"sellQuantity\":2,\"isOffSell\":false,\"salePrice\":2.00,\"salePriceByVip\":1.00}]},{\"id\":\"0a8cc495b3714c6eb8fff32043801ed5\",\"receptionMode\":1,\"barCode\":\"\",\"cumCode\":\"343\",\"pinYinIndex\":\"KKKLGZ250ML\",\"productId\":\"f1b911de7b28445184b1ad89a0eade1b\",\"name\":\"可口可乐 灌装 250ML\",\"mainImgUrl\":\"http://file.17fanju.com/Upload/product/3d78ab2404c743cfa30ea634d217dd0f_O.jpg\",\"displayImgUrls\":[{\"url\":\"http://file.17fanju.com/Upload/product/3d78ab2404c743cfa30ea634d217dd0f_O.jpg\",\"isMain\":false,\"name\":\"3dbf6644-c267-46f8-bba2-7d9d1735366d_o.jpg\",\"priority\":0}],\"specDes\":[{\"name\":\"形状\",\"value\":\"灌装\"},{\"name\":\"容量\",\"value\":\"250ML\"}],\"briefDes\":\"#美味可口\",\"specItems\":[{\"name\":\"形状\",\"value\":[{\"name\":\"圆形\"},{\"name\":\"灌装\"}]},{\"name\":\"容量\",\"value\":[{\"name\":\"100ML\"},{\"name\":\"250ML\"}]}],\"specIdx\":\"灌装,250ML\",\"isTrgVideoService\":true,\"charTags\":[\"精致美品\",\"优选\"],\"specIdxSkus\":[{\"skuId\":\"0a8cc495b3714c6eb8fff32043801ed5\",\"specIdx\":\"灌装,250ML\"},{\"skuId\":\"27a780d03d364197a657cf4e05ac000d\",\"specIdx\":\"灌装,100ML\"},{\"skuId\":\"88eae5cf68f84471a35b71c453d8f806\",\"specIdx\":\"圆形,100ML\"},{\"skuId\":\"af08c84c45e04fc4b9e467b52a1671f5\",\"specIdx\":\"圆形,250ML\"}],\"stocks\":[{\"refType\":1,\"refId\":\"000000000000\",\"cabinetId\":\"0\",\"slotId\":\"0\",\"sumQuantity\":100,\"lockQuantity\":0,\"sellQuantity\":100,\"isOffSell\":false,\"salePrice\":3.50,\"salePriceByVip\":3.50}]}]";

            //var ProductSkus = ab.ToJsonObject<List<LocalS.BLL.Biz.RopOrderReserve.ProductSku>>();
            //var bizProductSkus = cs.ToJsonObject<List<ProductSkuInfoModel>>();
            //var buildOrderSubs =BizFactory.Order.BuildOrderSubs(ProductSkus, bizProductSkus);


            // StoreAppServiceFactory.Product.GetProducts(0, int.MaxValue, "21ae9399b1804dbc9ddd3c29e8b5c670", E_SellChannelRefType.Machine, null);

            //EasemobSdk.ApiDoRequest api = new EasemobSdk.ApiDoRequest();

            //TokenRequest tokenRequest = new TokenRequest("client_credentials", "YXA6bQh4SdXsSDq3_RNy3hRoRw", "YXA6gV8I7B64QvVlU3xQrzt6aI2CK5w");

            //var apiAccessTokenResult = api.DoPost(tokenRequest);

            //if (apiAccessTokenResult.Result == ResultType.Success)
            //{

            //    RegisterUserRequest registerUserRequest = new RegisterUserRequest("15989287032", "123456", "邱");

            //    api.setAccessToken(apiAccessTokenResult.Data.Access_token);

            //    var registerUserRequestResult = api.DoPost(registerUserRequest);

            //}


            //string[] arr="A/S/B".Split('X')

            //MyDESCryptoUtil.DecodeQrcode2PickupCode("pickupcode@v2:G1et+r347rTCrWvmkFaBHQ==");



            //StringBuilder sql = new StringBuilder(" select StoreName,SumCount ");

            //sql.Append(" SumComplete,(SumCount-SumComplete) as SumNoComplete, ");
            //sql.Append(" SumEx,SumExHandle,(SumEx-SumExHandle) as SumExNoHandle,  ");
            //sql.Append(" SumQuantity,SumChargeAmount,SumRefundAmount,(SumChargeAmount-SumRefundAmount) as SumAmount,PayWayByWx,PayWayByZfb ");
            //sql.Append("  from (  ");
            //sql.Append(" select StoreId,StoreName,COUNT(Id) as SumCount, ");
            //sql.Append(" SUM( CASE ExIsHappen WHEN 1 THEN 1 ELSE 0 END) as SumEx,");
            //sql.Append(" SUM( CASE ExIsHandle WHEN 1 THEN 1 ELSE 0 END) as SumExHandle, ");
            //sql.Append(" SUM(Quantity) as SumQuantity, ");
            //sql.Append(" SUM(ChargeAmount)as SumChargeAmount, ");
            //sql.Append(" SUM(RefundAmount) as SumRefundAmount , ");
            //sql.Append(" SUM( CASE PayWay WHEN 1 THEN 1 ELSE 0 END) as PayWayByWx, ");
            //sql.Append(" SUM( CASE PayWay WHEN 2 THEN 1 ELSE 0 END) as PayWayByZfb, ");
            //sql.Append(" SUM( CASE [Status] WHEN '4000' THEN 1 ELSE 0 END) as SumComplete  ");
            //sql.Append("  from [Order]  a where PayStatus=3 and MerchId='d17df2252133478c99104180e8062230'   ");
            //sql.Append("  group by StoreId,StoreName ) tb  order by SumChargeAmount desc  ");



            // var dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToJsonObject<List<object>>();

            //TestA a = new TestA();
            // a.MaA = "dasd";

            // Task4Tim2GlobalProvider a = new Task4Tim2GlobalProvider();
            //  a.BuildSellChannelStockDateReport();

            //  string b = a.Ma.GetCnName();
            // string c = a.MaA.GetCnName();
            // string c = EventCode.GetEventLevel(EventCode.Login);
            /// string c1 = EventCode.GetEventName(EventCode.Login);
            // SdkFactory.Wx.GiftvoucherActivityNotifyPick("dad", "otakHv019rDPK-sMjbBUj8khGgAE", "1212122122", "test", "33311231", "test", DateTime.Now, "http://www.17fanju.com");
            ///string a = RedisSnUtil.BuildMachineId();

            ///IPushService pushService = new JgPushService();

            //pushService.Send("1104a8979234f30c8c2", "dasdsada", "das");
            //BizFactory.Machine.SendRebootSys(IdWorker.Build(IdType.EmptyGuid), AppId.MERCH, "d17df2252133478c99104180e8062230", "202004220011");

            //BizFactory.Order.BuildQrcode2PickupCode("31231232");

            //string a = Convert.ToString(15, 2);
            //XrtPayInfoConfg payInfo = new XrtPayInfoConfg();

            //payInfo.Mch_id = "86144035999J054";
            //payInfo.Key = "FBC8B4396940E0969048767F53CB649A";
            //payInfo.PayResultNotifyUrl = "http://api.m.17fanju.com/Api/Order/PayResultNotifyByWx";

            //RopAppTraceLog a1 = new RopAppTraceLog();
            //string aa = a1.ToJsonString();
            //BizFactory.Order.BuildPayParams(IdWorker.Build(IdType.EmptyGuid),new LocalS.BLL.Biz.RopOrderBuildPayParams {   })

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
            //var result = StoreAppServiceFactory.Order.Reserve(IdWorker.Build(IdType.EmptyGuid), "e170b69479c14804a38b089dac040740", rop);
            ;
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

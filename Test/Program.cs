using LocalS.BLL.Mq.MqByRedis;
using LocalS.Service.Api.StoreApp;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        public class userInfo
        {
            public string channelUserCode { get; set; }
            public string userName { get; set; }
            public string phone { get; set; }
            public string idName { get; set; }
            public string email { get; set; }
            public string channelCompanyCode { get; set; }
        }

        static void Main(string[] args)
        {
            //ReidsMqFactory.Global.PushStockOperate(new LocalS.BLL.Mq.MqMessageConentModel.StockOperateModel { OrderId = "1" });
            //ReidsMqFactory.Global.PushStockOperate(new LocalS.BLL.Mq.MqMessageConentModel.StockOperateModel { OrderId = "1" });
            //ReidsMqFactory.Global.PushStockOperate(new LocalS.BLL.Mq.MqMessageConentModel.StockOperateModel { OrderId = "1" });


            //string publicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDTT1ryGLfq5lucyHdzPLbjtcVsgurf5x4Y09U/cTiV85duIk0zQeRTXNyGcMAS92+xV/eGp7IjncwL8QE8JqlclLvuOU3zTdlAQ58lu/JcTcsF6eA6JXb8OJAhmDoug1J77M2GLoqAl0Cf34kavj/r9bAQpWqbk8JlJU3YqIePuwIDAQAB";
            //RSAForJava rsa = new RSAForJava();

            //userInfo u = new userInfo();

            //u.channelUserCode = "e5d1a2ca4883474791ca91ce20c90014";
            //u.userName = "e5d1a2ca4883474791ca91ce20c90014";
            //u.phone = "15989287032";
            //u.idName = "银联";
            //u.email = "";
            //u.channelCompanyCode = "";

            //string input = Newtonsoft.Json.JsonConvert.SerializeObject(u);


            //String encry = rsa.EncryptByPublicKey(input, publicKey);


            // MyAlipaySdk.AlipayUtil alipay = new MyAlipaySdk.AlipayUtil(null);

            // var u = new MyAlipaySdk.UnifiedOrder();

            // u.out_trade_no = "201503200101010011";
            //// u.store_id = "sdasdd";
            // u.subject = "Iphone6 16G";
            // u.timeout_express = "2m";
            // u.total_amount = "0.01";

            // alipay.UnifiedOrder(u);

            for (int i = 0; i < 1000; i++)
            {
                string threadName = "thread " + i;
                int secondsToWait = 2 + 2 * i;
                var t = new Thread(new ThreadStart(DoWork));
                t.Start();

            }

            Console.ReadLine();
        }

        public static void DoWork()
        {


            RopOrderReserve rop = new RopOrderReserve();

            rop.Source = LocalS.Entity.E_OrderSource.WechatMiniProgram;
            rop.StoreId = "21ae9399b1804dbc9ddd3c29e8b5c670";
            rop.ProductSkus.Add(new RopOrderReserve.ProductSku { Id = "ec2209ac9a3f4cc5b45d928c96b80287", Quantity = 2, ReceptionMode = LocalS.Entity.E_ReceptionMode.Machine });
            rop.ProductSkus.Add(new RopOrderReserve.ProductSku { Id = "2b239e36688e4910adffe36848921015", Quantity = 2, ReceptionMode = LocalS.Entity.E_ReceptionMode.Machine });
            var result = StoreAppServiceFactory.Order.Reserve(GuidUtil.Empty(), "e170b69479c14804a38b089dac040740", rop);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }
    }
}

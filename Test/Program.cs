using LocalS.Mq.MqByRedis;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsyInscarSdk;

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
            ReidsMqFactory.Global.PushOrderReserve(new LocalS.BLL.Mq.MqMessageConentModel.OrderReserveModel { OrderId = "1" });
            ReidsMqFactory.Global.PushOrderPayCompleted(new LocalS.BLL.Mq.MqMessageConentModel.OrderPayCompletedModel { OrderId = "1" });
            ReidsMqFactory.Global.PushOrderCancle(new LocalS.BLL.Mq.MqMessageConentModel.OrderCancleModel { OrderId = "1" });

            Console.ReadLine();
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




        }
    }
}

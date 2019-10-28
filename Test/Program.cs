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
        static void Main(string[] args)
        {

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

            RopOrderReserve rop = new RopOrderReserve();

            rop.Source = LocalS.Entity.E_OrderSource.WechatMiniProgram;
            rop.StoreId = "21ae9399b1804dbc9ddd3c29e8b5c670";
            rop.ProductSkus.Add(new RopOrderReserve.ProductSku { Id = "ec2209ac9a3f4cc5b45d928c96b80287", Quantity = 2, ReceptionMode = LocalS.Entity.E_ReceptionMode.Machine });
            rop.ProductSkus.Add(new RopOrderReserve.ProductSku { Id = "2b239e36688e4910adffe36848921015", Quantity = 2, ReceptionMode = LocalS.Entity.E_ReceptionMode.Machine });
            var result = StoreAppServiceFactory.Order.Reserve(GuidUtil.Empty(), "e170b69479c14804a38b089dac040740", rop);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
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

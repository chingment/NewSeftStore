using System;
using System.Linq;


namespace Lumos.Redis
{
    public enum RedisSnType
    {
        Unknow = 0,
        Order = 1,
        OrderPickCode = 2
    }
    public class RedisSnUtil
    {

        private static readonly object lock_GetIncrNum = new object();

        private static int GetIncrNum()
        {

            lock (lock_GetIncrNum)
            {
                try
                {
                    var incr = RedisManager.Db.StringIncrement(RedisKeyS.IR_SN, 1);

                    return (int)incr;

                }
                catch (Exception ex)
                {
                    LogUtil.Error("业务流水号生成发生异常，错误编码：005", ex);

                    throw new Exception("业务流水号生成发生异常，错误编码：005");
                }
            }

        }

        public static string Build(RedisSnType snType)
        {

            string prefix = "";

            switch (snType)
            {
                case RedisSnType.Order:
                    prefix = "61";
                    break;
                case RedisSnType.OrderPickCode:
                    break;

            }

            ThreadSafeRandom ran = new ThreadSafeRandom();


            string part0 = ran.Next(100, 999).ToString();
            string part1 = DateTime.Now.ToString("yyyyMMddHHmmss");
            string part2 = GetIncrNum().ToString().PadLeft(5, '0');

            string sn = prefix + part2 + part1 + part0;
            return sn;
        }

        public static string BuildMachineId()
        {
            string prefix = "";
            ThreadSafeRandom ran = new ThreadSafeRandom();
            var incr = RedisManager.Db.StringIncrement(RedisKeyS.IR_MACHINEID, 1);
            string part1 = DateTime.Now.ToString("yyyyMMdd");
            string part2 = incr.ToString().PadLeft(4, '0');
            string sn = prefix + part1 + part2;
            return sn;
        }


        public static string BuildPickupCode()
        {
            try
            {
                ThreadSafeRandom rd = new ThreadSafeRandom();
                int part1 = rd.Next(100, 999);
                var incr = RedisManager.Db.StringIncrement(RedisKeyS.IR_PICKCODE, 1);

                string part2 = incr.ToString().PadLeft(5, '0');
                ThreadSafeRandom ran = new ThreadSafeRandom();

                string code = part1.ToString() + part2;
                return code;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

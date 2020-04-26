using System;
using System.Linq;


namespace Lumos.Redis
{
    public enum IdType
    {
        Unknow = 0,
        OrderId = 1,
        MachineId = 2,
        NewGuid = 3,
        EmptyGuid = 4
    }
    public class IdWorker
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

        public static string Build(IdType snType)
        {
            string id = "";

            string prefix = "";
            string part1 = "";
            string part2 = "";
            ThreadSafeRandom ran = new ThreadSafeRandom();
            switch (snType)
            {
                case IdType.OrderId:
                    prefix = "61";
                    string part0 = ran.Next(100, 999).ToString();
                    part1 = DateTime.Now.ToString("yyyyMMddHHmmss");
                    part2 = GetIncrNum().ToString().PadLeft(5, '0');
                    id = prefix + part2 + part1 + part0;
                    break;
                case IdType.MachineId:
                    var incr = RedisManager.Db.StringIncrement(RedisKeyS.IR_MACHINEID, 1);
                    part1 = DateTime.Now.ToString("yyyyMMdd");
                    part2 = incr.ToString().PadLeft(4, '0');
                    id = prefix + part1 + part2;
                    break;
                case IdType.NewGuid:
                    id = Guid.NewGuid().ToString().Replace("-", ""); ;
                    break;
                case IdType.EmptyGuid:
                    id = Guid.Empty.ToString().Replace("-", "");
                    break;
            }

            return id;
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

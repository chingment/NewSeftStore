using LocalS.BLL.Mq.MqByRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Mq
{
    public class MqFactory
    {
        public static RedisMq4GlobalProvider Global
        {
            get
            {
                return new RedisMq4GlobalProvider();
            }
        }
    }
}

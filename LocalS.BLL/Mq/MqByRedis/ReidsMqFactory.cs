using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Mq.MqByRedis
{
    public static class ReidsMqFactory
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

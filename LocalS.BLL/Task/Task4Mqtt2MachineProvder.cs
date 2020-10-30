using LocalS.BLL.Mq.MqByRedis;
using Lumos;
using MQTTnet;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
using MyPushSdk;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocalS.BLL.Task
{
    public class Task4Mqtt2MachineProvder : BaseDbContext, ITask
    {


        public CustomJsonResult Run()
        {
            CustomJsonResult result = new CustomJsonResult();

            var push = new EmqxPushService();

            push.ConnectMqttServer();
            return result;
        }
    }
}

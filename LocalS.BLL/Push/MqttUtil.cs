using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Push
{
    public class MqttUtil
    {
        private static MqttConnectConfig GetConnectConfig(string str_conn)
        {
            var config = new MqttConnectConfig();

            string[] arrs = str_conn.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var arr in arrs)
            {
                string[] pram = arr.Split('=');
                string key = pram[0];
                string value = pram[1];
                switch (key)
                {
                    case "type":
                        config.Type = pram[1];
                        break;
                    case "server":
                        config.Server = pram[1];
                        break;
                    case "port":
                        config.Port = int.Parse(pram[1]);
                        break;
                    case "username":
                        config.UserName = pram[1];
                        break;
                    case "password":
                        config.Password = pram[1];
                        break;
                    case "clientid":
                        config.ClientId = pram[1];
                        break;
                    case "productkey":
                        config.ProductKey = pram[1];
                        break;
                    case "devicename":
                        config.DeviceName = pram[1];
                        break;
                    case "devicesecret":
                        config.DeviceSecret = pram[1];
                        break;

                }

            }

            return config;
        }

        public static MqttTcpOptions GetMqttTcpOptions(string str_conn)
        {
            var tcpOptions = new MqttTcpOptions();

            var config = GetConnectConfig(str_conn);

            if (config.Type == "exmq")
            {
                tcpOptions.Server = config.Server;
                tcpOptions.Port = config.Port;
                tcpOptions.ClientId = config.ClientId;
                tcpOptions.UserName = config.UserName;
                tcpOptions.Password = config.Password;
            }


            return tcpOptions;
        }
    }
}

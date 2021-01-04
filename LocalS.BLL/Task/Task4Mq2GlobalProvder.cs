using LocalS.BLL.Mq.MqByRedis;
using Lumos;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocalS.BLL.Task
{
    public class Task4Mq2GlobalProvder : BaseService, ITask
    {
        private int _threadCount = 1;

        public CustomJsonResult Run()
        {
            CustomJsonResult result = new CustomJsonResult();

            for (int i = 0; i < _threadCount; i++)
            {
                Thread thread = new Thread(new ThreadStart(DoWork));
                thread.Start();
            }

            return result;
        }

        public void DoWork()
        {
            RedisMq4GlobalProvider redisMq = new RedisMq4GlobalProvider();
            while (true)
            {
                try
                {
                    var handleObj = redisMq.Pop();
                    if (handleObj == null)
                    {
                        Console.WriteLine("无处理信息，休息100毫秒");
                        Thread.Sleep(1000);
                        continue;
                    }

                    while (_threadCount <= 0)
                    {
                        Thread.Sleep(100);
                    }
                    _threadCount--;

                    Thread thread = new Thread(new ThreadStart(handleObj.Handle));
                    thread.Start();

                    Console.WriteLine("正在处理信息，休息100毫秒");
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "," + ex.StackTrace);
                    Thread.Sleep(1000);
                }
                finally
                {
                    _threadCount++;
                }

            }
        }
    }
}

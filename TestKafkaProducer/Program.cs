using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;


namespace TestKafkaProducer
{
    class Program
    {
        static void Main(string[] args)
        {

            string brokerList = "127.0.0.1:9092";
            string topicName = "a";

            //这是以异步方式生产消息的代码实例
            var config = new Dictionary<string, string> {
                { "bootstrap.servers", brokerList },
                { "enable.auto.commit", "false" }
            };
            var builder = new ProducerBuilder<string, string>(config);
            var message = new Message<string, string>();
            message.Key = "A";
            message.Value = "我就是要传输的消息内容";


            using (var producer = builder.Build())
            {
                var deliveryReport = producer.ProduceAsync(topicName, message);
   
                deliveryReport.ContinueWith(task =>
                {
                    Console.WriteLine("Producer: " + producer.Name + "\r\nTopic: " + topicName + "\r\nPartition: " + task.Result.Partition + "\r\nOffset: " + task.Result.Offset);
                });

                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}

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
            //if (args.Length != 2)
            //{
            //    Console.WriteLine("Usage: .. brokerList topicName");
            //    return;
            //}

            //string brokerList = args[0];
            //string topicName = args[1];
            //string message = "我就是要传输的消息内容";

            ////这是以异步方式生产消息的代码实例
            //var config = new Dictionary<string, object> { { "bootstrap.servers", brokerList } };
            //using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            //{
            //    var deliveryReport = producer.ProduceAsync(topicName, null, message);
            //    deliveryReport.ContinueWith(task =>
            //    {
            //        Console.WriteLine("Producer: " + producer.Name + "\r\nTopic: " + topicName + "\r\nPartition: " + task.Result.Partition + "\r\nOffset: " + task.Result.Offset);
            //    });

            //    producer.Flush(TimeSpan.FromSeconds(10));
            //}
        }
    }
}

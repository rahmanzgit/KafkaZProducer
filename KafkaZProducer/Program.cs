using Confluent.Kafka;
using KafkaZProducer.Model;
using Microsoft.Extensions.Configuration;

namespace KafkaZProducer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false);

            IConfiguration appConfig = builder.Build();

            string brokerList = appConfig.GetConnectionString("Broker");//"localhost:9092"; 
            string topicName = appConfig.GetConnectionString("Topic");//"ztesttopic2";

            var config = new ProducerConfig { BootstrapServers = brokerList };
            int time = 1;
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {                
                Console.WriteLine("Ctrl-C to quit.\n");

                var cancelled = false;
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; 
                    cancelled = true;
                };

                while (!cancelled)
                {
                    Console.Write("> ");

                    string text;
                    try
                    {
                        TimeTable timetable = new TimeTable();
                        timetable.Populate(time++);
                        text = timetable.ToString(); 
                    }
                    catch (IOException)
                    {                        
                        break;
                    }
                    if (text == null)
                    {                     
                        break;
                    }
                    //
                    string key = "data";
                    string val = text;                    

                    try
                    {                      
                        var deliveryReport = await producer.ProduceAsync(
                            topicName, new Message<string, string> { Key = key, Value = val });

                        Console.WriteLine($"Delivered to: {deliveryReport.TopicPartitionOffset}");
                    }
                    catch (ProduceException<string, string> e)
                    {
                        Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to deliver message. Error: {ex.ToString()}");
                    }
                    Thread.Sleep(500);
                }
            }
        }
    }
}

using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace RabbitMQ.Publisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName="localhost"};

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            /// rabbitMq de bi kuyruk oluşturulur.
            /// 
            /// durable: true olursa rabbitmq kapatılıp açıldığında da tutulur ancak
            /// false olursa kapatıldığında uçar
            /// 
            /// exclusive: true yapılırsa sadece buradaki kuyruktan bağlanılır, 
            /// false yapılırsa subscriber veya başka yerlerden de bağlanabilir.
            /// 
            /// autodelete: subscriber yanlışlıkla giderse kuyruk yerinde kalsın istiyorsak false
            /// ama subscriber ile birlikte kuyruk da silinsin istiyorsak true denir.
            channel.QueueDeclare("hello-queue", true, false, false);


            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"Message {x}";

                Thread.Sleep(1000);
                var messageBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

                Console.WriteLine($"Mesaj gönderilmiştir:{message}");
            });


            Console.ReadLine();
        }
    }
}

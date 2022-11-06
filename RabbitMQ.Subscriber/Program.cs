using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Subscriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            /// rabbitMq de bi kuyruk oluşturulur.
            /// NOTE: Bunu publisher'ın oluşturduğundan kesin olarak eminsek subscriber dan silebiliriz.
            /// ancak emin değilsek yazılmalı çünkü publish edilmemişse hata alır.
            /// 
            /// durable: true olursa rabbitmq kapatılıp açıldığında da tutulur ancak
            /// false olursa kapatıldığında uçar
            /// 
            /// exclusive: true yapılırsa sadece buradaki kuyruktan bağlanılır, 
            /// false yapılırsa subscriber veya başka yerlerden de bağlanabilir.
            /// 
            /// autodelete: subscriber yanlışlıkla giderse kuyruk yerinde kalsın istiyorsak false
            /// ama subscriber ile birlikte kuyruk da silinsin istiyorsak true denir.
            /// 
            /// 
            //channel.QueueDeclare("hello-queue", true, false, false);


            /// Yeni Bir Consumer oluşturulur.
            var consumer = new EventingBasicConsumer(channel);

            /// prefetch size: 0 olursa herhangi bir boyuttaki mesajı gönderebilirsin.
            /// prefetch count: her bir Subscriber'a kaç kaç değer gelsin.
            /// global: true=> prefetch count'u toplam subscriber'a eşit şekilde dağıtır.
            ///         false=> prefetch count her bir subscriber için geçerli olmuş olur.
            channel.BasicQos(0, 1, false);

            /// Consume Metodu
            /// AutoAck: eğer false verirsen kuyruğu consume ettikten sonra
            /// bize başarılı olursa ben seni silmen için haber vericem demek oluyor.
            /// Eğer true verilirse consume edilir edilmez siliyor kuyruğu
            channel.BasicConsume("hello-queue", true, consumer);


            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Console.WriteLine("Gelen Mesaj: "+ message);

                /// queue'yu başarılı bir şekilde işlendiğini ve mesajı silmesini belirtiriz.
                channel.BasicAck(e.DeliveryTag, false);

            };

            Console.ReadLine();
        }
    }
}

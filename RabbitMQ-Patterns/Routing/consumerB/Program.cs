using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using FakeData;

var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "matheus", Password = "1234", VirtualHost = "rabbitmq" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "contactsB",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

channel.QueueBind(queue: "contactsB",
                  exchange: "direct_logs",
                  routingKey: ContactType.FORNECEDOR.ToString());

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" Recived [x] {message}");
};
channel.BasicConsume(queue: "contactsB",
                     autoAck: true,
                     consumer: consumer);

Console.ReadLine();
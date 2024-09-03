using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using FakeData;

var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "matheus", Password = "1234", VirtualHost = "rabbitmq" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "topic_contactA",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

channel.ExchangeDeclare(exchange: "topic_contacts", type: ExchangeType.Topic);

channel.QueueBind(queue: "topic_contactA",
                  exchange: "topic_contacts",
                  routingKey: ContactType.CLIENTE.ToString()+".*");

channel.QueueBind(queue: "topic_contactA",
                  exchange: "topic_contacts",
                  routingKey: "*."+ContactType.CLIENTE.ToString());

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" Recived [x] {message}");
};
channel.BasicConsume(queue: "topic_contactA",
                     autoAck: true,
                     consumer: consumer);

Console.ReadLine();
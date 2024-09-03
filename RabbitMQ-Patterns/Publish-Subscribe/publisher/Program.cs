using System.Text;
using System.Text.Json;
using FakeData;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "matheus", Password = "1234", VirtualHost = "rabbitmq" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare("logs", ExchangeType.Fanout);

do{
    var message = GetMessageContact();
    var body = Encoding.UTF8.GetBytes(message);

    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;

    channel.BasicPublish(exchange: "logs",
                        routingKey: string.Empty,
                        basicProperties: properties,
                        body: body);
    Console.WriteLine($" [x] Sent {message}");
    Thread.Sleep(1000);
}while(true);


IBasicProperties GetBasicProperties()
{
    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;
    return properties;
}

string GetMessageContact()
{
    Contact contact = ContactRepository.GetContact();
    return JsonSerializer.Serialize(contact, new JsonSerializerOptions() { WriteIndented = true });
}


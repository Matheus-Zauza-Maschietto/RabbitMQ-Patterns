using System.Text;
using System.Text.Json;
using FakeData;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "matheus", Password = "1234", VirtualHost = "rabbitmq" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare("direct_logs", ExchangeType.Direct);

do{
    Contact contact = ContactRepository.GetContact();
    var message = GetMessageContact(contact);
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "direct_logs",
                        routingKey: contact.ContactType,
                        basicProperties: null,
                        body: body);
    Console.WriteLine($" [x] Sent {message}");
    Thread.Sleep(1000);
}while(true);


string GetMessageContact(Contact contact)
{
    return JsonSerializer.Serialize(contact, new JsonSerializerOptions() { WriteIndented = true });
}


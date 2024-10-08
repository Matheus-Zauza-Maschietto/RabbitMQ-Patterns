﻿using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


await InvokeAsync("10");
Console.ReadLine();


static async Task InvokeAsync(string n)
{
    using var rpcClient = new RpcClient();

    Console.WriteLine(" [x] Requesting fib({0})", n);
    var response = await rpcClient.CallAsync(n);
    Console.WriteLine(" [.] Got '{0}'", response);
}

public class RpcClient : IDisposable
{
    private const string QUEUE_NAME = "rpc_queue";
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string replyQueueName;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> callbackMapper = new();

    public RpcClient()
    {
        var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "matheus", Password = "1234", VirtualHost = "rabbitmq" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        replyQueueName = channel.QueueDeclare().QueueName;
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            if (!callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out var tcs))
                return;
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            System.Console.WriteLine(response);
            tcs.TrySetResult(response);
        };

        channel.BasicConsume(consumer: consumer,
                             queue: replyQueueName,
                             autoAck: true);
    }

    public Task<string> CallAsync(string message, CancellationToken cancellationToken = default)
    {
        IBasicProperties props = channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId; //123
        props.ReplyTo = replyQueueName; //random-tatata
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var tcs = new TaskCompletionSource<string>();
        callbackMapper.TryAdd(correlationId, tcs);

        channel.BasicPublish(exchange: string.Empty,
                             routingKey: QUEUE_NAME,
                             basicProperties: props,
                             body: messageBytes);

        cancellationToken.Register(() => callbackMapper.TryRemove(correlationId, out _));
        return tcs.Task;
    }

    public void Dispose()
    {
        connection.Close();
    }
}
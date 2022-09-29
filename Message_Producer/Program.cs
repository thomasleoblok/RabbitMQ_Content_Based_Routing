using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    while (true)
    {


        channel.ExchangeDeclare(exchange: "topic_logs",
                                type: "topic");


        Console.WriteLine("Indtast besked");
        string message = Console.ReadLine();

        string routingKey = "anonymus";

        if (message.ToLower().Contains("gadget"))
            routingKey = "gadget.info";
        else if (message.ToLower().Contains("widget"))
            routingKey = "widget.info";


        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "topic_logs",
                             routingKey: routingKey,
                             basicProperties: null,
                             body: body);
        //PrintToScreen($" [x] Sent '{routingKey}':'{message}'");
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;


        };
        channel.BasicConsume(queue: channel.QueueDeclare().QueueName,
                             autoAck: true,
                             consumer: consumer);
    }
}

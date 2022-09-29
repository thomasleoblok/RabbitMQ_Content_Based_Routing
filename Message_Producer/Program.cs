using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;


var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    while (true)
    {
        Console.WriteLine("Indtast messageType");
        string mt = Console.ReadLine();

        IBasicProperties props = channel.CreateBasicProperties();
        props.ContentType = "text/plain";
        props.DeliveryMode = 2;
        props.Headers = new Dictionary<string, object>();
        props.Headers.Add("messageType", mt);

        channel.ExchangeDeclare(exchange: "topic_logs",
                                type: "topic");


        string message = "En besked";

        string routingKey = "anonymus";

        props.Headers.TryGetValue("messageType", out object messageType);

        if ((string)messageType == "Gadget")
            routingKey = "gadget.info";
        else if ((string)messageType == "Widget")
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

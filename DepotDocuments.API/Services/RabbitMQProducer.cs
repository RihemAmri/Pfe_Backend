using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using  DepotDocuments.API.DTO;

public class RabbitMQProducer
{
    private readonly IConfiguration _config;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQProducer(IConfiguration config)
    {
        _config = config;
        var factory = new ConnectionFactory()
        {
            HostName = _config["RabbitMQ:Host"],
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "credit_creation_queue", durable: false, exclusive: false, autoDelete: false);
    }

    public void SendMessage(CreditMessageDto message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(exchange: "", routingKey: "credit_creation_queue", body: body);
    }
}

using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using DepotDocuments.API.DTO;
namespace DepotDocuments.API.Services{
public class RabbitMqPublisherNotifService
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqPublisherNotifService(IConfiguration configuration)
    {
        _configuration = configuration;

        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMq:HostName"] ?? "localhost"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "notification_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void Publish(NotificationMessage message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _channel.BasicPublish(exchange: "", routingKey: "notification_queue", basicProperties: null, body: body);
    }
}}

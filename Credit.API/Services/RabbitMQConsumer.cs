using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Credit.API.DTOs;
using Credit.API.Services;

public class RabbitMQConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConfiguration _configuration;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQConsumer(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _configuration = configuration;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Connexion et création du canal RabbitMQ
        var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQ:Host"] };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "credit_creation_queue", durable: false, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            // Extraction et désérialisation du message
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            CreditMessageDto message;
            try
            {
                message = JsonSerializer.Deserialize<CreditMessageDto>(json);
            }
            catch (JsonException ex)
            {
                // Log erreur de désérialisation
                Console.WriteLine($"Erreur de désérialisation : {ex.Message}");
                return;  // Sortir si la désérialisation échoue
            }

            // Traitement du message avec injection de dépendances
            using var scope = _serviceScopeFactory.CreateScope();
            var creditService = scope.ServiceProvider.GetRequiredService<ICreditService>();

            var credit = new CreateCreditDto
            {
                IdDemande = message.IdDemande,
                IdClient = message.IdClient,
                Montant = message.Montant,
                DureeMois = message.DureeMois,
                TypeCredit = message.TypeCredit,
                InteretFixe = message.DureeMois >= 180,
                TauxInteret = message.DureeMois >= 180 ? 0.05f : 0.07f
            };

            try
            {
                await creditService.AjouterCreditAsync(credit);
            }
            catch (Exception ex)
            {
                // Log erreur lors de l'ajout du crédit
                Console.WriteLine($"Erreur lors de l'ajout du crédit : {ex.Message}");
            }
        };

        // Consommer les messages de la queue
        _channel.BasicConsume(queue: "credit_creation_queue", autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }

    // Méthode pour fermer la connexion et le canal proprement
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _channel?.Close();
        _connection?.Close();
        await base.StopAsync(stoppingToken);
    }
}

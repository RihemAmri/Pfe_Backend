using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Credit.API.DTOs;
using Credit.API.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using System;

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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQ:Host"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]),
            UserName = _configuration["RabbitMQ:Username"],
            Password = _configuration["RabbitMQ:Password"],
            DispatchConsumersAsync = true
        };

        int retryCount = 0;

        while (retryCount < 10 && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                Console.WriteLine("✅ Connexion à RabbitMQ réussie.");
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                Console.WriteLine($"❌ Tentative {retryCount} de connexion à RabbitMQ échouée : {ex.Message}");
                await Task.Delay(5000, stoppingToken); // Attendre 5 secondes sans bloquer
            }
        }

        if (_connection == null || !_connection.IsOpen)
        {
            Console.WriteLine("❌ Impossible de se connecter à RabbitMQ après plusieurs tentatives.");
            return;
        }

        _channel.QueueDeclare(
            queue: "credit_creation_queue",
            durable: false,
            exclusive: false,
            autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            CreditMessageDto message;
            try
            {
                message = JsonSerializer.Deserialize<CreditMessageDto>(json);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erreur de désérialisation : {ex.Message}");
                return;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var creditService = scope.ServiceProvider.GetRequiredService<ICreditService>();

            var credit = new CreateCreditDto
            {
                IdDemande = message.IdDemande,
                IdClient = message.IdClient,
                Montant = message.Montant,
                DureeMois = message.DureeMois,
                Emailclient= message.Emailclient,
                TypeCredit = message.TypeCredit,
                InteretFixe = message.DureeMois >= 180,
                TauxInteret = message.DureeMois >= 180 ? 0.05f : 0.07f
            };

            try
            {
                await creditService.AjouterCreditAsync(credit);
                Console.WriteLine($"✅ Crédit ajouté pour la demande {credit.IdDemande}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout du crédit : {ex.Message}");
            }
        };

        _channel.BasicConsume(queue: "credit_creation_queue", autoAck: true, consumer: consumer);

        // Garde la tâche vivante tant que le service tourne
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        try
        {
            _channel?.Close();
            _connection?.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la fermeture de RabbitMQ : {ex.Message}");
        }

        await base.StopAsync(stoppingToken);
    }
}

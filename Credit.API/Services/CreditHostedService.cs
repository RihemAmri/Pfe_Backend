using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Credit.API.Services
{
    public class CreditHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CreditHostedService> _logger;

        public CreditHostedService(IServiceProvider serviceProvider, ILogger<CreditHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("⏳ HostedService démarré.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var creditService = scope.ServiceProvider.GetRequiredService<ICreditService>();

                        _logger.LogInformation("🔄 Mise à jour des amortissements démarrée à {Time}", DateTime.Now);
                        await creditService.MettreAJourAmortissementsAsync();
                        _logger.LogInformation("✅ Mise à jour terminée à {Time}", DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erreur lors de la mise à jour des amortissements");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Change en TimeSpan.FromMinutes(1) pour test rapide
            }
        }
    }
}

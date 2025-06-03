using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Statistiques.API.DTO;
using System.Globalization;
namespace Statistiques.API.Services
{
    public class StatistiquesService : IStatistiquesService
    {
        private readonly HttpClient _httpClient;

        public StatistiquesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TotauxDto> GetTotauxAsync()
        {
            var users = await _httpClient.GetFromJsonAsync<List<UserDto>>("http://authentication-api:4000/api/v1/Auth");
            var demandes = await _httpClient.GetFromJsonAsync<List<DemandeDto>>("http://depotdocuments-api:4002/api/v1/DemandeAdmin/all");
            var credits = await _httpClient.GetFromJsonAsync<List<CreditDto>>("http://credit-api:4011/api/Credit/all-sans-amortissement");

            var utilisateurs = users?.Where(u => u.Role == "utilisateur").ToList();
            var demandesAcceptees = demandes?.Count(d => d.Statut == "validée" || d.Statut == "contrat_signé" || d.Statut == "crédit_actif");
            var demandesRefusees = demandes?.Count(d => d.Statut == "refusée");
            var creditsEnCours = credits?.Count(c => c.Status == "en cours");
            var demandesEnAttente = demandes?.Count(d => d.Statut == "soumise");
            return new TotauxDto
            {
                TotalUsers = utilisateurs?.Count ?? 0,
                TotalDemandes = demandes?.Count ?? 0,
                TotalCredits = credits?.Count ?? 0,
                TotalDemandesAcceptees = demandesAcceptees ?? 0,
                TotalDemandesRefusees = demandesRefusees ?? 0,
                TotalCreditsEnCours = creditsEnCours ?? 0,
                TotalDemandesEnAttente = demandesEnAttente ?? 0
            };
        }

        public async Task<List<MonthlyStatDto>> GetDemandesParMoisAsync()
        {
            var demandes = await _httpClient.GetFromJsonAsync<List<DemandeDto>>("http://depotdocuments-api:4002/api/v1/DemandeAdmin/all");

            return demandes?
                .GroupBy(d => d.DateDepot.ToString("yyyy-MM"))
                .OrderByDescending(g => g.Key)
                .Take(4)
                .Select(g => new MonthlyStatDto
                {
                    Mois = DateTime.ParseExact(g.Key, "yyyy-MM", CultureInfo.InvariantCulture).ToString("MMMM", new CultureInfo("fr-FR")),

                    Total = g.Count()
                })
                .OrderBy(m => DateTime.ParseExact(m.Mois, "MMMM", new CultureInfo("fr-FR")))
                .ToList();
        }

        public async Task<List<MonthlyStatDto>> GetCreditsParMoisAsync()
        {
            var credits = await _httpClient.GetFromJsonAsync<List<CreditDto>>("http://credit-api:4011/api/Credit/all-sans-amortissement");

            return credits?
                .GroupBy(c => c.DateDebut.ToString("yyyy-MM"))
                .OrderByDescending(g => g.Key)
                .Take(4)
                .Select(g => new MonthlyStatDto
                {
                    Mois = DateTime.ParseExact(g.Key, "yyyy-MM", CultureInfo.InvariantCulture).ToString("MMMM", new CultureInfo("fr-FR")),
                    Total = g.Count()
                })
                .OrderBy(m => DateTime.ParseExact(m.Mois, "MMMM", new CultureInfo("fr-FR")))
                .ToList();
        }
    }
}

using System.Threading.Tasks;
using Statistiques.API.DTO;

namespace Statistiques.API.Services
{
    public interface IStatistiquesService
    {
        Task<TotauxDto> GetTotauxAsync();
        Task<List<MonthlyStatDto>> GetDemandesParMoisAsync();
        Task<List<MonthlyStatDto>> GetCreditsParMoisAsync();
    }
}

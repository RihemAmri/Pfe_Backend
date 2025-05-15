using Credit.API.DTOs;
using Credit.API.Models;
namespace Credit.API.Services
{
    public interface ICreditService
    {
        Task<CreditResponseDto> AjouterCreditAsync(CreateCreditDto dto);
        Task<List<CreditResponseDto>> GetCreditsClientAsync(string idClient);
        Task MettreAJourAmortissementParIdAsync(string idCredit);
        Task<List<CreditSansAmortissementDto>> GetAllCreditsSansAmortissementAsync();
        Task<List<CreditResponseDto>> GetCreditsParStatusAsync(string status);
         Task<List<CreditSansAmortissementDto>> GetCreditsParTypeSansAmortissementAsync(string typeCredit);
        Task<CreditResponseDto> GetCreditParIdAsync(string idCredit);
       Task<bool> PayerIntegralementCreditAsync(string idCredit);
       Task <bool>CloturerCreditAsync(string idCredit);
       Task MettreAJourAmortissementsAsync();
       Task<bool> AjouterDemandeAnticipationAsync(DemandeAnticipationDto dto);
       Task<List<DemandeAnticipation>> GetAnticipationsSansReponseAsync();
       Task<bool> RepondreAnticipationAsync(ReponseAnticipationDto reponse);
       Task<List<DemandeAnticipation>> GetAnticipationsAvecReponseAsync();
       Task<List<DemandeAnticipation>> GetDemandesAnticipationParClientAsync(string IdCredit);
       Task<bool> UploadRecupayementAsync(string idDemande, IFormFile fichier);

    }
}

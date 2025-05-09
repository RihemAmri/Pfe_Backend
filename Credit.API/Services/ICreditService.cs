using Credit.API.DTOs;

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


    }
}

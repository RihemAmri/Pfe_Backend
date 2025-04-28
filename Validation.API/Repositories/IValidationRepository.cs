using System.Threading.Tasks;
using Validation.API.Models;

namespace Validation.API.Repositories
{
    public interface IValidationRepository
    {
        Task<ValidationItem> GetByIdAsync(string id);
        Task<ValidationItem> GetDemandeByIdAsync(string id);
        Task UpdateAsync(ValidationItem item);
        Task<bool> UpdateDemandeStatusAsync(string id, string simplifiedStatus);
        Task CreateAsync(ValidationItem item);
    }
}

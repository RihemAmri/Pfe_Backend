using System.Threading.Tasks;
using Validation.API.Dtos;
using Validation.API.Models;

namespace Validation.API.Services
{
    public interface IValidationService
    {
        Task<bool> UpdateItemStatusAsync(string id, ValidationRequestDto dto);
    }
}

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DepotDocuments.API.Services.Ocr.Interfaces
{
    public interface IOcrProcessor
    {
        Task<string> ExtractTextAsync(IFormFile file);
    }
}

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService()
    {
        var cloudinaryAccount = new Account("drnyxizz9", "626115975531244", "DDP3m9YZtRxLtDwj-NIuHy4AvcA");
        _cloudinary = new Cloudinary(cloudinaryAccount);
    }

    public async Task<CloudinaryDotNet.Actions.ImageUploadResult> UploadImageAsync(IFormFile file)
    {
        if (file == null)
        {
            throw new ArgumentNullException(nameof(file), "File cannot be null.");
        }

        var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams()
        {
            File = new FileDescription(file.FileName, file.OpenReadStream())
        };

        var result = await _cloudinary.UploadAsync(uploadParams);
        return result;
    }
}

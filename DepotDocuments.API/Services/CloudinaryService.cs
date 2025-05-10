using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService()
    {
        var cloudinaryAccount = new Account("drnyxizz9", "626115975531244", "DDP3m9YZtRxLtDwj-NIuHy4AvcA");
        _cloudinary = new Cloudinary(cloudinaryAccount);
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null)
        {
            throw new ArgumentNullException(nameof(file), "File cannot be null.");
        }

        // Détecter le type MIME du fichier
        var mimeType = file.ContentType.ToLower();

        
    if (mimeType.StartsWith("image/") || mimeType == "application/pdf")

        {
            // C'est une image ➔ upload image
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false,
                AccessMode = "public"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.AbsoluteUri;
        }
        else
        {
            // Ce n'est PAS une image ➔ upload raw (PDF, Word, etc.)
            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false,
                AccessMode = "public"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.AbsoluteUri;
        }
    }
}

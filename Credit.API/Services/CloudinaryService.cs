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
    public async Task<string> UploadFileAsync1(byte[] fileBytes, string fileName, string folderName = null)
    {
        if (fileBytes == null || fileBytes.Length == 0)
            throw new ArgumentNullException(nameof(fileBytes), "File bytes cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name must be provided.", nameof(fileName));

        using var ms = new MemoryStream(fileBytes);

        // Déduire le type MIME basique à partir de l'extension (exemple simplifié)
        string extension = Path.GetExtension(fileName).ToLower();
        bool isImage = extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif";
        bool isPdf = extension == ".pdf";

        if (isImage || isPdf)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, ms),
                Folder = folderName,
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
            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(fileName, ms),
                Folder = folderName,
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
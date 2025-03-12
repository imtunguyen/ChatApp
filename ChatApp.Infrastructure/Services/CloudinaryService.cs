using ChatApp.Application.Abstracts.Services;
using ChatApp.Application.DTOs.Cloudinary;
using ChatApp.Infrastructure.Configurations;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ChatApp.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloud;

        public CloudinaryService(IOptions<CloudinaryConfig> config)
        {
            var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloud = new Cloudinary(acc);
        }

        public async Task<List<FileUploadResult>> UploadMultipleFilesAsync(List<IFormFile> files)
        {
            var uploadTasks = files.Select(file => UploadAsync(file)).ToList();
            var results = await Task.WhenAll(uploadTasks);
            return results.ToList();
        }
      

        public async Task<FileUploadResult> UploadAsync(IFormFile formFile)
        {
            string transformation = null;
            string folder = "chatapp";  
            if (formFile.Length == 0)
            {
                return new FileUploadResult { Error = $"Tệp '{formFile.FileName}' rỗng." };
            }

            using var stream = formFile.OpenReadStream();
            RawUploadResult? uploadResult = null;
            var fileType = formFile.ContentType.ToLower();

            if (fileType.StartsWith("image"))
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(formFile.FileName, stream),
                    Transformation = transformation != null
                        ? new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                        : null,
                    Folder = folder
                };
                uploadResult = await _cloud.UploadAsync(uploadParams);
            }
            else if (fileType.StartsWith("video"))
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(formFile.FileName, stream),
                    Folder = folder
                };
                uploadResult = await _cloud.UploadAsync(uploadParams);
            }
            else if (fileType.StartsWith("audio") || fileType.StartsWith("application"))
            {
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(formFile.FileName, stream),
                    Folder = folder
                };
                uploadResult = await _cloud.UploadAsync(uploadParams);
            }

            if (uploadResult?.Error != null)
            {
                return new FileUploadResult { Error = uploadResult.Error.Message };
            }

            return new FileUploadResult
            {
                PublicId = uploadResult?.PublicId ?? "",
                Url = uploadResult?.SecureUrl.ToString() ?? "",
                Error = null
            };
        }


        public async Task<FileDeleteResult> DeleteFileAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloud.DestroyAsync(deleteParams);

            if (result.Error != null)
            {
                return new FileDeleteResult { Error = result.Error.Message };
            }

            return new FileDeleteResult
            {
                Result = result.Result,
                Error = null
            };
        }

       
    }
}

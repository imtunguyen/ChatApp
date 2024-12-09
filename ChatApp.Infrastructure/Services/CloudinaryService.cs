using ChatApp.Application.DTOs.Cloudinary;
using ChatApp.Application.Mappers;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Domain.Enums;
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
        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloud = new Cloudinary(acc);
        }
        public async Task<FileUploadResult> UploadFileAsync(IFormFile formFile)
        {
            RawUploadResult uploadResult = null;
            using var stream = formFile.OpenReadStream();

            if (formFile.Length == 0)
            {
                return new FileUploadResult { Error = "Tệp rỗng." };
            }
            MessageType messageType = MessageMapper.GetMessageType(new List<IFormFile> { formFile });
            switch (messageType)
            {
                case MessageType.Image:
                    var imageUploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(formFile.FileName, stream),
                        Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                        Folder = "chatapp"
                    };
                    uploadResult = await _cloud.UploadAsync(imageUploadParams);
                    break;

                case MessageType.Video:
                    var videoUploadParams = new VideoUploadParams
                    {
                        File = new FileDescription(formFile.FileName, stream),
                        Folder = "chatapp"
                    };
                    uploadResult = await _cloud.UploadAsync(videoUploadParams);
                    break;

                case MessageType.File:
                    var rawUploadParams = new RawUploadParams
                    {
                        File = new FileDescription(formFile.FileName, stream),
                        Folder = "chatapp"
                    };
                    uploadResult = await _cloud.UploadAsync(rawUploadParams);
                    break;
            }

            if (uploadResult?.Error != null)
            {
                return new FileUploadResult
                {
                    Error = uploadResult.Error.Message,
                };
            }

            return new FileUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.ToString(),
                Error = null
            };
        }
        public async Task<FileDeleteResult> DeleteFileAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloud.DestroyAsync(deleteParams);

            if (result.Error != null)
            {
                return new FileDeleteResult
                {
                    Error = result.Error.Message
                };
            }

            return new FileDeleteResult
            {
                Result = result.Result,
                Error = null
            };
        }

        public async Task<FileUploadResult> UploadPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "chatapp"
                };
                uploadResult = await _cloud.UploadAsync(uploadParams);

            }
            if (uploadResult.Error != null)
            {
                return new FileUploadResult
                {
                    Error = uploadResult.Error.Message
                };
            }
            return new FileUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.ToString(),
                Error = null
            };
        }
    }
}

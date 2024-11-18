﻿using ChatApp.Application.DTOs.Cloudinary;
using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Services.Abstracts
{
    public interface ICloudinaryService
    {
        Task<FileDeleteResult> DeleteFileAsync(string publicId);
        Task<FileUploadResult> UploadFileAsync(IFormFile formFile, MessageType type);

    }
}

﻿using ChatApp.Application.DTOs.Cloudinary;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Application.Abstracts.Services
{
    public interface ICloudinaryService
    {
        Task<FileDeleteResult> DeleteFileAsync(string publicId);
        Task<FileUploadResult> UploadFileAsync(IFormFile formFile);
        Task<FileUploadResult> UploadPhotoAsync(IFormFile formFile);
    }
}

﻿using ChatApp.Application.Utilities;
using ChatApp.Presentation.Modals;
using System.Text.Json;

namespace ChatApp.Presentation.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
        {
            var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };


            response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
        }
    }
}

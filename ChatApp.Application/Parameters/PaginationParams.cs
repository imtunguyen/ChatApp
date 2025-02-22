﻿namespace ChatApp.Application.Parameters
{
    public class PaginationParams
    {
        private const int MaxPageSize = int.MaxValue;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}

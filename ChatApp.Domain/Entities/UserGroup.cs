﻿namespace ChatApp.Domain.Entities
{
    public class UserGroup : BaseEntity
    {
        public required string UserId { get; set; }   
        public int GroupId { get; set; }
        public string? RoleId { get; set; }
        public DateTimeOffset JoinedAt { get; set; }
        public DateTimeOffset? RemovedAt { get; set; }
        public bool IsRemoved { get; set; }

        public AppUser? User { get; set; }    
        public Group? Group { get; set; }  
        public AppRole? Role { get; set; }
    }
}

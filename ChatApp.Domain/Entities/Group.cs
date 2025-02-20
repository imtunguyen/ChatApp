namespace ChatApp.Domain.Entities
{
    public class Group : BaseEntity    
    {
        public required string Name { get; set; }
        public required string CreatorId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public GroupFile? File { get; set; } 

        // Navigation 
        public AppUser? Creator { get; set; }
        public virtual ICollection<UserGroup>? UserGroups { get; set; } 
        public virtual ICollection<Message>? Messages { get; set; } 

    }
}

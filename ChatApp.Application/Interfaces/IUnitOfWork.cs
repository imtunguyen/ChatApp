namespace ChatApp.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IMessageRepository MessageRepository { get; }
        IGroupRepository GroupRepository { get; }
        IUserGroupRepository UserGroupRepository { get; }
        IFriendShipRepository FriendShipRepository { get; }
        Task<bool> CompleteAsync();
    }
}

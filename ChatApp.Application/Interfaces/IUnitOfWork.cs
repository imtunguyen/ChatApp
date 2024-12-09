namespace ChatApp.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IMessageRepository MessageRepository { get; }
        IChatRoomRepository ChatRoomRepository { get; }
        IUserChatRoomRepository UserChatRoomRepository { get; }

        Task<bool> CompleteAsync();
    }
}

namespace ChatApp.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IMessageRepository MessageRepository { get; }
        IChatRoomRepository ChatRoomRepository { get; }

        Task<bool> CompleteAsync();
    }
}

using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories
{
    public class FriendShipRepository : Repository<FriendShip>, IFriendShipRepository
    {
        private readonly ChatAppContext _context;
        public FriendShipRepository(ChatAppContext context) : base(context)
        {
            _context = context;
        }

        public async Task<FriendShip> GetFriendShip(string requesterId, string addresseeId)
        {
            return await _context.FriendShips.FirstOrDefaultAsync(f => (f.RequesterId == requesterId && f.AddresseeId == addresseeId) || (f.RequesterId == addresseeId && f.AddresseeId == requesterId));

        }

        public async Task<int> GetFriendShipId(string requesterId, string addresseeId)
        {
            var friendShip = await _context.FriendShips
                .Where(f => f.RequesterId == requesterId && f.AddresseeId == addresseeId)
                .Select(f => f.Id)
                .FirstOrDefaultAsync();

            return friendShip;
        }

        public async Task<IEnumerable<FriendShip>> GetFriendShips(string userId, FriendShipStatus? status = null)
        {
            var query = _context.FriendShips.AsQueryable();
            if(!string.IsNullOrEmpty(userId))
            {
                query = query.Where(f => f.RequesterId == userId || f.AddresseeId == userId);
            }
            if (status != null)
            {
                query = query.Where(f => f.Status == status.Value);
            }
            return await query.ToListAsync();
        }

        public void UpdateFriendShip(FriendShip friendShip)
        {
            var friendShipEntity = _context.FriendShips.FirstOrDefault(f => f.Id == friendShip.Id);

            if (friendShipEntity != null)
            {
                friendShipEntity.Status = friendShip.Status;
            }

        }
    }
}

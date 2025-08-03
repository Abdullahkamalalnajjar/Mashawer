﻿
namespace Mashawer.EF.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IUserRepository Users { get; private set; }

        public IDeleteRecoredRepository DeleteRecoreds { get; private set; }
        public IUserUpgradeRequestRepository UserUpgradeRequests { get; private set; }
        public IOrderRepository Orders { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            DeleteRecoreds = new DeleteRecoredRepository(_context);
            UserUpgradeRequests = new UserUpgradeRequestRepository(_context);
            Orders = new OrderRepository(_context);
        }
        public async Task<int> CompeleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

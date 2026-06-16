using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        public IUserRepository Users { get; }

        public UnitOfWork(AppDbContext db, IUserRepository users)
        {
            _db = db;
            Users = users;
        }

        public async Task<int> SaveChangesAsync() =>
            await _db.SaveChangesAsync();

        public void Dispose() => _db.Dispose();
    }
}

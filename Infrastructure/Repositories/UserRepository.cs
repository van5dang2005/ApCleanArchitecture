using Domain;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db) => _db = db;

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _db.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _db.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _db.Users.Where(u => u.IsActive).ToListAsync();

        public async Task<User> CreateAsync(User user)
        {
            _db.Users.Add(user);
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _db.Users.Update(user);
            return user;
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user is not null)
            {
                user.IsActive = false;   // soft delete
                _db.Users.Update(user);
            }
        }
    }
}

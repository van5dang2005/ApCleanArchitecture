using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;

        public UserService(IUnitOfWork uow) => _uow = uow;

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            return user is null ? null : MapToDto(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _uow.Users.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _uow.Users.GetByIdAsync(id)
                ?? throw new NotFoundException($"User {id} not found");

            if (dto.Username is not null) user.Username = dto.Username;
            if (dto.Email is not null) user.Email = dto.Email;

            var updated = await _uow.Users.UpdateAsync(user);
            await _uow.SaveChangesAsync();
            return MapToDto(updated);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _uow.Users.GetByIdAsync(id)
                ?? throw new NotFoundException($"User {id} not found");

            await _uow.Users.DeleteAsync(id);
            await _uow.SaveChangesAsync();
        }

        private static UserDto MapToDto(User u) =>
            new(u.Id, u.Username, u.Email, u.Role, u.CreatedAt, u.IsActive);
    }
}

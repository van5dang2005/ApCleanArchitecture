using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto);
        Task DeleteAsync(Guid id);
    }
}

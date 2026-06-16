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
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtService _jwt;

        public AuthService(IUnitOfWork uow, IJwtService jwt)
        {
            _uow = uow;
            _jwt = jwt;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _uow.Users.GetByEmailAsync(dto.Email)
                ?? throw new UnauthorizedException("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid credentials");

            if (!user.IsActive)
                throw new UnauthorizedException("Account is disabled");

            var token = _jwt.GenerateToken(user);
            return new AuthResponseDto(token, "Bearer", 3600, MapToDto(user));
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (await _uow.Users.GetByEmailAsync(dto.Email) is not null)
                throw new ConflictException("Email already exists");

            if (await _uow.Users.GetByUsernameAsync(dto.Username) is not null)
                throw new ConflictException("Username already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User"
            };

            var created = await _uow.Users.CreateAsync(user);
            await _uow.SaveChangesAsync();

            var token = _jwt.GenerateToken(created);
            return new AuthResponseDto(token, "Bearer", 3600, MapToDto(created));
        }

        private static UserDto MapToDto(User u) =>
            new(u.Id, u.Username, u.Email, u.Role, u.CreatedAt, u.IsActive);
    }
}

using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Auth
{
    public class LoginCommandHandler(IUnitOfWork uow, IJwtService jwt)
    : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        public async Task<AuthResponseDto> Handle(LoginCommand cmd, CancellationToken ct)
        {
            var user = await uow.Users.GetByEmailAsync(cmd.Email)
                ?? throw new UnauthorizedException("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(cmd.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid credentials");

            if (!user.IsActive)
                throw new UnauthorizedException("Account is disabled");

            var token = jwt.GenerateToken(user);
            return new AuthResponseDto(token, "Bearer", 3600,
                new UserDto(user.Id, user.Username, user.Email, user.Role, user.CreatedAt, user.IsActive));
        }
    }
}

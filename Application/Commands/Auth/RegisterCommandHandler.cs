using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Domain;
using Domain.Interfaces;

namespace Application.Commands.Auth
{
    public class RegisterCommandHandler(IUnitOfWork uow, IJwtService jwt)
    : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        public async Task<AuthResponseDto> Handle(RegisterCommand cmd, CancellationToken ct)
        {
            if (await uow.Users.GetByEmailAsync(cmd.Email) is not null)
                throw new ConflictException("Email already exists");

            if (await uow.Users.GetByUsernameAsync(cmd.Username) is not null)
                throw new ConflictException("Username already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = cmd.Username,
                Email = cmd.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(cmd.Password),
                Role = "User"
            };

            await uow.Users.CreateAsync(user);
            await uow.SaveChangesAsync();

            var token = jwt.GenerateToken(user);
            return new AuthResponseDto(token, "Bearer", 3600,
                new UserDto(user.Id, user.Username, user.Email, user.Role, user.CreatedAt, user.IsActive));
        }
    }
}

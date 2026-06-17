using Application.DTOs;
using Application.Exceptions;
using MediatR;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Users
{
    public class UpdateUserCommandHandler(IUnitOfWork uow)
        : IRequestHandler<UpdateUserCommand, UserDto>
    {
        public async Task<UserDto> Handle(UpdateUserCommand cmd, CancellationToken ct)
        {
            var user = await uow.Users.GetByIdAsync(cmd.Id)
                ?? throw new NotFoundException($"User {cmd.Id} not found");

            if (cmd.Username is not null) user.Username = cmd.Username;
            if (cmd.Email is not null) user.Email = cmd.Email;

            await uow.Users.UpdateAsync(user);
            await uow.SaveChangesAsync();

            return new UserDto(user.Id, user.Username, user.Email, user.Role, user.CreatedAt, user.IsActive);
        }
    }
}

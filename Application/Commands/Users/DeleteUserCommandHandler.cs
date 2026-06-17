using Application.Exceptions;
using MediatR;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Users
{
    public class DeleteUserCommandHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteUserCommand>
    {
        public async Task Handle(DeleteUserCommand cmd, CancellationToken ct)
        {
            var user = await uow.Users.GetByIdAsync(cmd.Id)
                ?? throw new NotFoundException($"User {cmd.Id} not found");

            await uow.Users.DeleteAsync(cmd.Id);
            await uow.SaveChangesAsync();
        }
    }
}

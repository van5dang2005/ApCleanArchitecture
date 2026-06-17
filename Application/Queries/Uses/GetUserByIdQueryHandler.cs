using Application.DTOs;
using MediatR;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Queries.Uses
{
    public class GetUserByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        public async Task<UserDto?> Handle(GetUserByIdQuery query, CancellationToken ct)
        {
            var user = await uow.Users.GetByIdAsync(query.Id);
            return user is null ? null
                : new UserDto(user.Id, user.Username, user.Email, user.Role, user.CreatedAt, user.IsActive);
        }
    }
}

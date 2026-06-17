using Application.DTOs;
using MediatR;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Queries.Uses
{
    public class GetAllUsersQueryHandler(IUnitOfWork uow)
     : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
    {
        public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery query, CancellationToken ct)
        {
            var users = await uow.Users.GetAllAsync();
            return users.Select(u =>
                new UserDto(u.Id, u.Username, u.Email, u.Role, u.CreatedAt, u.IsActive));
        }
    }
}

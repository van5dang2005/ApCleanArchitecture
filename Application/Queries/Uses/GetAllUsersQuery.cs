using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Queries.Uses
{
    public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;

}

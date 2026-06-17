using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Users
{
    public record UpdateUserCommand(Guid Id, string? Username, string? Email)
     : IRequest<UserDto>;
}

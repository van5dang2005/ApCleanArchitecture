using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Users
{
    public record DeleteUserCommand(Guid Id) : IRequest;

}

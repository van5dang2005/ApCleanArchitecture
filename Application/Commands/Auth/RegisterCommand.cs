using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands.Auth
{
    public record RegisterCommand(string Username, string Email, string Password)
    : IRequest<AuthResponseDto>;
}

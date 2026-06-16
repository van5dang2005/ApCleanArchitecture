using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public record LoginDto(
        string Email,
        string Password
    );

    public record AuthResponseDto(
        string AccessToken,
        string TokenType,
        int ExpiresIn,
        UserDto User
    );

    public record RegisterDto(
        string Username,
        string Email,
        string Password
    );
}

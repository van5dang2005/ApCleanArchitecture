namespace Application.DTOs
{
    public record UserDto(
        Guid Id,
        string Username,
        string Email,
        string Role,
        DateTime CreatedAt,
        bool IsActive
    );

    public record AuthDto(
        string Username,
        string Email,
        string Password
    );

    public record UpdateUserDto(
        string? Username,
        string? Email
    );
}

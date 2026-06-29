namespace Application.Events
{
    public record UserRegisteredEvent(
        Guid UserId,
        string Username,
        string Email,
        DateTime CreatedAt
    );
}
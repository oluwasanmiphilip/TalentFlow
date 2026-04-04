namespace TalentFlow.Application.Users.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string LearnerId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

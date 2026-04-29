public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Discipline { get; set; } = string.Empty;
    public int CohortYear { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
    public string? Bio { get; set; }
    public bool EmailNotifications { get; set; } = true;
    public string LearnerId { get; internal set; }
}

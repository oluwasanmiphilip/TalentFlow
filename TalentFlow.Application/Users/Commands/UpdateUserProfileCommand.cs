using MediatR;

public record UpdateUserProfileCommand(
    string LearnerId,
    string FullName,
    string Email,
    string PhoneNumber,
    string UpdatedBy,
    string? Bio,
    string? ProfilePhotoUrl,
    bool? EmailNotifications
) : IRequest<bool>;

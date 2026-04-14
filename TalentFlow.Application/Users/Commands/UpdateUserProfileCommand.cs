using MediatR;

namespace TalentFlow.Application.Users.Commands
{
    public class UpdateUserProfileCommand : IRequest<bool>
    {
        public string LearnerId { get; }
        public string FullName { get; }
        public string Email { get; }
        public string PhoneNumber { get; }
        public string UpdatedBy { get; }

        public UpdateUserProfileCommand(
            string learnerId,
            string fullName,
            string email,
            string phoneNumber,
            string updatedBy)
        {
            LearnerId = learnerId;
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            UpdatedBy = updatedBy;
        }
    }
}

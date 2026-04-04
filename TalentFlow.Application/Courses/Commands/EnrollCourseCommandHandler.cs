using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Courses.Commands
{
    public class EnrollCourseCommandHandler : IRequestHandler<EnrollCourseCommand, bool>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EnrollCourseCommandHandler(
            ICourseRepository courseRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(EnrollCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetBySlugAsync(request.CourseSlug, cancellationToken);
            if (course == null) return false;

            // Convert LearnerId string to Guid and fetch User entity
            if (!Guid.TryParse(request.LearnerId, out var learnerGuid))
                return false;

            var user = await _userRepository.GetByIdAsync(learnerGuid, cancellationToken);
            if (user == null) return false;

            // Pass the User entity to Enroll
            course.Enroll(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

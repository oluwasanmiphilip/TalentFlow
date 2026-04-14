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

            // ✅ LearnerId is string
            var user = await _userRepository.GetByLearnerIdAsync(request.LearnerId, cancellationToken);
            if (user == null) return false;

            course.Enroll(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

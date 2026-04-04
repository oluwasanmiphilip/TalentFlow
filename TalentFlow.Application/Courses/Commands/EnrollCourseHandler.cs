using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Courses.Commands
{
    public class EnrollCourseHandler : IRequestHandler<EnrollCourseCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EnrollCourseHandler(IUserRepository userRepository, ICourseRepository courseRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(EnrollCourseCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByLearnerIdAsync(request.LearnerId, cancellationToken);
            if (user is null) return false;

            var course = await _courseRepository.GetBySlugAsync(request.CourseSlug, cancellationToken);
            if (course is null) return false;

            course.Enroll(user);
            await _courseRepository.UpdateAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

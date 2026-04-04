using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Courses.Commands
{
    public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, string>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCourseHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork)
        {
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = new Course(request.Title, request.Description);
            await _courseRepository.AddAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return course.Slug;
        }
    }
}

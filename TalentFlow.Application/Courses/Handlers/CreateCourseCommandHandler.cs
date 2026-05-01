using MediatR;
using TalentFlow.Application.Courses.Commands;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;
using TalentFlow.Application.Common.Exceptions;

using TalentFlow.Domain.Events;

namespace TalentFlow.Application.Courses.Handlers
{
    public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Guid>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IInstructorRepository _instructorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCourseCommandHandler(
            ICourseRepository courseRepository,
            IInstructorRepository instructorRepository,
            IUnitOfWork unitOfWork)
        {
            _courseRepository = courseRepository;
            _instructorRepository = instructorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var instructor = await _instructorRepository.GetByIdAsync(request.InstructorId, cancellationToken);
            if (instructor == null)
                throw new NotFoundException($"Instructor with ID {request.InstructorId} not found");

            var course = new Course(
                request.Title,
                request.Description,
                request.Slug,
                request.ThumbnailUrl,
                request.InstructorId,
                request.DurationMinutes,
                request.Level,
                request.Price,
                request.Tags
            );

            await _courseRepository.AddAsync(course, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return course.Id;
        }

    }
}

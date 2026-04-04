using MediatR;

public record EnrollCourseCommand(string LearnerId, string CourseSlug) : IRequest<bool>;

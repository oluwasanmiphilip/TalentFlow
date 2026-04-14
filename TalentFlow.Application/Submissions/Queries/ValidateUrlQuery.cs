using MediatR;

namespace TalentFlow.Application.Submissions.Queries
{
    public record ValidateUrlQuery(string Url) : IRequest<bool>;
}

using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.Instructors.DTOs;
using TalentFlow.Application.Instructors.Queries;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Instructors.Handlers
{
    public class GetAllInstructorsHandler : IRequestHandler<GetAllInstructorsQuery, List<InstructorDto>>
    {
        private readonly IInstructorRepository _instructorRepository;

        public GetAllInstructorsHandler(IInstructorRepository instructorRepository)
        {
            _instructorRepository = instructorRepository;
        }

        public async Task<List<InstructorDto>> Handle(GetAllInstructorsQuery request, CancellationToken cancellationToken)
        {
            var instructors = await _instructorRepository.GetAllAsync(cancellationToken);

            return instructors.Select(i => new InstructorDto
            {
                Id = i.Id,
                FullName = i.FullName,
                Email = i.Email,
                //Department = i.Department
            }).ToList();
        }
    }
}
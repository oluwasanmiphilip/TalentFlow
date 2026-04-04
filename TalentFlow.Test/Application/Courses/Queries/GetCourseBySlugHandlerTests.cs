//using Xunit;
//using Moq;
//using TalentFlow.Application.Courses.Queries;
//using TalentFlow.Application.Common.Interfaces;
//using TalentFlow.Domain.Entities;

//public class GetCourseBySlugHandlerTests
//{
//    [Fact]
//    public async Task Handle_ShouldReturnNull_WhenCourseNotFound()
//    {
//        var repoMock = new Mock<ICourseRepository>();
//        repoMock.Setup(r => r.GetBySlugAsync("missing", default))
//                .ReturnsAsync((Course?)null);

//        var handler = new GetCourseBySlugHandler(repoMock.Object);
//        var result = await handler.Handle(new GetCourseBySlugQuery("missing"), default);

//        Assert.Null(result);
//    }
//}

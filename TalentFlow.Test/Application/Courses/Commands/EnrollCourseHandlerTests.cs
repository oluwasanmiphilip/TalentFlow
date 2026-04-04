//using Xunit;
//using Moq;
//using TalentFlow.Application.Courses.Commands;
//using TalentFlow.Application.Common.Interfaces;
//using TalentFlow.Domain.Entities;

//public class EnrollCourseHandlerTests
//{
//    [Fact]
//    public async Task Handle_ShouldReturnFalse_WhenUserNotFound()
//    {
//        var userRepoMock = new Mock<IUserRepository>();
//        var courseRepoMock = new Mock<ICourseRepository>();
//        var uowMock = new Mock<IUnitOfWork>();

//        userRepoMock.Setup(r => r.GetByLearnerIdAsync("missing", default))
//                    .ReturnsAsync((User?)null);

//        var handler = new EnrollCourseHandler(userRepoMock.Object, courseRepoMock.Object, uowMock.Object);
//        var result = await handler.Handle(new EnrollCourseCommand("missing", "course-slug"), default);

//        Assert.False(result);
//    }
//}

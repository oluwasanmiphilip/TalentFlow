//using Moq;
//using System.Timers;
//using TalentFlow.Application.Common.Interfaces;
//using TalentFlow.Application.Courses.Commands;
//using TalentFlow.Domain.Entities;
//using Xunit;

//public class CreateCourseHandlerTests
//{
//    [Fact]
//    public async Task Handle_ShouldCreateCourse_AndReturnSlug()
//    {
//        var repoMock = new Mock<ICourseRepository>();
//        var uowMock = new Mock<IUnitOfWork>();

//        var handler = new CreateCourseHandler(repoMock.Object, uowMock.Object);
//        var command = new CreateCourseCommand("Test Course", "Description");

//        var slug = await handler.Handle(command, default);

//        Assert.Equal("test-course", slug);
//        repoMock.Verify(r => r.AddAsync(It.IsAny<Course>(), default), Times.Once);
//        uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
//    }
//}

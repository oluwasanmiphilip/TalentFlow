//using Xunit;
//using TalentFlow.Application.Courses.Commands;
//using TalentFlow.Application.Courses.Validators;

//public class CreateCourseCommandValidatorTests
//{
//    [Fact]
//    public void ShouldFail_WhenTitleIsEmpty()
//    {
//        var validator = new CreateCourseCommandValidator();
//        var result = validator.Validate(new CreateCourseCommand("", "desc"));

//        Assert.False(result.IsValid);
//        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
//    }
//}

//using System.Net.Http.Json;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using TalentFlow.API;
//using TalentFlow.Application.Courses.Commands;
//using TalentFlow.Application.Courses.DTOs;
//using Xunit;

//public class CourseEnrollmentTests : IClassFixture<WebApplicationFactory<Program>>
//{
//    private readonly WebApplicationFactory<Program> _factory;

//    public CourseEnrollmentTests(WebApplicationFactory<Program> factory)
//    {
//        _factory = factory.WithWebHostBuilder(builder =>
//        {
//            builder.ConfigureServices(services =>
//            {
//                // Replace DB with InMemory for testing
//                var descriptor = services.SingleOrDefault(
//                    d => d.ServiceType == typeof(DbContextOptions<TalentFlowDbContext>));
//                if (descriptor != null) services.Remove(descriptor);

//                services.AddDbContext<TalentFlowDbContext>(options =>
//                    options.UseInMemoryDatabase("EnrollmentTestDb"));
//            });
//        });
//    }

//    [Fact]
//    public async Task EnrollCourse_ShouldAddLearnerToCourse()
//    {
//        var client = _factory.CreateClient();

//        // Step 1: Create course
//        var createCommand = new CreateCourseCommand("Enrollment Test Course", "Test description");
//        var createResponse = await client.PostAsJsonAsync("/api/course", createCommand);
//        createResponse.EnsureSuccessStatusCode();

//        var created = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
//        var slug = created!["slug"];

//        // Step 2: Enroll learner
//        var learnerId = "learner123";
//        var enrollResponse = await client.PostAsJsonAsync($"/api/course/{slug}/enroll", learnerId);
//        enrollResponse.EnsureSuccessStatusCode();

//        // Step 3: Fetch course by slug
//        var getResponse = await client.GetAsync($"/api/course/{slug}");
//        getResponse.EnsureSuccessStatusCode();

//        var course = await getResponse.Content.ReadFromJsonAsync<CourseDto>();
//        Assert.NotNull(course);
//        Assert.Equal("Enrollment Test Course", course!.Title);
//        Assert.Contains(course.Enrollments, e => e.UserId.ToString().Contains(learnerId));
//    }
//}

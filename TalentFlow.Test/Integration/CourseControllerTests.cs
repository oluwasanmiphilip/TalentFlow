//using System.Net.Http.Json;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.VisualStudio.TestPlatform.TestHost;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using TalentFlow.API;
//using TalentFlow.Application.Courses.Commands;
//using TalentFlow.Application.Courses.DTOs;
//using TalentFlow.Persistence;
//using Xunit;

//public class CourseControllerTests : IClassFixture<WebApplicationFactory<Program>>
//{
//    private readonly WebApplicationFactory<Program> _factory;

//    public CourseControllerTests(WebApplicationFactory<Program> factory)
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
//                    options.UseInMemoryDatabase("TestDb"));
//            });
//        });
//    }

//    [Fact]
//    public async Task CreateCourse_ThenGetCourseBySlug_ShouldReturnCourse()
//    {
//        var client = _factory.CreateClient();

//        // Step 1: Create course
//        var createCommand = new CreateCourseCommand("Integration Test Course", "Test description");
//        var createResponse = await client.PostAsJsonAsync("/api/course", createCommand);
//        createResponse.EnsureSuccessStatusCode();

//        var created = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
//        var slug = created!["slug"];

//        // Step 2: Fetch course by slug
//        var getResponse = await client.GetAsync($"/api/course/{slug}");
//        getResponse.EnsureSuccessStatusCode();

//        var course = await getResponse.Content.ReadFromJsonAsync<CourseDto>();
//        Assert.IsNotNull(course);
//        Assert.Equal("Integration Test Course", course!.Title);
//        Assert.Equal(slug, course.Slug);
//    }
//}

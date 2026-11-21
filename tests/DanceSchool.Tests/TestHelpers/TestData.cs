using DanceSchool.Api.Data;
using DanceSchool.Api.Domain.Entities;
using DanceSchool.Api.Domain.Enums;

namespace DanceSchool.Tests.TestHelpers;

public static class TestData
{
    public static async Task<User> AddUserAsync(
        DanceSchoolDbContext db, string email = "user@test.local")
    {
        var user = new User
        {
            EmailAddress = email,
            PasswordHash = "hash",
            Role = UserRole.Participant
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    public static async Task<DanceGroup> AddGroupAsync(
        DanceSchoolDbContext db, int capacity = 2)
    {
        var instructor = new User
        {
            EmailAddress = $"instr_{Guid.NewGuid():N}@test.local",
            PasswordHash = "hash",
            Role = UserRole.Instructor
        };
        var course = new Course { Name = "Test", Level = CourseLevel.Beginner };
        db.Users.Add(instructor);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var group = new DanceGroup
        {
            CourseId = course.Id,
            InstructorId = instructor.Id,
            Weekday = DayOfWeek.Monday,
            StartsAt = new TimeOnly(18, 0),
            DurationMinutes = 60,
            Capacity = capacity
        };
        db.DanceGroups.Add(group);
        await db.SaveChangesAsync();
        return group;
    }
}
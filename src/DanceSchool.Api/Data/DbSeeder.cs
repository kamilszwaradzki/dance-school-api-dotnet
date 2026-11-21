using DanceSchool.Api.Domain.Entities;
using DanceSchool.Api.Domain.Enums;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DanceSchool.Api.Data;

public class DbSeeder(
    DanceSchoolDbContext db,
    IPasswordHasher<User> hasher,
    TimeProvider time,
    IConfiguration config)
{
    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await db.Users.AnyAsync(ct))
            return;

        var password = config["Seed:Password"]
            ?? throw new InvalidOperationException(
                "Missing Seed:Password configuration.");

        var now = time.GetUtcNow().UtcDateTime;

        User CreateUser(string email, UserRole role, string? name = null)
        {
            var user = new User { EmailAddress = email, Name = name, Role = role };
            user.PasswordHash = hasher.HashPassword(user, password);

            return user;
        }

        var admin = CreateUser("admin@danceschool.pl", UserRole.Admin, "Anna Administrator");

        var instructor1 = CreateUser("jan.kowalski@danceschool.pl", UserRole.Instructor, "Jan Kowalski");
        var instructor2 = CreateUser("maria.nowak@danceschool.pl", UserRole.Instructor, "Maria Nowak");

        var participant1 = CreateUser("piotr.wisniewski@example.com", UserRole.Participant, "Piotr Wiśniewski");
        var participant2 = CreateUser("kasia.lewandowska@example.com", UserRole.Participant, "Kasia Lewandowska");
        var participant3 = CreateUser("tomek.zielinski@example.com", UserRole.Participant, "Tomek Zieliński");
        var participant4 = CreateUser("ola.mazur@example.com", UserRole.Participant, "Ola Mazur");
        var participant5 = CreateUser("michal.krawczyk@example.com", UserRole.Participant, "Michał Krawczyk");


        db.Users.AddRange(admin, instructor1, instructor2,
            participant1, participant2, participant3, participant4, participant5);

        var salsa = new Course
        {
            Name = "Salsa",
            Description = "Podstawy salsy kubańskiej – kroki, prowadzenie, rytm.",
            Level = CourseLevel.Beginner
        };
        var bachata = new Course
        {
            Name = "Bachata",
            Description = "Bachata sensual – technika, figury, muzykalność.",
            Level = CourseLevel.Intermediate
        };
        var kizomba = new Course
        {
            Name = "Kizomba",
            Description = "Wprowadzenie do kizomby – posture, prowadzenie, podstawowe kroki.",
            Level = CourseLevel.Beginner
        };

        db.Courses.AddRange(salsa, bachata, kizomba);

        var salsaMon = new DanceGroup
        {
            Course = salsa,
            Instructor = instructor1,
            Weekday = DayOfWeek.Monday,
            StartsAt = new TimeOnly(18, 0),
            DurationMinutes = 60,
            Capacity = 20
        };
        var salsaWed = new DanceGroup
        {
            Course = salsa,
            Instructor = instructor2,
            Weekday = DayOfWeek.Wednesday,
            StartsAt = new TimeOnly(19, 0),
            DurationMinutes = 60,
            Capacity = 20
        };
        var bachataTue = new DanceGroup
        {
            Course = bachata,
            Instructor = instructor1,
            Weekday = DayOfWeek.Tuesday,
            StartsAt = new TimeOnly(20, 0),
            DurationMinutes = 75,
            Capacity = 2
        };
        var kizombaThu = new DanceGroup
        {
            Course = kizomba,
            Instructor = instructor2,
            Weekday = DayOfWeek.Thursday,
            StartsAt = new TimeOnly(18, 30),
            DurationMinutes = 60,
            Capacity = 15
        };

        db.DanceGroups.AddRange(salsaMon, salsaWed, bachataTue, kizombaThu);

        var e1 = new Enrollment
        {
            User = participant1, DanceGroup = salsaMon,
            Status = EnrollmentStatus.Active
        };
        var e2 = new Enrollment
        {
            User = participant2, DanceGroup = salsaMon,
            Status = EnrollmentStatus.Active
        };
        var e3 = new Enrollment
        {
            User = participant3, DanceGroup = salsaWed,
            Status = EnrollmentStatus.Active
        };
        var e4 = new Enrollment
        {
            User = participant4, DanceGroup = kizombaThu,
            Status = EnrollmentStatus.Active
        };

        var e5 = new Enrollment
        {
            User = participant1, DanceGroup = bachataTue,
            Status = EnrollmentStatus.Active
        };
        var e6 = new Enrollment
        {
            User = participant3, DanceGroup = bachataTue,
            Status = EnrollmentStatus.Active
        };
        var e7 = new Enrollment
        {
            User = participant5, DanceGroup = bachataTue,
            Status = EnrollmentStatus.Waitlisted, Position = 1
        };
        var e8 = new Enrollment
        {
            User = participant2, DanceGroup = bachataTue,
            Status = EnrollmentStatus.Waitlisted, Position = 2
        };

        db.Enrollments.AddRange(e1, e2, e3, e4, e5, e6, e7, e8);

        var passValid = new Pass
        {
            User = participant1,
            EntriesTotal = 10,
            EntriesLeft = 8,
            ValidUntil = now.AddDays(45)
        };

        var passExpiring = new Pass
        {
            User = participant2,
            EntriesTotal = 8,
            EntriesLeft = 3,
            ValidUntil = now.AddDays(3)
        };

        var passUsed = new Pass
        {
            User = participant3,
            EntriesTotal = 10,
            EntriesLeft = 0,
            ValidUntil = now.AddDays(30)
        };

        db.Passes.AddRange(passValid, passExpiring, passUsed);

        await db.SaveChangesAsync(ct);
    }
}

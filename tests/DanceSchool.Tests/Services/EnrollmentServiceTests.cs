using DanceSchool.Api.Domain.Entities;
using DanceSchool.Api.Domain.Enums;
using DanceSchool.Api.Services;
using DanceSchool.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace DanceSchool.Tests.Services;

public class EnrollmentServiceTests(SqlServerFixture fx) : IClassFixture<SqlServerFixture>
{
    [Fact]
    public async Task Enroll_GroupHasCapacity_ReturnsSuccess_AndEnrollmentIsActive()
    {
        var (cs, ctx) = await fx.CreateDatabaseAsync();
        await using var db = ctx;
        var group = await TestData.AddGroupAsync(db, capacity: 2);
        var user = await TestData.AddUserAsync(db);

        var result = await new EnrollmentService(db).EnrollAsync(user.Id, group.Id);
        Assert.Equal(EnrollResult.Success, result);

        await using var verify = fx.OpenContext(cs);
        var e = await verify.Enrollments.SingleAsync();
        Assert.Equal(EnrollmentStatus.Active, e.Status);
        Assert.Null(e.Position);
    }

    [Fact]
    public async Task Enroll_GroupFull_ReturnsAddedToWaitingList_WithCorrectPositions()
    {
        var (cs, ctx) = await fx.CreateDatabaseAsync();
        await using var db = ctx;
        var group = await TestData.AddGroupAsync(db, capacity: 1);
        var u1 = await TestData.AddUserAsync(db, "u1@test.local");
        var u2 = await TestData.AddUserAsync(db, "u2@test.local");
        var u3 = await TestData.AddUserAsync(db, "u3@test.local");

        var svc = new EnrollmentService(db);
        Assert.Equal(EnrollResult.Success, await svc.EnrollAsync(u1.Id, group.Id));
        Assert.Equal(EnrollResult.AddedToWaitingList, await svc.EnrollAsync(u2.Id, group.Id));
        Assert.Equal(EnrollResult.AddedToWaitingList, await svc.EnrollAsync(u3.Id, group.Id));

        await using var verify = fx.OpenContext(cs);
        var waiting = await verify.Enrollments
            .Where(e => e.Status == EnrollmentStatus.Waitlisted)
            .OrderBy(e => e.Position)
            .ToListAsync();

        Assert.Equal(2, waiting.Count);
        Assert.Equal(1, waiting[0].Position);
        Assert.Equal(2, waiting[1].Position);
    }

    [Fact]
    public async Task Cancel_ActiveEnrollment_PromotesFirstFromWaitingList()
    {
        var (cs, ctx) = await fx.CreateDatabaseAsync();
        await using var db = ctx;
        var group = await TestData.AddGroupAsync(db, capacity: 1);
        var active = await TestData.AddUserAsync(db, "a@test.local");
        var waiting = await TestData.AddUserAsync(db, "w@test.local");

        var svc = new EnrollmentService(db);
        await svc.EnrollAsync(active.Id, group.Id);
        await svc.EnrollAsync(waiting.Id, group.Id);

        var result = await svc.CancelAsync(active.Id, group.Id);
        Assert.Equal(CancelResult.PromotedFromWaitingList, result);

        await using var verify = fx.OpenContext(cs);
        var promoted = await verify.Enrollments.SingleAsync(e => e.UserId == waiting.Id);
        Assert.Equal(EnrollmentStatus.Active, promoted.Status);
        Assert.Null(promoted.Position);

        var cancelled = await verify.Enrollments.SingleAsync(e => e.UserId == active.Id);
        Assert.Equal(EnrollmentStatus.Cancelled, cancelled.Status);
    }

    [Fact]
    public async Task ReEnroll_AfterCancellation_ReusesExistingRecord()
    {
        var (cs, ctx) = await fx.CreateDatabaseAsync();
        await using var db = ctx;
        var group = await TestData.AddGroupAsync(db, capacity: 1);
        var user = await TestData.AddUserAsync(db);

        var svc = new EnrollmentService(db);
        await svc.EnrollAsync(user.Id, group.Id);
        await svc.CancelAsync(user.Id, group.Id);

        var result = await svc.EnrollAsync(user.Id, group.Id);
        Assert.Equal(EnrollResult.Success, result);

        await using var verify = fx.OpenContext(cs);
        var enrollment = await verify.Enrollments.SingleAsync();
        Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
        Assert.Null(enrollment.Position);
    }

    [Fact]
    public async Task Cancel_PromotesFirst_AndRenumbersRemainingQueue()
    {
        var (cs, ctx) = await fx.CreateDatabaseAsync();
        await using var db = ctx;
        var group = await TestData.AddGroupAsync(db, capacity: 1);
        var active = await TestData.AddUserAsync(db, "a@test.local");
        var w1 = await TestData.AddUserAsync(db, "w1@test.local");
        var w2 = await TestData.AddUserAsync(db, "w2@test.local");
        var w3 = await TestData.AddUserAsync(db, "w3@test.local");

        var svc = new EnrollmentService(db);
        await svc.EnrollAsync(active.Id, group.Id);
        await svc.EnrollAsync(w1.Id, group.Id); // Position 1
        await svc.EnrollAsync(w2.Id, group.Id); // Position 2
        await svc.EnrollAsync(w3.Id, group.Id); // Position 3

        await svc.CancelAsync(active.Id, group.Id);

        await using var verify = fx.OpenContext(cs);
        var promoted = await verify.Enrollments.SingleAsync(e => e.UserId == w1.Id);
        Assert.Equal(EnrollmentStatus.Active, promoted.Status);
        Assert.Null(promoted.Position);

        var remaining = await verify.Enrollments
            .Where(e => e.Status == EnrollmentStatus.Waitlisted)
            .OrderBy(e => e.Position)
            .ToListAsync();

        Assert.Equal(2, remaining.Count);
        Assert.Equal(w2.Id, remaining[0].UserId);
        Assert.Equal(1, remaining[0].Position);
        Assert.Equal(w3.Id, remaining[1].UserId);
        Assert.Equal(2, remaining[1].Position);
    }
}
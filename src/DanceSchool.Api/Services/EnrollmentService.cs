using DanceSchool.Api.Data;
using DanceSchool.Api.Domain.Entities;
using DanceSchool.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DanceSchool.Api.Services;

public class EnrollmentService
{
    private readonly DanceSchoolDbContext _db;

    public EnrollmentService(DanceSchoolDbContext db) => _db = db;

    public async Task<EnrollResult> EnrollAsync(int userId, int groupId, CancellationToken ct = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var group = await _db.DanceGroups
            .FromSql($"SELECT * FROM DanceGroups WITH (UPDLOCK) WHERE Id = {groupId}")
            .SingleOrDefaultAsync(ct);

        if (group is null)
            return EnrollResult.GroupNotFound;

        var userExists = await _db.Users.AnyAsync(u => u.Id == userId, ct);
        if (!userExists)
            return EnrollResult.UserNotFound;

        var existing = await _db.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.DanceGroupId == groupId, ct);

        if (existing is { Status: EnrollmentStatus.Active or EnrollmentStatus.Waitlisted })
            return EnrollResult.AlreadyEnrolled;

        var activeCount = await _db.Enrollments
            .CountAsync(e => e.DanceGroupId == groupId && e.Status == EnrollmentStatus.Active, ct);

        var isFull = activeCount >= group.Capacity;

        int? nextPosition = null;
        if (isFull)
        {
            nextPosition = await _db.Enrollments
                .Where(e => e.DanceGroupId == groupId && e.Status == EnrollmentStatus.Waitlisted)
                .MaxAsync(e => (int?)e.Position, ct) ?? 0;
            nextPosition += 1;
        }

        if (existing is null)
        {
            existing = new Enrollment { UserId = userId, DanceGroupId = groupId };
            _db.Enrollments.Add(existing);
        }

        existing.Status = isFull ? EnrollmentStatus.Waitlisted : EnrollmentStatus.Active;
        existing.Position = isFull ? nextPosition : null;

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return isFull ? EnrollResult.AddedToWaitingList : EnrollResult.Success;
    }

    public async Task<CancelResult> CancelAsync(int userId, int groupId, CancellationToken ct = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var group = await _db.DanceGroups
            .FromSql($"SELECT * FROM DanceGroups WITH (UPDLOCK) WHERE Id = {groupId}")
            .SingleOrDefaultAsync(ct);

        if (group is null)
            return CancelResult.GroupNotFound;

        var enrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId
                                   && e.DanceGroupId == groupId
                                   && e.Status != EnrollmentStatus.Cancelled, ct);

        if (enrollment is null)
            return CancelResult.NotEnrolled;

        var wasActive = enrollment.Status == EnrollmentStatus.Active;

        enrollment.Status = EnrollmentStatus.Cancelled;
        enrollment.Position = null;

        var promoted = false;
        if (wasActive)
        {
            var queue = await _db.Enrollments
                .Where(e => e.DanceGroupId == groupId && e.Status == EnrollmentStatus.Waitlisted)
                .OrderBy(e => e.Position)
                .ToListAsync(ct);

            if (queue.Count > 0)
            {
                queue[0].Status = EnrollmentStatus.Active;
                queue[0].Position = null;
                promoted = true;

                for (var i = 1; i < queue.Count; i++)
                    queue[i].Position = i;
            }
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return promoted ? CancelResult.PromotedFromWaitingList : CancelResult.Success;
    }
}
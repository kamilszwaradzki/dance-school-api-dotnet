using DanceSchool.Api.Domain;
using DanceSchool.Api.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace DanceSchool.Api.Data;

public class DanceSchoolDbContext : DbContext
{
    public DanceSchoolDbContext(
        DbContextOptions<DanceSchoolDbContext> options,
        TimeProvider timeProvider)
        : base(options)
    {
        _timeProvider = timeProvider;
    }

    private readonly TimeProvider _timeProvider;
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<DanceGroup> DanceGroups => Set<DanceGroup>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Pass> Passes => Set<Pass>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DanceSchoolDbContext).Assembly);

        foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            fk.DeleteBehavior = DeleteBehavior.Restrict;
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();

        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;

                entry.Property(x => x.CreatedAt).IsModified = false;
            }
        }
    }
}

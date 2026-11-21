using DanceSchool.Api.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanceSchool.Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(x => x.Name)
            .HasMaxLength(255);

        builder.Property(x => x.EmailAddress)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(x => x.EmailAddress)
            .IsUnique();
    }
}

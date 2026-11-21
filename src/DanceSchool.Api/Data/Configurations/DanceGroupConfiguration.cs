using DanceSchool.Api.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanceSchool.Api.Data.Configurations;

public class DanceGroupConfiguration : IEntityTypeConfiguration<DanceGroup>
{
    public void Configure(EntityTypeBuilder<DanceGroup> builder)
    {
        builder.ToTable("DanceGroups", t => t.HasCheckConstraint("CK_DanceGroups_Capacity", "[Capacity] > 0"));
    }
}

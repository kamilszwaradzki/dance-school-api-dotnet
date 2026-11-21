using DanceSchool.Api.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DanceSchool.Api.Data.Configurations;

public class PassConfiguration : IEntityTypeConfiguration<Pass>
{
    public void Configure(EntityTypeBuilder<Pass> builder)
    {
        builder.ToTable("Passes", t =>
        {
            t.HasCheckConstraint("CK_Passes_EntriesTotal", "[EntriesTotal] > 0");
            t.HasCheckConstraint("CK_Passes_EntriesLeft", "[EntriesLeft] >= 0 AND [EntriesLeft] <= [EntriesTotal]");
        });
    }
}

using A_MVC01.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace A_MVC01.Configurations
{
    public class PlanConfigurations : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(p => p.Name).HasColumnType("varchar").HasMaxLength(20);
            builder.Property(p => p.Description).HasMaxLength(200);
            builder.Property(p => p.Price).HasPrecision(10, 2);

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GetDate()");
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("DurationCheckValue", "Duration Between 1 and 365");
            });

        }
    }
}

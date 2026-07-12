using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace GymSystemG03.DAL.Configurations
{
    internal class HealthRecordConfigurations : IEntityTypeConfiguration<HealthRecord>
    {
        public void Configure(EntityTypeBuilder<HealthRecord> builder)
        {
            builder.Property(x => x.BloodType)
                .HasMaxLength(5);

            builder.Property(x => x.Note)
                   .HasMaxLength(500);
            builder.Property(x => x.Height).HasColumnType("decimal(5,2)");

            builder.Property(x => x.Weight).HasColumnType("decimal(5,2)");
        }
    }
}

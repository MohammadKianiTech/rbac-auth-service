using Evalify.Domain.Entities;
using Evalify.Domain.Enums;
using Evalify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evalify.Infrastructure.Configurations;

internal sealed class EvaluationConfiguration : IEntityTypeConfiguration<Evaluation>
{
    public void Configure(EntityTypeBuilder<Evaluation> builder)
    {
        builder.ToTable("evaluations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired()
            .HasConversion(title => title.Value, value => new Title(value));

        builder.Property(e => e.Description)
            .HasMaxLength(1000)
            .IsRequired()
            .HasConversion(description => description.Value, value => new Description(value));

        builder.Property(e => e.Status)
               .HasDefaultValue(EvaluationStatus.Draft)
               .HasConversion<int>();

        builder.OwnsOne(e => e.Duration, owned =>
        {
            owned.Property(p => p.Start).HasColumnName("start_date");
            owned.Property(p => p.End).HasColumnName("end_date");
        });

        builder.HasOne(e => e.Creator).WithMany(u => u.CreatedEvaluations).HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.CreatedBy);
        builder.HasIndex(e => e.Status);
    }
}
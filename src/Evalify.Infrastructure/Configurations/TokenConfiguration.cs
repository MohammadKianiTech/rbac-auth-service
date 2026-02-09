using Evalify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evalify.Infrastructure.Configurations;

internal sealed class TokenConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToTable("tokens");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.Client).HasConversion<int>().IsRequired();
        builder.Property(t => t.AccessTokenHash).IsRequired();
        builder.Property(t => t.AccessTokenExpiresDateTime).IsRequired();
        builder.Property(t => t.RefreshTokenIdHash).HasMaxLength(450).IsRequired();
        builder.Property(t => t.RefreshTokenExpiresDateTime).IsRequired();

        builder.HasOne(t => t.User).WithMany(u => u.Tokens).HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(t => t.UserId);
    }
}
using Evalify.Domain.Entities;
using Evalify.Domain.Enums;
using Evalify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evalify.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);
        builder.Property(user => user.FirstName).HasMaxLength(200).HasConversion(firstName => firstName.Value, value => new FirstName(value));
        builder.Property(user => user.LastName).HasMaxLength(200).HasConversion(firstName => firstName.Value, value => new LastName(value));
        builder.Property(user => user.Email).HasMaxLength(200).HasConversion(email => email.Value.ToLowerInvariant(), value => new Email(value));
        builder.Property(user => user.EmailVerified).HasDefaultValue(false);
        builder.Property(user => user.PasswordHash).HasMaxLength(200).HasConversion(password => password.Value, value => new Password(value));
        builder.Property(user => user.RoleId).HasConversion(roleId => roleId.Value, dbId => new RoleId(dbId)).HasColumnName("role_id");
        builder.Property(user => user.SerialNumber).HasConversion(serialNumber => serialNumber.Value, dbId => new SerialNumber(dbId));
        builder.Property(user => user.Status).IsRequired().HasConversion<int>();

        builder.Property(user => user.OtpExpiresAt).IsRequired(false);
        builder.Property(user => user.OtpCode).IsRequired(false).HasMaxLength(10);
        builder.Property(user => user.TwoFactorEnabled).HasDefaultValue(false);
        builder.Property(user => user.TwoFactorRecoveryCode).IsRequired(false).HasMaxLength(250);
        builder.Property(user => user.TwoFactorSecret).IsRequired(false).HasMaxLength(250);
        builder.Property(user => user.FailedLoginAttempts).HasDefaultValue(0);
        builder.Property(user => user.LastFailedLoginAt).IsRequired(false);
        builder.Property(user => user.AccountLockedUntil).IsRequired(false);
        builder.Property(user => user.LastLoggedIn).IsRequired(false);

        builder.HasOne(user => user.Role).WithMany().HasForeignKey("role_id").OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(user => user.Tokens).WithOne(t => t.User).HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(user => user.CreatedEvaluations).WithOne(e => e.Creator).HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(user => user.Email).IsUnique();
    }
}
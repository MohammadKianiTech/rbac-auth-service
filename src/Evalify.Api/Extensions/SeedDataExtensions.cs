using Bogus;
using Evalify.Application.Abstractions.Data;
using Dapper;
using Evalify.Application.Abstractions.Hashing;

namespace Evalify.Api.Extensions;

public static class SeedDataExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        using var connection = sqlConnectionFactory.CreateConnection();

        // var faker = new Faker();
        //---------------------------------------
        // 1. Seed Permissions
        var permissions = new List<object>
            {
                new { Id = 1, Name = "users.read" },
                new { Id = 2, Name = "users.create" },
                new { Id = 3, Name = "users.update" },
                new { Id = 4, Name = "users.delete" },
                new { Id = 5, Name = "evaluations.read" },
                new { Id = 6, Name = "evaluations.create" },
                new { Id = 7, Name = "evaluations.update" },
                new { Id = 8, Name = "evaluations.delete" },
                new { Id = 9, Name = "admin.super" }
            };

        const string permissionsSql = """
                INSERT INTO permissions (id, name)
                SELECT @Id, @Name
                WHERE NOT EXISTS (SELECT 1 FROM permissions WHERE id = @Id);
            """;
        connection.Execute(permissionsSql, permissions);
        //---------------------------------------
        // 2. Seed Role (Admin)
        var roles = new List<object>{
                new { Id = 1, Name = "Admin", IsActive = true },
                new { Id = 2, Name = "Member", IsActive = true }
            };
        const string roleSql = """
                INSERT INTO roles (id, name, is_active)
                SELECT @Id, @Name, @IsActive
                WHERE NOT EXISTS (SELECT 1 FROM roles WHERE id = @Id);
            """;
        connection.Execute(roleSql, roles);
        //---------------------------------------
        // 3. Seed Role-Permission Mapping
        var rolePermissions = new[]
        {
                new { RoleId = 1, PermissionId = 1 },
                new { RoleId = 1, PermissionId = 2 },
                new { RoleId = 1, PermissionId = 3 },
                new { RoleId = 1, PermissionId = 4 },
                new { RoleId = 1, PermissionId = 5 },
                new { RoleId = 1, PermissionId = 6 },
                new { RoleId = 1, PermissionId = 7 },
                new { RoleId = 1, PermissionId = 8 },
                new { RoleId = 1, PermissionId = 9 },

                new { RoleId = 2, PermissionId = 1 },
                new { RoleId = 2, PermissionId = 5 },
            };

        const string rolePermissionsSql = """
                INSERT INTO role_permissions (role_id, permission_id)
                SELECT @RoleId, @PermissionId
                WHERE NOT EXISTS (
                    SELECT 1 FROM role_permissions 
                    WHERE role_id = @RoleId AND permission_id = @PermissionId
                );
            """;
        connection.Execute(rolePermissionsSql, rolePermissions);
        //---------------------------------------
        // 2. Seed User
        List<object> users = new();
        users.Add(new
        {
            Id = Guid.NewGuid(),
            FirstName = "Mohammad",
            LastName = "Kiani",
            Email = "admin@aawiz.com",
            EmailVerified = true,
            PasswordHash = passwordHasher.Hash("123456"),
            RoleId = 1,
            SerialNumber = Guid.NewGuid(),
            OtpCode = (string?)null,
            OtpExpiresAt = (DateTime?)null,
            TwoFactorEnabled = false,
            TwoFactorRecoveryCode = (string?)null,
            TwoFactorSecret = (string?)null,
            FailedLoginAttempts = 0,
            LastFailedLoginAt = (DateTime?)null,
            AccountLockedUntil = (DateTime?)null,
            Status = 1,
            LastLoggedIn = (DateTime?)null,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        });

        const string usersSql = """
                INSERT INTO users
                (
                    id,
                    first_name,
                    last_name,
                    email,
                    email_verified,
                    password_hash,
                    role_id,
                    serial_number,
                    otp_code,
                    otp_expires_at,
                    two_factor_enabled,
                    two_factor_recovery_code,
                    two_factor_secret,
                    failed_login_attempts,
                    last_failed_login_at,
                    account_locked_until,
                    status,
                    last_logged_in,
                    created_at,
                    modified_at
                )
                VALUES
                (
                    @Id,
                    @FirstName,
                    @LastName,
                    @Email,
                    @EmailVerified,
                    @PasswordHash,
                    @RoleId,
                    @SerialNumber,
                    @OtpCode,
                    @OtpExpiresAt,
                    @TwoFactorEnabled,
                    @TwoFactorRecoveryCode,
                    @TwoFactorSecret,
                    @FailedLoginAttempts,
                    @LastFailedLoginAt,
                    @AccountLockedUntil,
                    @Status,
                    @LastLoggedIn,
                    @CreatedAt,
                    @ModifiedAt
                );
            """;

        connection.Execute(usersSql, users);
    }
}

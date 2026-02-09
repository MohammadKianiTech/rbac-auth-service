using Evalify.Domain.DomainEvents;
using Evalify.Domain.Entities;
using Evalify.Domain.UnitTests.Infrastructure;
using FluentAssertions;

namespace Evalify.Domain.UnitTests.Users;

public class UserTests : BaseTest
{
    [Fact]
    public void Create_Should_SetPropertyValue()
    {
        // Act
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email, UserData.Password, UserData.RoleId, DateTime.UtcNow, DateTime.UtcNow);

        // Assert
        user.FirstName.Should().Be(UserData.FirstName);
        user.LastName.Should().Be(UserData.LastName);
        user.Email.Should().Be(UserData.Email);
        user.PasswordHash.Should().Be(UserData.Password);
        user.RoleId.Should().Be(UserData.RoleId);
    }

    [Fact]
    public void Create_Should_RaiseUserCreatedDomainEvent()
    {
        // Act
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email, UserData.Password, UserData.RoleId, DateTime.UtcNow, DateTime.UtcNow);

        // Assert
        var userCreatedDomainEvent = AssertDomainEventWasPublished<UserCreatedDomainEvent>(user);

        userCreatedDomainEvent.UserId.Should().Be(user.Id);
        userCreatedDomainEvent.Id.Should().NotBe(Guid.Empty);
    }

}
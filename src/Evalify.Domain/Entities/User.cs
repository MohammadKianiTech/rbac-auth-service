namespace Evalify.Domain.Entities;

public sealed class User : Entity
{
    private readonly List<Token> _tokens = new();
    private readonly List<Evaluation> _createdEvaluations = new();

    private User(Guid id, FirstName firstName, LastName lastName, Email email, Password passwordHash, RoleId roleId, DateTime createdAt, DateTime modifiedAt) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        RoleId = roleId;
        SerialNumber = new SerialNumber(Guid.NewGuid());
        Status = UserStatus.Active;
        EmailVerified = false;
        TwoFactorEnabled = false;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    private User()
    {
    }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Email Email { get; private set; }
    public bool EmailVerified { get; private set; }
    public Password PasswordHash { get; private set; }
    public RoleId RoleId { get; private set; }
    public SerialNumber SerialNumber { get; private set; } = default!;

    public string? OtpCode { get; private set; }
    public DateTime? OtpExpiresAt { get; private set; }
    public bool TwoFactorEnabled { get; private set; }
    public string? TwoFactorRecoveryCode { get; private set; }
    public string? TwoFactorSecret { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LastFailedLoginAt { get; private set; }
    public DateTime? AccountLockedUntil { get; private set; }

    public UserStatus Status { get; private set; }
    public DateTime? LastLoggedIn { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ModifiedAt { get; private set; }

    public Role? Role { get; private set; }
    public IReadOnlyCollection<Token> Tokens => _tokens.AsReadOnly();
    public IReadOnlyCollection<Evaluation> CreatedEvaluations => _createdEvaluations.AsReadOnly();

    public static User Create(FirstName firstName, LastName lastName, Email email, Password passwordHash, RoleId roleId, DateTime createdAt, DateTime modifiedAt)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email, passwordHash, roleId, createdAt, modifiedAt);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(Guid.NewGuid(), user.Id));

        return user;
    }
    public void AddToken(Token token)
    {
        ArgumentNullException.ThrowIfNull(token);
        _tokens.Add(token);
    }
}
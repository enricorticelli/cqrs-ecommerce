using Shared.BuildingBlocks.Exceptions;

namespace Account.Domain.Entities;

public sealed class AccountUser
{
    public Guid Id { get; }
    public string Realm { get; }
    public string Username { get; }
    public string Email { get; }
    public string NormalizedEmail { get; }
    public string PasswordHash { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Phone { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; }

    private AccountUser(
        Guid id,
        string realm,
        string username,
        string email,
        string normalizedEmail,
        string passwordHash,
        bool isEmailVerified,
        string firstName,
        string lastName,
        string phone,
        DateTimeOffset createdAtUtc)
    {
        Id = id;
        Realm = realm;
        Username = username;
        Email = email;
        NormalizedEmail = normalizedEmail;
        PasswordHash = passwordHash;
        IsEmailVerified = isEmailVerified;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        CreatedAtUtc = createdAtUtc;
    }

    public static AccountUser CreateCustomer(
        string email,
        string normalizedEmail,
        string passwordHash,
        string firstName,
        string lastName,
        string phone)
    {
        EnsureNotBlank(email, "Customer email is required.");
        EnsureNotBlank(normalizedEmail, "Customer normalized email is required.");
        EnsureNotBlank(passwordHash, "Customer password hash is required.");
        EnsureNotBlank(firstName, "Customer first name is required.");
        EnsureNotBlank(lastName, "Customer last name is required.");

        return new AccountUser(
            Guid.NewGuid(),
            "customer",
            normalizedEmail,
            email.Trim(),
            normalizedEmail.Trim(),
            passwordHash,
            false,
            firstName.Trim(),
            lastName.Trim(),
            (phone ?? string.Empty).Trim(),
            DateTimeOffset.UtcNow);
    }

    public static AccountUser CreateAdmin(string username, string passwordHash)
    {
        EnsureNotBlank(username, "Admin username is required.");
        EnsureNotBlank(passwordHash, "Admin password hash is required.");

        var normalized = username.Trim().ToLowerInvariant();

        return new AccountUser(
            Guid.NewGuid(),
            "admin",
            normalized,
            normalized,
            normalized,
            passwordHash,
            true,
            "Backoffice",
            "Admin",
            string.Empty,
            DateTimeOffset.UtcNow);
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
    }

    public void ChangePasswordHash(string newPasswordHash)
    {
        EnsureNotBlank(newPasswordHash, "Password hash is required.");
        PasswordHash = newPasswordHash;
    }

    public void UpdateProfile(string firstName, string lastName, string phone)
    {
        EnsureNotBlank(firstName, "First name is required.");
        EnsureNotBlank(lastName, "Last name is required.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Phone = (phone ?? string.Empty).Trim();
    }

    public static AccountUser Restore(
        Guid id,
        string realm,
        string username,
        string email,
        string normalizedEmail,
        string passwordHash,
        bool isEmailVerified,
        string firstName,
        string lastName,
        string phone,
        DateTimeOffset createdAtUtc)
    {
        return new AccountUser(
            id,
            realm,
            username,
            email,
            normalizedEmail,
            passwordHash,
            isEmailVerified,
            firstName,
            lastName,
            phone,
            createdAtUtc);
    }

    private static void EnsureNotBlank(string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationAppException(message);
        }
    }
}

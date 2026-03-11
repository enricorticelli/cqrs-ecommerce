using Account.Domain.Entities;
using Shared.BuildingBlocks.Exceptions;
using Xunit;

namespace Account.Tests;

public sealed class AccountDomainTests
{
    [Fact]
    public void CreateCustomer_ValidInput_CreatesNotVerifiedCustomer()
    {
        var user = AccountUser.CreateCustomer(
            "mario.rossi@example.com",
            "mario.rossi@example.com",
            "hash",
            "Mario",
            "Rossi",
            "+39 333 1112222");

        Assert.Equal("customer", user.Realm);
        Assert.False(user.IsEmailVerified);
        Assert.Equal("Mario", user.FirstName);
        Assert.Equal("Rossi", user.LastName);
    }

    [Fact]
    public void VerifyEmail_SetsFlagToTrue()
    {
        var user = AccountUser.CreateCustomer("a@b.c", "a@b.c", "hash", "A", "B", "");

        user.VerifyEmail();

        Assert.True(user.IsEmailVerified);
    }

    [Fact]
    public void UpdateProfile_BlankFirstName_ThrowsValidationException()
    {
        var user = AccountUser.CreateCustomer("a@b.c", "a@b.c", "hash", "A", "B", "");

        Assert.Throws<ValidationAppException>(() => user.UpdateProfile("", "B", "+39"));
    }

    [Fact]
    public void CreateAddress_ValidInput_CreatesAddress()
    {
        var address = AccountAddress.Create(
            Guid.NewGuid(),
            "Casa",
            "Via Roma 1",
            "Milano",
            "20100",
            "Italia",
            isDefaultShipping: true,
            isDefaultBilling: false);

        Assert.Equal("Casa", address.Label);
        Assert.True(address.IsDefaultShipping);
        Assert.False(address.IsDefaultBilling);
    }

    [Fact]
    public void UpdateAddress_BlankStreet_ThrowsValidationException()
    {
        var address = AccountAddress.Create(Guid.NewGuid(), "Casa", "Via Roma 1", "Milano", "20100", "Italia", true, true);

        Assert.Throws<ValidationAppException>(() =>
            address.Update("Casa", "", "Milano", "20100", "Italia", true, true));
    }
}

using Cart.Application;
using Moq;
using Xunit;

namespace Cart.Tests;

public sealed class AddCartItemCommandValidatorTests
{
    [Fact]
    public void Should_fail_when_quantity_is_zero()
    {
        var validator = new AddCartItemCommandValidator();
        var command = new AddCartItemCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "SKU-1",
            "Product",
            0,
            10m);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(AddCartItemCommand.Quantity));
    }

    [Fact]
    public void Moq_should_verify_test_probe_invocation()
    {
        var probe = new Mock<ITestProbe>();
        probe.Object.Ping("cart-tests");

        probe.Verify(x => x.Ping("cart-tests"), Times.Once);
    }
}

public interface ITestProbe
{
    void Ping(string message);
}

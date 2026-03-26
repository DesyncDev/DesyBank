using DesyBank.Domain.Models;
using FluentAssertions;

namespace DesyBank.Tests.Models
{
    public class AccountModelTests
    {
        [Fact]
        [Trait("Models", "Account")]
        public void Account_WithValidData_ReturnsAccountWithCorrectProperties()
        {
            // Arrange
            var expectedUserId = Guid.NewGuid();
            var expectedAccountNumber = "12345-6";

            var account = new Account(
                expectedUserId,
                expectedAccountNumber
            );

            // Expected result
            account.Id.Should().NotBeEmpty();
            account.UserId.Should().Be(expectedUserId);
            account.AccountNumber.Should().Be(expectedAccountNumber);
            account.Balance.Should().Be(0);
            account.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using DesyBank.Domain.Models;
using FluentAssertions;

namespace DesyBank.Tests.Models
{
    public class TransferModelTests
    {
        // Faker
        private readonly Faker _faker = new("pt_BR");

        [Fact]
        [Trait("Models", "Transfer")]
        public void CreateTransfer_WithValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            var expectedFromAccountId = Guid.NewGuid();
            var expectedToAccountId = Guid.NewGuid();
            var expectedAmount = _faker.Finance.Amount();

            var transfer = new Transfer(
                expectedFromAccountId,
                expectedToAccountId,
                expectedAmount
            );

            // Expected results
            transfer.Id.Should().NotBeEmpty();
            transfer.FromAccountId.Should().Be(expectedFromAccountId);
            transfer.ToAccountId.Should().Be(expectedToAccountId);
            transfer.Amount.Should().Be(expectedAmount);
            transfer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }
    }
}
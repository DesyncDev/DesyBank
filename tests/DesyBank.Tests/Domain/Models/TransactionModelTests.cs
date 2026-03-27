using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using DesyBank.Domain.Enums;
using DesyBank.Domain.Models;
using FluentAssertions;

namespace DesyBank.Tests.Models
{
    public class TransactionModelTests
    {
        // Faker 
        private readonly Faker _faker = new("pt_BR");

        [Fact]
        [Trait("Models", "Transaction")]
        public void CreateTransaction_WithDepositType_SetsPropertiesCorrectly()
        {
            // Arrange
            var expectedAccountId = Guid.NewGuid();
            var expectedAmount = _faker.Finance.Amount();

            var transaction = new Transaction(
                expectedAccountId,
                expectedAmount,
                ETransactionType.Deposit,
                null
            );

            // Expected result
            transaction.Id.Should().NotBeEmpty();
            transaction.AccountId.Should().Be(expectedAccountId);
            transaction.Amount.Should().Be(expectedAmount);
            transaction.Type.Should().Be(ETransactionType.Deposit);
            transaction.TransferType.Should().BeNull();
            transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        [Fact]
        [Trait("Models", "Transaction")]
        public void CreateTransaction_WithWithdrawType_SetsPropertiesCorrectly()
        {
            // Arrange
            var expectedAccountId = Guid.NewGuid();
            var expectedAmount = _faker.Finance.Amount();

            var transaction = new Transaction(
                expectedAccountId,
                expectedAmount,
                ETransactionType.WithDraw,
                null
            );

            // Expected result
            transaction.Id.Should().NotBeEmpty();
            transaction.AccountId.Should().Be(expectedAccountId);
            transaction.Amount.Should().Be(expectedAmount);
            transaction.Type.Should().Be(ETransactionType.WithDraw);
            transaction.TransferType.Should().BeNull();
            transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        [Fact]
        [Trait("Models", "Transaction")]
        public void CreateTransaction_WithTransferCredit_SetsPropertiesCorrectly()
        {
            // Arrange
            var expectedAccountId = Guid.NewGuid();
            var expectedAmount = _faker.Finance.Amount();

            var transaction = new Transaction(
                expectedAccountId,
                expectedAmount,
                ETransactionType.Transfer,
                ETransferType.Credit
            );

            // Expected result
            transaction.Id.Should().NotBeEmpty();
            transaction.AccountId.Should().Be(expectedAccountId);
            transaction.Amount.Should().Be(expectedAmount);
            transaction.Type.Should().Be(ETransactionType.Transfer);
            transaction.TransferType.Should().Be(ETransferType.Credit);
            transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        [Fact]
        [Trait("Models", "Transaction")]
        public void CreateTransaction_WithTransferDebit_SetsPropertiesCorrectly()
        {
            // Arrange
            var expectedAccountId = Guid.NewGuid();
            var expectedAmount = _faker.Finance.Amount();

            var transaction = new Transaction(
                expectedAccountId,
                expectedAmount,
                ETransactionType.Transfer,
                ETransferType.Debit
            );

            // Expected result
            transaction.Id.Should().NotBeEmpty();
            transaction.AccountId.Should().Be(expectedAccountId);
            transaction.Amount.Should().Be(expectedAmount);
            transaction.Type.Should().Be(ETransactionType.Transfer);
            transaction.TransferType.Should().Be(ETransferType.Debit);
            transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }
    }
}
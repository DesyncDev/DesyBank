using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using DesyBank.Domain.Models;
using FluentAssertions;
using Xunit;

namespace DesyBank.Tests.Models
{
    public class UserModelTests
    {
        // Faker
        private readonly Faker _faker = new("pt_BR");

        [Fact]
        [Trait("Models", "User")]
        public void UserConstructor_ShouldGenerateNonEmptyId()
        {
            var user = new User(
                _faker.Name.FullName(),
                _faker.Internet.Email(),
                BCrypt.Net.BCrypt.HashPassword(_faker.Internet.Password())
            );

            user.Id.Should().NotBeEmpty();
        }

        [Fact]
        [Trait("Models", "User")]
        public void UserConstructor_ReceivesValidValues_SetTheValuesCorrectly()
        {
            // Set the user valid values
            var expectedFullName = _faker.Name.FullName();
            var expectedEmail = _faker.Internet.Email();
            var password = _faker.Internet.Password();
            var expectedHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User(
                expectedFullName,
                expectedEmail,
                expectedHash
            );

            // Expected result
            user.Id.Should().NotBeEmpty();
            user.FullName.Should().Be(expectedFullName);
            user.Email.Should().Be(expectedEmail);
            user.HashPassword.Should().Be(expectedHash);
            user.IsBlocked.Should().BeFalse();
            user.JoinedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }
    }
}
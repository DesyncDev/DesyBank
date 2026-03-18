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
            var user = CreateValidUser();

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
            user.FullName.Should().Be(expectedFullName);
            user.Email.Should().Be(expectedEmail);
            user.HashPassword.Should().Be(expectedHash);
        }

        [Fact]
        [Trait("Models", "User")]
        public void UserConstructor_ReceivesValidValues_SetIsBlockedFalse()
        {
            var user = CreateValidUser();

            // Checks if IsBlocked is set to false.
            user.IsBlocked.Should().BeFalse();
        }

        [Fact]
        [Trait("Models", "User")]
        public void UserConstructor_ShouldSetJoinedAtToApproximatelyCurrentTime()
        {
            var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

            var user = CreateValidUser();

            var afterCreation = DateTime.UtcNow.AddSeconds(1);

            // Checks user JoinedAt
            user.JoinedAt.Should().BeOnOrBefore(afterCreation);
            user.JoinedAt.Should().BeOnOrAfter(beforeCreation);

        }

        // Método auxiliar para construção de usuário
        private User CreateValidUser()
        {
            return new User(
                _faker.Name.FullName(),
                _faker.Internet.Email(),
                BCrypt.Net.BCrypt.HashPassword(_faker.Internet.Password())
            );
        }
    }
}
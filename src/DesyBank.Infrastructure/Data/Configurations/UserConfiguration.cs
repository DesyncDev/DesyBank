using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesyBank.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(x => x.Id);

            builder.Property(fn => fn.FullName)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(254);

            builder.Property(hp => hp.HashPassword)
            .IsRequired();

            builder.Property(ja => ja.JoinedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

            // Indices
            builder.HasIndex(e => e.Email)
            .IsUnique();
        }
    }
}
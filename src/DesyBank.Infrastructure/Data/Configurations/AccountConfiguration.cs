using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesyBank.Infrastructure.Data.Configurations
{
    public class AccountConfiguration :IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("accounts");

            builder.HasKey(x => x.Id);

            builder.Property(ac => ac.AccountNumber)
            .IsRequired()
            .HasMaxLength(50);

            builder.Property(b => b.Balance)
            .IsRequired();

            builder.Property(ca => ca.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

            // Relações
            builder.HasOne(u => u.User)
            .WithOne()
            .HasForeignKey<Account>(fk => fk.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(ts => ts.Transactions)
            .WithOne(a => a.Account)
            .HasForeignKey(fk => fk.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.ReceivedTransfers)
            .WithOne(a => a.ToAccount)
            .HasForeignKey(fk => fk.ToAccountId)
            .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasMany(t => t.SentTransfers)
            .WithOne(a => a.FromAccount)
            .HasForeignKey(fk => fk.FromAccountId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
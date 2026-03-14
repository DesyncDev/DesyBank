using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DesyBank.Domain.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Cryptography.X509Certificates;

namespace DesyBank.Infrastructure.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("transactions");

            builder.HasKey(x => x.Id);

            builder.Property(am => am.Amount)
            .IsRequired();

            builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<int>();

            builder.Property(ca => ca.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");
        }
    }
}
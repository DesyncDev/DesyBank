using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesyBank.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesyBank.Infrastructure.Data.Configurations
{
    public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
    {
        public void Configure(EntityTypeBuilder<Transfer> builder)
        {
            builder.ToTable("transfers");

            builder.HasKey(x => x.Id);

            builder.Property(am => am.Amount)
            .IsRequired();

            builder.Property(ca => ca.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");
        }
    }
}
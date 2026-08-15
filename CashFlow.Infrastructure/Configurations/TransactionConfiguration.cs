using CashFlow.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Infrastructure.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Type)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(t => t.Description)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(t => t.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(t => t.Date)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()"); 
        }
    }
}


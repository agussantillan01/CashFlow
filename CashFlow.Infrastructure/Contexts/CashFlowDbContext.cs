using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashFlow.Infrastructure.Configurations;
using CashFlow.Core.Entities;
namespace CashFlow.Infrastructure.Data
{
    public class CashFlowDbContext : DbContext
    {
        public CashFlowDbContext(DbContextOptions<CashFlowDbContext> options) : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new TransactionConfiguration());

        }

    }
}

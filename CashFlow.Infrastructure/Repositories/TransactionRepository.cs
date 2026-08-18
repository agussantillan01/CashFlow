using CashFlow.Core.Entities;
using CashFlow.Core.Enums;
using CashFlow.Core.Interfaces;
using CashFlow.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly CashFlowDbContext _context;

        public TransactionRepository(CashFlowDbContext context)
        {
            _context = context;
        }
       public async Task<Transaction> AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.Transactions
                                             .OrderByDescending(t => t.Date)
                                             .ToListAsync();
        }

        public async Task<decimal> GetTotalExpenseAsync()
        {
            return await _context.Transactions
                                             .Where(t => t.Type == TransactionType.Expense)
                                             .SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetTotalIncomeAsync()
        {
            return await _context.Transactions
                                             .Where(t => t.Type == TransactionType.Income)
                                             .SumAsync(t => t.Amount);
        }

        public async Task<Transaction> AddExpenseAtomicAsync(Transaction transaction)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);

            try
            {
                var income = await _context.Transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .SumAsync(t => t.Amount);

                var expense = await _context.Transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .SumAsync(t => t.Amount);

                var currentBalance = income - expense;

                if (currentBalance < transaction.Amount)
                {
                    throw new System.InvalidOperationException("Saldo insuficiente para registrar este gasto.");
                }

                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return transaction;
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }
    }
}

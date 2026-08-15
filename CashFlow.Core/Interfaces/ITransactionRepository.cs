using CashFlow.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Core.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<decimal> GetTotalIncomeAsync();
        Task<decimal> GetTotalExpenseAsync();
    }
}

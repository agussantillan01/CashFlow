using CashFlow.Core.DTOs;
using CashFlow.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> AddIncomeAsync(CreateTransactionDto dto);
        Task<Transaction> AddExpenseAsync(CreateTransactionDto dto);
        Task<BalanceDto> GetBalanceAsync();
        Task<IEnumerable<Transaction>> GetHistoryAsync();
    }
}

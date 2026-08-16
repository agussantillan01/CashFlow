using CashFlow.Core.DTOs;
using CashFlow.Core.Entities;
using CashFlow.Core.Enums;
using CashFlow.Core.Interfaces;
using CashFlow.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Services.services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Transaction> AddIncomeAsync(CreateTransactionDto dto)
        {
            var transaction = new Transaction
            {
                Type = TransactionType.Income,
                Description = dto.Description,
                Amount = dto.Amount,
                Date = dto.Date ?? DateTime.Now
            };

            return await _repository.AddAsync(transaction);
        }

        public async Task<Transaction> AddExpenseAsync(CreateTransactionDto dto)
        {
            var balance = await GetBalanceAsync();

            if (balance.NetBalance < dto.Amount)
            {
                throw new InvalidOperationException("Saldo insuficiente para registrar este gasto.");
            }

            var transaction = new Transaction
            {
                Type = TransactionType.Expense,
                Description = dto.Description,
                Amount = dto.Amount,
                Date = dto.Date ?? DateTime.Now
            };

            return await _repository.AddAsync(transaction);
        }

        public async Task<BalanceDto> GetBalanceAsync()
        {
            var totalIncome = await _repository.GetTotalIncomeAsync();
            var totalExpense = await _repository.GetTotalExpenseAsync();

            return new BalanceDto
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense
            };
        }

        public async Task<IEnumerable<Transaction>> GetHistoryAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}

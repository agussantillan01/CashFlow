using System;
using System.Threading.Tasks;
using CashFlow.Core.DTOs;
using CashFlow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // 1. Registrar Ingreso
        [HttpPost("income")]
        public async Task<IActionResult> AddIncome([FromBody] CreateTransactionDto dto)
        {
            if (dto.Amount <= 0)
                return BadRequest("El monto del ingreso debe ser mayor a cero.");

            var transaction = await _transactionService.AddIncomeAsync(dto);

            return StatusCode(201, transaction);
        }

        // 2. Registrar Gasto
        [HttpPost("expense")]
        public async Task<IActionResult> AddExpense([FromBody] CreateTransactionDto dto)
        {
            if (dto.Amount <= 0)
                return BadRequest("El monto del gasto debe ser mayor a cero.");

            try
            {
                var transaction = await _transactionService.AddExpenseAsync(dto);
                return StatusCode(201, transaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error interno en el servidor." });
            }
        }

        // 3. Consultar Balance
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var balance = await _transactionService.GetBalanceAsync();
            return Ok(balance); // 200 OK
        }

        // 4. Historial
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var history = await _transactionService.GetHistoryAsync();
            return Ok(history);
        }
    }
}

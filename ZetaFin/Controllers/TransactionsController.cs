using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("Usuário não autenticado");

        return Guid.Parse(userIdClaim);
    }

    /// <summary>
    /// Cria uma nova transação (receita ou despesa)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TransactionDto>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto dto)
    {
        try
        {
            var userId = GetUserId();
            var transaction = await _transactionService.CreateAsync(userId, dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = transaction.Id },
                new ApiResponse<TransactionDto>
                {
                    Success = true,
                    Message = "Transação criada com sucesso",
                    Data = transaction
                });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Error = new ErrorDetails
                {
                    Code = "VALIDATION_ERROR",
                    Message = ex.Message
                }
            });
        }
    }

    /// <summary>
    /// Lista transações com filtros e paginação
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<TransactionListDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] TransactionQueryDto query)
    {
        try
        {
            var userId = GetUserId();
            var result = await _transactionService.GetFilteredAsync(userId, query);

            return Ok(new ApiResponse<TransactionListDto>
            {
                Success = true,
                Data = result
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Error = new ErrorDetails
                {
                    Code = "QUERY_ERROR",
                    Message = ex.Message
                }
            });
        }
    }

    /// <summary>
    /// Obtém uma transação específica por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TransactionDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var transaction = await _transactionService.GetByIdAsync(id);
            if (transaction == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "RESOURCE_NOT_FOUND",
                        Message = "Transação não encontrada"
                    }
                });

            // Verificar se a transação pertence ao usuário
            var userId = GetUserId();
            if (transaction.UserId != userId)
                return Forbid();

            return Ok(new ApiResponse<TransactionDto>
            {
                Success = true,
                Data = transaction
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Error = new ErrorDetails
                {
                    Code = "QUERY_ERROR",
                    Message = ex.Message
                }
            });
        }
    }

    /// <summary>
    /// Atualiza uma transação existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TransactionDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTransactionDto dto)
    {
        try
        {
            var transaction = await _transactionService.UpdateAsync(id, dto);

            return Ok(new ApiResponse<TransactionDto>
            {
                Success = true,
                Message = "Transação atualizada com sucesso",
                Data = transaction
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Error = new ErrorDetails
                {
                    Code = "UPDATE_ERROR",
                    Message = ex.Message
                }
            });
        }
    }

    /// <summary>
    /// Deleta uma transação
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await _transactionService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Error = new ErrorDetails
                    {
                        Code = "RESOURCE_NOT_FOUND",
                        Message = "Transação não encontrada"
                    }
                });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Transação deletada com sucesso"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Error = new ErrorDetails
                {
                    Code = "DELETE_ERROR",
                    Message = ex.Message
                }
            });
        }
    }

    /// <summary>
    /// Obtém resumo financeiro
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<DetailedSummaryDto>), 200)]
    public async Task<IActionResult> GetSummary(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? month)
    {
        try
        {
            var userId = GetUserId();

            // Se month foi fornecido, calcular startDate e endDate
            if (!string.IsNullOrEmpty(month))
            {
                if (DateTime.TryParse($"{month}-01", out var monthDate))
                {
                    startDate = new DateTime(monthDate.Year, monthDate.Month, 1);
                    endDate = startDate.Value.AddMonths(1).AddDays(-1);
                }
            }

            var summary = await _transactionService.GetSummaryAsync(userId, startDate, endDate);

            return Ok(new ApiResponse<DetailedSummaryDto>
            {
                Success = true,
                Data = summary
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Error = new ErrorDetails
                {
                    Code = "SUMMARY_ERROR",
                    Message = ex.Message
                }
            });
        }
    }
}

// Classes auxiliares para respostas padronizadas
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public ErrorDetails? Error { get; set; }
}

public class ErrorDetails
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? Details { get; set; }
}
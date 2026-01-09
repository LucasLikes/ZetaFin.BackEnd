using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZetaFin.Application.DTOs;
using ZetaFin.Application.Interfaces;
using ZetaFin.Domain.Entities;
using ZetaFin.Domain.Interfaces;

namespace ZetaFin.Application.Services;

public class DepositService : IDepositService
{
    private readonly IGoalRepository _goalRepository;
    private readonly ExpenseCategories _depositRepository;

    public DepositService(IGoalRepository goalRepository, ExpenseCategories depositRepository)
    {
        _goalRepository = goalRepository;
        _depositRepository = depositRepository;
    }

    public async Task<DepositDto> AddDepositAsync(CreateDepositDto dto)
    {
        var goal = await _goalRepository.GetByIdAsync(dto.GoalId);
        if (goal == null)
            throw new Exception("Goal not found");

        var deposit = new Deposit(dto.Amount, dto.Date, dto.Source, dto.GoalId, dto.UserId);
        goal.AddDeposit(deposit); // atualiza valor acumulado

        await _depositRepository.AddAsync(deposit);
        await _goalRepository.SaveChangesAsync(); // garantir atualização no CurrentAmount

        return new DepositDto
        {
            Id = deposit.Id,
            Amount = deposit.Amount,
            Date = deposit.Date,
            Source = deposit.Source
        };
    }

    public async Task<IEnumerable<DepositDto>> GetDepositsByGoalIdAsync(Guid goalId)
    {
        var deposits = await _depositRepository.GetByGoalIdAsync(goalId);
        return deposits.Select(d => new DepositDto
        {
            Id = d.Id,
            Amount = d.Amount,
            Date = d.Date,
            Source = d.Source
        });
    }
}

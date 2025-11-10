using AutoMapper;
using ZetaFin.Domain.Entities;
using ZetaFin.Application.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Expense, ExpenseDto>();
    }
}
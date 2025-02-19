using Romb.Application.Dtos;
using Romb.Application.Exceptions;

namespace Romb.Application.Extensions;

public static class PlannedEventRequestDtoExtension
{
    public static void CheckValidity(this PlannedEventRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name cannot be empty.");

        if (dto.PlannedCofinanceRate < 0 || dto.PlannedCofinanceRate > 100)
            throw new IncorrectValueException("Incorrect cofinance rate.");

        if (dto.TotalBudget <= 0 || dto.TotalBudget > decimal.MaxValue)
            throw new IncorrectValueException("Incorrect total budget value.");
    }
}

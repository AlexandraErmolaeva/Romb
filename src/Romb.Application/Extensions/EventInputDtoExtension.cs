using Romb.Application.Dtos;
using Romb.Application.Exceptions;

namespace Romb.Application.Extensions;

public static class EventInputDtoExtension
{
    public static void CheckValidity(this EventInputDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name cannot be empty.");

        if (dto.CofinanceRate < 0 || dto.CofinanceRate > 100)
            throw new CofinanceRateIncorrectValueException("Incorrect cofinance rate.");

        if (dto.TotalBudget <= 0 || dto.TotalBudget > decimal.MaxValue)
            throw new TotalBudgetIncorrectValueException("Incorrect total budget value.");
    }
}

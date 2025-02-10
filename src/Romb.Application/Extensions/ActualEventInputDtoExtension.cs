﻿using Romb.Application.Dtos;
using Romb.Application.Exceptions;

namespace Romb.Application.Extensions;

public static class ActualEventInputDtoExtension
{
    public static void CheckValidity(this ActualEventInputDto dto)
    {
        if (dto.PlannedEventId < 0)
            throw new IncorrectValueException("Incorrect planned event ID.");

        if (dto.CompletedWorksBudget <= 0 || dto.CompletedWorksBudget > decimal.MaxValue)
            throw new IncorrectValueException("Incorrect completed works budget value.");
    }
}

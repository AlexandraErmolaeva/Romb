using Romb.Application.Attribute;

namespace Romb.Application.Dtos;

public class ActualEventInputDto
{
    public long PlannedEventId { get; set; }

    [DigitsOnly]
    [AcceptibleValueForDecimal]
    public decimal CompletedWorksBudget { get; set; }
}

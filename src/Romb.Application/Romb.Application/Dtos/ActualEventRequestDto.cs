using Romb.Application.Attribute;
using System.ComponentModel.DataAnnotations;

namespace Romb.Application.Dtos;

public class ActualEventRequestDto
{
    [Required(ErrorMessage = "Id is required.")]
    public long PlannedEventId { get; set; }

    [DigitsOnly]
    [AcceptibleValueForDecimal]
    public decimal CompletedWorksBudget { get; set; }
}

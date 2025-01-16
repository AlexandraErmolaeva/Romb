using Romb.Application.Attribute;
using System.ComponentModel.DataAnnotations;

namespace Romb.Application.Dtos;

public class EventInputDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string Name { get; set; }

    // TODO: разобраться с децималом.
    [Required(ErrorMessage = "Value is required.")]
    [DigitsOnly]
    [AcceptibleValueForDecimal]
    public decimal TotalBudget { get; set; }

    [Required(ErrorMessage = "Value is required.")]
    [DigitsOnly]
    [Range(0, 100, ErrorMessage = "Cofinance rate is incorrect.")]
    public decimal CofinanceRate { get; set; }

}

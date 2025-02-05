﻿using Romb.Application.Attribute;
using System.ComponentModel.DataAnnotations;

namespace Romb.Application.Dtos;

public class EventInputDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string Name { get; set; }

    [DigitsOnly]
    [AcceptibleValueForDecimal]
    public decimal TotalBudget { get; set; }

    [DigitsOnly]
    [Range(0, 100, ErrorMessage = "Incorrect value for field.")]
    public decimal CofinanceRate { get; set; }
} 

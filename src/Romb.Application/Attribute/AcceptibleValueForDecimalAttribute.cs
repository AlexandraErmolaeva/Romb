using System.ComponentModel.DataAnnotations;

namespace Romb.Application.Attribute;

public class AcceptibleValueForDecimalAttribute : ValidationAttribute
{
    public decimal Min { get; set; }
    public decimal Max { get; set; }

    public AcceptibleValueForDecimalAttribute() : base($"Incorrect value for field.")
    {
        Min = 0M;
        Max = decimal.MaxValue;
    }

    public override bool IsValid(object value)
    {
        if (value == null)
            return true;

        if (value is decimal decimalValue)
            return decimalValue >= Min && decimalValue <= Max;

        return false;
    }
}

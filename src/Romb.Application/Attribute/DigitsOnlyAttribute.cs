using System.ComponentModel.DataAnnotations;

namespace Romb.Application.Attribute;

public class DigitsOnlyAttribute : ValidationAttribute
{
    public DigitsOnlyAttribute() : base("The field can contains digits only.") { }

    public override bool IsValid(object value)
    {
        if (value == null)
            return true;

        if (value is decimal decimalValue)
            return decimal.TryParse(decimalValue.ToString(), out _);

        return false;
    }
}

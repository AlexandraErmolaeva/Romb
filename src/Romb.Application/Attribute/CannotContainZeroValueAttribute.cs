using System.ComponentModel.DataAnnotations;

namespace Romb.Application.Attribute
{
    public class CannotContainZeroValueAttribute : ValidationAttribute
    {
        public CannotContainZeroValueAttribute() : base("Cannot contain zero value.") { }

        public override bool IsValid(object value)
        {
            if (value is null)
                return false;

            if (value is decimal decimalValue)
                return decimalValue > 0 ? true : false;

            return false;
        }
    }
}

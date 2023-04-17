using System;

namespace Org.OpenOrbit.Libraries.JpegXr.Exceptions
{
    public class InvalidFormatException : FormatException
    {
        public InvalidFormatException(string field, object expected, object actual)
            : base($"Format Error: Value of {field} should be '{expected}' but is '{actual}'.")
        {
        }
    }
}
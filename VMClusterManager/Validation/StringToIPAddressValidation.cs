using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;
using System.Net;

namespace VMClusterManager.Validation
{
    public class StringToIPAddressValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            
            try
            {
                IPAddress.Parse(value as string);
                return ValidationResult.ValidResult;
            }
            catch (FormatException)
            {
                return new ValidationResult(false, "IP address is not valid!");
            }
            catch (ArgumentNullException)
            {
                return new ValidationResult (false, "IP address required!");
            }
        //    if (DateTime.Now.Date > date)
        //    {
        //        return new ValidationResult(false, "Please enter a date in the future.");
        //    }
        //    else
        //    {
        //        return ValidationResult.ValidResult;
        //    }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class CalculatorException : ApplicationException
    {
        public CalculatorException(String message) : base(message)
        {

        }
    }
}

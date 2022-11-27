using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils
{
    public static class Utils
    {
        public static bool IsNumber(object value) {
            return value is int || value is float;
        }
    }
}

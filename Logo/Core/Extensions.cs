using Logo.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core
{
    public static class Extensions
    {
        public static string ToString(this Position position)
        {
            return "Position(line=" + (position.line + 1) + ",column=" + (position.column + 1) + ")";
        }
    }
}

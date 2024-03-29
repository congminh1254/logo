﻿using Logo.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Logo.Core.Utils.Grammar;

namespace Logo.Core
{
    public static class Extensions
    {
        public static string ToString(this Position position)
        {
            return "Position(line=" + (position.line + 1) + ",column=" + (position.column + 1) + ")";
        }

        public static Color ToColor(this uint argb)
        {
            return Color.FromArgb(byte.MaxValue,
                                  (byte)((argb & 0xff0000) >> 0x10),
                                  (byte)((argb & 0xff00) >> 8),
                                  (byte)(argb & 0xff));
        }
    }
}

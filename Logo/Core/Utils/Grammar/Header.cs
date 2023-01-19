using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils.Grammar
{
    public class Header
    {
        public int width { get; private set; }
        public int height { get; private set; }
        public Header(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDViewer
{
    public struct ColorRGB
    {
        public byte r;
        public byte g;
        public byte b;

        public override string ToString()
        {
            return $"R:{r} G:{g} B:{b}";
        }
    }
}

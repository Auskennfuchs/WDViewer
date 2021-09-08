using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDViewer
{
    class Entry
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public UInt16 Width { get; set; }
        public UInt16 Height { get; set; }
    }
}

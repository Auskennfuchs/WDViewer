using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDViewer
{
    class Asset
    {
        public string Path { get; set; }
        public int FileSize { get; set; }
        public uint FileOffset { get; set; }

        public byte[] Content { get; set; }
    }
}

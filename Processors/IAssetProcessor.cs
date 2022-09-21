using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDViewer.Assets;

namespace WDViewer.Reader
{
    interface IAssetProcessor
    {
        Tuple<bool, Asset> Read(byte[] content, string path, Dictionary<string, Asset> entries);
    }
}

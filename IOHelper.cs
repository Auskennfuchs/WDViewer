using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WDViewer
{
    class IOHelper
    {
        public static T ByteToType<T>(byte[] bytes)
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        public static T ReadTypeFromReader<T>(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(Marshal.SizeOf<T>());
            return ByteToType<T>(bytes);

        }
    }
}

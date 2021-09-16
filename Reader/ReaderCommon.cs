using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WDViewer.Assets;

namespace WDViewer.Reader
{
    public class ReaderCommon
    {
        public static uint ASSET_PALETTE_COUNT = 256;

        public struct ImageSize
        {
            public ushort width;
            public ushort height;
        }

        public static Image CreateImageRGB565(int width, int height, byte[] imageBytes)
        {
            var img = new Bitmap(width, height);
            var data = img.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            var ptr = data.Scan0;
            var size = Math.Abs(data.Stride) * height;
            Marshal.Copy(imageBytes, 0, ptr, size);
            img.UnlockBits(data);
            return img;
        }

        public static Image CreateImageRGBA(int width, int height, byte[] imageBytes)
        {
            var img = new Bitmap(width, height);
            var data = img.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var ptr = data.Scan0;
            var size = Math.Abs(data.Stride) * height;
            Marshal.Copy(imageBytes, 0, ptr, size);
            img.UnlockBits(data);
            return img;
        }

        public static Image CreatePaletteImage(int width, int height, byte[] imageBytes, AssetPalette palette, bool transparent)
        {
            var img = new Bitmap(width, height);
            var data = img.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var ptr = data.Scan0;
            var size = Math.Abs(data.Stride) * height;

            var colorBytes = new byte[size];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var paletteIndex = (int)imageBytes[x + y * width];
                    var color = palette.Palette[paletteIndex];
                    var isTransparent = transparent && paletteIndex == 0;
                    colorBytes[(x + y * width) * 4 + 3] = (byte)(isTransparent ? 0 : 255);
                    colorBytes[(x + y * width) * 4 + 2] = (byte)(isTransparent ? 255 : color.r);
                    colorBytes[(x + y * width) * 4 + 1] = color.g;
                    colorBytes[(x + y * width) * 4 + 0] = (byte)(isTransparent ? 255 : color.b);

                    var isHalfTransparent = transparent && paletteIndex == palette.Palette.Length - 2;
                    var isQuarterTransparent = transparent && paletteIndex == palette.Palette.Length - 3;
                    if (isHalfTransparent)
                    {
                        colorBytes[(x + y * width) * 4 + 3] = 128;
                        colorBytes[(x + y * width) * 4 + 2] = 0;
                        colorBytes[(x + y * width) * 4 + 1] = 0;
                        colorBytes[(x + y * width) * 4 + 0] = 0;
                    }
                    if (isQuarterTransparent)
                    {
                        colorBytes[(x + y * width) * 4 + 3] = 96;
                        colorBytes[(x + y * width) * 4 + 2] = 0;
                        colorBytes[(x + y * width) * 4 + 1] = 0;
                        colorBytes[(x + y * width) * 4 + 0] = 0;
                    }
                }
            }
            Marshal.Copy(colorBytes, 0, ptr, size);
            img.UnlockBits(data);
            return img;
        }
    }
}

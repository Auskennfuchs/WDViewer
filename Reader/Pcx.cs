﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDViewer.Reader
{
    public class Pcx
    {
        public Pcx()
        {
        }

        ushort xmin;
        ushort xmax;
        ushort ymin;
        ushort ymax;

        bool with_alpha;

        public void ReadFromStream(Stream stream, int translucentIndex, int transparentIndex)
        {
            using var binReader = new BinaryReader(stream);
            with_alpha = translucentIndex != -1 || transparentIndex != -1;

            byte magic = binReader.ReadByte();// Util.ReadByte(stream);
            if (magic != 0x0A)
                throw new Exception("stream is not a valid .pcx file");

            /*version =*/
            stream.ReadByte();//Util.ReadByte(stream);
            /*encoding =*/
            stream.ReadByte(); //Util.ReadByte(stream);
            ushort bpp = binReader.ReadByte(); //Util.ReadByte(stream);
            xmin = binReader.ReadUInt16();// Util.ReadWord(stream);
            ymin = binReader.ReadUInt16(); //Util.ReadWord(stream);
            xmax = binReader.ReadUInt16(); //Util.ReadWord(stream);
            ymax = binReader.ReadUInt16(); //Util.ReadWord(stream);
            /*ushort h_dpi =*/
            binReader.ReadUInt16(); //Util.ReadWord(stream);
            /*ushort v_dpi =*/
            binReader.ReadUInt16(); //Util.ReadWord(stream);
            stream.Seek(48, SeekOrigin.Current); /* skip the header palette */
            stream.Seek(1, SeekOrigin.Current); //stream.Position++;    /* skip the reserved byte */
            ushort numplanes = binReader.ReadByte();// Util.ReadByte(stream);
            /*ushort stride =*/
            binReader.ReadUInt16(); //Util.ReadWord(stream);
            /*headerInterp =*/
            binReader.ReadUInt16(); //Util.ReadWord(stream);
            /*videoWidth =*/
            binReader.ReadUInt16(); //Util.ReadWord(stream);
            /*videoHeight =*/
            binReader.ReadUInt16(); //Util.ReadWord(stream);
            stream.Seek(54, SeekOrigin.Current); //stream.Position += 54;

            if (bpp != 8 || numplanes != 1)
                throw new Exception("unsupported .pcx image type");

            width = (ushort)(xmax - xmin + 1);
            height = (ushort)(ymax - ymin + 1);

            long imageData = stream.Position;

            stream.Position = stream.Length - 256 * 3;
            /* read the palette */
            palette = new byte[256 * 3];
            stream.Read(palette, 0, 256 * 3);

            stream.Position = imageData;

            Debug.WriteLine("imageData begins at {0}", imageData);

            /* now read the image data */
            data = new byte[width * height * 4];

            int idx = 0;
            while (idx < data.Length)
            {
                byte b = binReader.ReadByte();// Util.ReadByte(stream);
                byte count;
                byte value;

                if ((b & 0xC0) == 0xC0)
                {
                    /* it's a count byte */
                    count = (byte)(b & 0x3F);
                    value = binReader.ReadByte();// Util.ReadByte(stream);
                }
                else
                {
                    count = 1;
                    value = b;
                }

                for (int i = 0; i < count; i++)
                {
                    if (idx + 4 > data.Length)
                        return;

                    /* this stuff is endian
					 * dependent... for big endian
					 * we need the "idx +"'s
					 * reversed */
                    data[idx + 3] = 0xff;
                    data[idx + 2] = palette[value * 3 + 0];
                    data[idx + 1] = palette[value * 3 + 1];
                    data[idx + 0] = palette[value * 3 + 2];
                    if (with_alpha)
                    {
                        if (value == translucentIndex)
                            data[idx + 3] = 0xd0;
                        else if (value == transparentIndex)
                            data[idx + 3] = 0x00;
                        else
                            data[idx + 3] = 0xff;
                    }

                    idx += 4;
                }
            }
        }


        byte[] data;
        byte[] palette;

        ushort width;
        ushort height;

        public byte[] RgbaData
        {
            get { return data; }
        }

        public byte[] RgbData
        {
            get
            {
                byte[] foo = new byte[width * height * 3];
                int i = 0;
                int j = 0;
                while (i < data.Length)
                {
                    foo[j++] = data[i++];
                    foo[j++] = data[i++];
                    foo[j++] = data[i++];
                    i++;
                }
                return foo;
            }
        }

        public byte[] Palette
        {
            get { return palette; }
        }

        public ushort Width
        {
            get { return width; }
        }

        public ushort Height
        {
            get { return height; }
        }

        public ushort Depth
        {
            get { return (ushort)(with_alpha ? 32 : 24); }
        }

        public ushort Stride
        {
            get { return (ushort)(width * (3 + (with_alpha ? 1 : 0))); }
        }
    }
}

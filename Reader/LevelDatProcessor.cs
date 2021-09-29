using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDViewer.Assets;

namespace WDViewer.Reader
{
    class LevelDatProcessor : IAssetProcessor
    {
        private static readonly uint TILESET_MAX = 252;
        private static readonly uint LEVEL_SIZE_MAX = 128;
        private static readonly uint ENTITIES_PER_SECTION = 256;
        public Tuple<bool, Asset> Read(byte[] content, string path, Dictionary<string, Asset> entries)
        {
            var ext = path.Substring(path.Length - 4, 4);
            var fileWithoutExt = path[0..^4];

            if (ext != ".DAT" || !(path.StartsWith("LEVEL/data/") || path.StartsWith("LEVEL2/data/")))
            {
                return Tuple.Create<bool, Asset>(false, null);
            }

            using var stream = new MemoryStream(content);
            using var binReader = new BinaryReader(stream);

            var name = "";
            for (var i = 0; i < 32; i++)
            {
                var c = Convert.ToChar(binReader.ReadByte());
                if (c == 0)
                {
                    break;
                }
                if (c >= 0x7f)
                {
                    c = ' ';
                }
                name += c;
            }
            name = name.TrimStart();

            stream.Seek(0xF627, SeekOrigin.Begin);
            var width = binReader.ReadUInt32();
            var height = binReader.ReadUInt32();

            stream.Seek(0xF653, SeekOrigin.Begin);
            var tileSetIndex = binReader.ReadUInt32();

            var tiles = ReadTiles(width, height, stream, binReader);
            var entities = ReadMapEntities(stream, binReader);

            return Tuple.Create<bool, Asset>(true, new AssetLevel()
            {
                Path = path,
                Name = name,
                Width = width,
                Height = height,
                TileSetIndex = tileSetIndex,
                Tiles = tiles,
                Entities = entities,
            });
        }

        private AssetLevel.Tile[,] ReadTiles(uint width, uint height, Stream stream, BinaryReader binReader)
        {
            var tiles = new AssetLevel.Tile[width, height];
            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = y + LEVEL_SIZE_MAX * x;
                    stream.Seek(0x801F + i, SeekOrigin.Begin);
                    tiles[x, y].tileIndex = binReader.ReadByte();

                    stream.Seek(0x001F + i * 2, SeekOrigin.Begin);
                    var flags = binReader.ReadUInt16();
                    switch (flags)
                    {
                        case 0x0001: //free
                            tiles[x, y].flags |= AssetLevel.TileFlags.Passable;
                            break;
                        case 0x0002: //Water
                            tiles[x, y].flags |= AssetLevel.TileFlags.Water | AssetLevel.TileFlags.Immutable | AssetLevel.TileFlags.Passable;
                            break;
                        case 0x0008: //Shore
                            tiles[x, y].flags |= AssetLevel.TileFlags.Shore | AssetLevel.TileFlags.Immutable | AssetLevel.TileFlags.Passable;
                            break;
                        case 0x0011: //Blocked
                            tiles[x, y].flags |= AssetLevel.TileFlags.Immutable;
                            break;
                        case 0x0021: //Ore
                            tiles[x, y].flags |= AssetLevel.TileFlags.Passable;
                            tiles[x, y].ore = 30 * 200;
                            break;
                        case 0x0041: //Sand
                            tiles[x, y].flags |= AssetLevel.TileFlags.Passable | AssetLevel.TileFlags.Sand;
                            break;
                        case 0x0061: //unknown
                            tiles[x, y].flags |= AssetLevel.TileFlags.Passable;
                            break;
                        default:
                            Debug.WriteLine($"unknown flag ${flags}");
                            break;
                    }
                }
            }
            return tiles;
        }

        private List<AssetLevel.Entity> ReadMapEntities(Stream stream, BinaryReader binReader)
        {
            var res = new List<AssetLevel.Entity>();
            stream.Seek(0xE229, SeekOrigin.Begin);
            for (var i = 1; i <= ENTITIES_PER_SECTION; ++i)
            {
                var index = binReader.ReadUInt16();
                var type = binReader.ReadUInt16();
                var x = binReader.ReadUInt16();
                var y = binReader.ReadUInt16();
                var sprite = binReader.ReadUInt16();
                if (index != i || sprite == 0)
                {
                    continue;
                }
                res.Add(new AssetLevel.Entity()
                {
                    x = x,
                    y = y,
                    objectTileIndex = sprite,
                    type = type,
                });
            }
            return res;
        }
    }
}

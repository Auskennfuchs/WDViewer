using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDViewer.Assets
{
    public class AssetLevel : Asset
    {
        public enum TileFlags
        {
            Passable = 1 << 0,
            Water = 1 << 1,
            Shore = 1 << 2,
            Immutable = 1 << 3,
            Sand = 1 << 4,
            EntityTerrain = 1 << 5,
            EntityWater = 1 << 6,
        }

        public struct Tile
        {
            public byte tileIndex;
            public TileFlags flags;
            public uint ore;

        }

        public struct Entity
        {
            public ushort objectTileIndex;
            public ushort type;
            public ushort x;
            public ushort y;
        }

        public String Name { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint TileSetIndex { get; set; }

        public Tile[,] Tiles { get; set; }

        public List<Entity> Entities { get; set; }

        public String TileSetPath
        {
            get
            {
                return $"MIX/SPRT{TileSetIndex}.MIX";
            }
        }

        public String ObjectSetPath
        {
            get
            {
                return $"MIX/SPRO{TileSetIndex}.MIX";
            }
        }
    }
}

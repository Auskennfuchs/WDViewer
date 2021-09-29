using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WDViewer.Assets;
using static WDViewer.Assets.AssetLevel;

namespace WDViewer.AssetControls
{
    class LevelPictureBox : PictureBox
    {
        private static readonly int TILE_SIZE = 64;
        private AssetLevel level;
        private Dictionary<string, Asset> assets;

        private AssetMix tileSet;
        private AssetMix objectSet;

        public bool ShowFlags
        {
            get
            {
                return showFlags;
            }
            set
            {
                showFlags = value;
                this.Invalidate();
            }
        }
        private bool showFlags;

        public LevelPictureBox(AssetLevel level, Dictionary<string, Asset> assets)
        {
            this.level = level;
            this.assets = assets;
            this.Width = (int)level.Width * TILE_SIZE;
            this.Height = (int)level.Height * TILE_SIZE;
            tileSet = assets[level.TileSetPath] as AssetMix;
            objectSet = assets[level.ObjectSetPath] as AssetMix;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.FillRectangle(Brushes.Red, 0, 0, Width, Height);
            var notPassableBrush = new SolidBrush(Color.FromArgb(40, Color.Red));
            var waterBrush = new SolidBrush(Color.FromArgb(40, Color.Blue));
            var oreBrush = new SolidBrush(Color.FromArgb(40, Color.Yellow));
            var shoreBrush = new SolidBrush(Color.FromArgb(40, Color.LightBlue));
            for (var y = 0; y < level.Height; ++y)
            {
                for (var x = 0; x < level.Width; ++x)
                {
                    var tile = level.Tiles[x, y];
                    var tileSprite = tileSet.Content[tile.tileIndex] as AssetImage;
                    pe.Graphics.DrawImage(tileSprite.Image, x * TILE_SIZE, y * TILE_SIZE);
                    if (showFlags)
                    {
                        Brush brush = null;
                        if (!tile.flags.HasFlag(TileFlags.Passable))
                        {
                            brush = notPassableBrush;
                        }
                        if (tile.flags.HasFlag(TileFlags.Water))
                        {
                            brush = waterBrush;
                        }
                        if (tile.flags.HasFlag(TileFlags.Shore))
                        {
                            brush = shoreBrush;
                        }
                        if (tile.ore > 0)
                        {
                            brush = oreBrush;
                        }
                        if (brush != null)
                        {
                            pe.Graphics.FillRectangle(brush, x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                        }
                    }
                }
            }
            foreach (var entity in level.Entities)
            {
                if (entity.objectTileIndex < objectSet.Content.Count)
                {
                    var objectSprite = objectSet.Content[entity.objectTileIndex] as AssetImage;
                    pe.Graphics.DrawImage(objectSprite.Image, entity.x * TILE_SIZE, entity.y * TILE_SIZE);
                }
            }
            base.OnPaint(pe);
        }

    }
}

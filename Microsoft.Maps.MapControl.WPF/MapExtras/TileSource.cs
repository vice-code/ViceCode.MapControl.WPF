using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.Maps.MapExtras
{
    internal abstract class TileSource
    {
        public abstract long FinestLodWidth { get; }

        public abstract long FinestLodHeight { get; }

        public abstract int TileWidth { get; }

        public abstract int TileHeight { get; }

        public abstract int MinimumLevelOfDetail { get; }

        public abstract int MaximumLevelOfDetail { get; }

        public abstract TimeSpan? TileFadeInDuration { get; set; }

        public abstract Rect? Clip { get; set; }

        public abstract Matrix3D Transform { get; }

        public abstract event EventHandler NewTilesAvailable;

        public abstract void SetRelevantTiles(IList<Tuple<TileId, int?>> relevantTiles);

        public abstract void GetTile(
          TileId tileId,
          out TileRenderable tileRenderable,
          out bool tileWillNeverBeAvailable);
    }
}

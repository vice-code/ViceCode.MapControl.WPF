using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Maps.MapExtras
{
    internal class RelevantTileSet
    {
        private List<Tuple<TileId, int?>> relevantTiles = new List<Tuple<TileId, int?>>();
        private List<Tuple<TileId, int?>> previousRelevantTiles = new List<Tuple<TileId, int?>>();
        private readonly HashSet<TileId> relevantTilesSet = new HashSet<TileId>();
        private readonly List<TileId> removedTiles = new List<TileId>();

        public void SetRelevantTiles(IList<Tuple<TileId, int?>> relevantTilesList)
        {
            VectorMath.Swap(ref relevantTiles, ref previousRelevantTiles);
            relevantTiles.Clear();
            relevantTilesSet.Clear();
            removedTiles.Clear();
            foreach (var relevantTiles in relevantTilesList)
            {
                this.relevantTiles.Add(relevantTiles);
                relevantTilesSet.Add(relevantTiles.Item1);
            }
            relevantTiles.Sort((left, right) =>
           {
               var num1 = left.Item2 ?? int.MinValue;
               var num2 = right.Item2 ?? int.MinValue;
               if (num1 == num2)
                   return left.Item1.CompareTo(right.Item1);
               return -num1.CompareTo(num2);
           });
            foreach (var previousRelevantTile in previousRelevantTiles)
            {
                if (!relevantTilesSet.Contains(previousRelevantTile.Item1))
                    removedTiles.Add(previousRelevantTile.Item1);
            }
        }

        public bool Contains(TileId tileId) => relevantTilesSet.Contains(tileId);

        public IList<Tuple<TileId, int?>> RelevantTiles => new ReadOnlyCollection<Tuple<TileId, int?>>(relevantTiles);

        public IList<TileId> RemovedTiles => new ReadOnlyCollection<TileId>(removedTiles);
    }
}

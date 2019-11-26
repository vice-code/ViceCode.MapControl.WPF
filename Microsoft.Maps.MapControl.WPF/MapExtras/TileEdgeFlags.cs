namespace Microsoft.Maps.MapExtras
{
    internal struct TileEdgeFlags
    {
        public bool IsLeftEdge { get; private set; }

        public bool IsRightEdge { get; private set; }

        public bool IsTopEdge { get; private set; }

        public bool IsBottomEdge { get; private set; }

        public TileEdgeFlags(bool isLeftEdge, bool isRightEdge, bool isTopEdge, bool isBottomEdge)
        {
            IsLeftEdge = isLeftEdge;
            IsRightEdge = isRightEdge;
            IsTopEdge = isTopEdge;
            IsBottomEdge = isBottomEdge;
        }
    }
}

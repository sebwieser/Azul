using System.Collections.Generic;

namespace Azul
{
    public interface ITileBagState
    {
        int RemainingTiles { get; }
        IEnumerable<Tile> Tiles { get; }
    }
}
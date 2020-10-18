using System.Collections.Generic;

namespace Azul
{
    public abstract class AbstractDisplay
    {
        public abstract void Put(Tile tile);
        public abstract List<Tile> TakeAll(TileColor tileColor);
    }
}
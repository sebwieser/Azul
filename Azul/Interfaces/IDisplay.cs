using System.Collections.Generic;

namespace Azul
{
    public interface IDisplay
    {
        IEnumerable<Tile> Tiles { get; }
        bool IsEmpty { get; }
    }
}
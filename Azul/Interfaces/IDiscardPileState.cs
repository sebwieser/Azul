using System.Collections;
using System.Collections.Generic;

namespace Azul
{
    public interface IDiscardPileState
    {
        IEnumerable<Tile> Tiles { get; }
    }
}
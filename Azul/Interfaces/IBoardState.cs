using System.Collections.Generic;

namespace Azul
{
    public interface IBoardState
    {
        IEnumerable<Tile> FloorLine { get; }
        //Left hand side
        IDictionary<BoardRow, List<Tile>> PatternLines { get; }
        //Right hand side
        public Wall Wall { get; }
    }
}
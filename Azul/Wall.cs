using System.Collections.Generic;

namespace Azul
{
    public class Wall
    {
        public IDictionary<BoardRow, List<Tile>> Rows { get { return _rows; } }

        private Dictionary<BoardRow, List<Tile>> _rows;

        public WallSide Side { get; }

        public Wall(WallSide side)
        {
            Side = side;
            _rows = new Dictionary<BoardRow, List<Tile>>()
            {
                { BoardRow.One, new List<Tile>(5) },
                { BoardRow.Two, new List<Tile>(5) },
                { BoardRow.Three, new List<Tile>(5) },
                { BoardRow.Four, new List<Tile>(5) },
                { BoardRow.Five, new List<Tile>(5) },
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul
{
    public class Board: IBoardState
    {
        private static int MAX_FLOORLINE_TILES = 7;

        //Negative points row
        public IEnumerable<Tile> FloorLine { get { return _floorLine.AsReadOnly(); } }
        //Left hand side
        public IDictionary<BoardRow, List<Tile>> PatternLines { get { return _patternLines; } }
        //Right hand side
        public Wall Wall { get; }

        private List<Tile> _floorLine;
        private Dictionary<BoardRow, List<Tile>> _patternLines;

        public Board(WallSide side)
        {
            _floorLine = new List<Tile>(MAX_FLOORLINE_TILES);
            Wall = new Wall(side);
            _patternLines = new Dictionary<BoardRow, List<Tile>>(5)
            {
                { BoardRow.One, new List<Tile>(1) },
                { BoardRow.Two, new List<Tile>(2) },
                { BoardRow.Three, new List<Tile>(3) },
                { BoardRow.Four, new List<Tile>(4) },
                { BoardRow.Five, new List<Tile>(5) },
            };
        }
        public List<Tile> PlaceOnFloorLine(List<Tile> tiles)
        {
            int remainingCapacity = MAX_FLOORLINE_TILES - _floorLine.Count;

            if (remainingCapacity < 1 || tiles.Count == 0)
            {
                return tiles;
            }
            
            var fittingTiles = tiles.GetRange(0, remainingCapacity - 1);
            _floorLine.AddRange(fittingTiles);

            return tiles.Except(fittingTiles).ToList();
        }
        public void PlaceStartingPlayerTile(Tile tile)
        {
            if(_floorLine.Count < MAX_FLOORLINE_TILES)
            {
                _floorLine.Add(tile);
            }
        }
        public List<Tile> PlaceOnPatternLine(BoardRow boardRow, List<Tile> tiles)
        {
            if (_patternLines[boardRow].Exists(t => t.TileColor != tiles.First().TileColor))
            {
                throw new AzulGameplayException(string.Format("Cannot place tiles on {0} as it already contains tile(s) of another color.", boardRow));
            }
            
            List<Tile> fittingTiles = tiles.GetRange(0, Math.Min(tiles.Count - 1, (int)boardRow - _patternLines[boardRow].Count - 1));
            _patternLines[boardRow].AddRange(fittingTiles);

            var remainingTiles = tiles.Except(fittingTiles).ToList();

            return PlaceOnFloorLine(remainingTiles);
        }
    }
}
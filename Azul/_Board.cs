using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul {

  public class _Board {
    private static int MAX_FLOORLINE_TILES = 7;

    //Negative points row
    public IReadOnlyCollection<Tile> FloorLine => floorLine.AsReadOnly();

    //Left hand side
    public IReadOnlyDictionary<BoardRow, List<Tile>> PatternLines => patternLines;

    //Right hand side
    public Wall Wall { get; }

    private List<Tile> floorLine;
    private Dictionary<BoardRow, List<Tile>> patternLines;

    public _Board(WallSide side) {

      floorLine = new List<Tile>(MAX_FLOORLINE_TILES);
      Wall = new WallColored();
      patternLines = new Dictionary<BoardRow, List<Tile>>(5)
      {
                { BoardRow.One, new List<Tile>(1) },
                { BoardRow.Two, new List<Tile>(2) },
                { BoardRow.Three, new List<Tile>(3) },
                { BoardRow.Four, new List<Tile>(4) },
                { BoardRow.Five, new List<Tile>(5) },
            };
    }

    public List<Tile> PlaceOnFloorLine(List<Tile> tiles) {
      int remainingCapacity = MAX_FLOORLINE_TILES - floorLine.Count;

      if(remainingCapacity < 1 || tiles.Count == 0) {
        return tiles;
      }

      var fittingTiles = tiles.GetRange(0, Math.Min(tiles.Count, remainingCapacity - 1));
      floorLine.AddRange(fittingTiles);

      return tiles.Except(fittingTiles).ToList();
    }

    public void PlaceStartingPlayerTile(Tile tile) {
      if(floorLine.Count < MAX_FLOORLINE_TILES) {
        floorLine.Add(tile);
      }
    }

    public List<Tile> PlaceOnPatternLine(BoardRow boardRow, List<Tile> tiles) {
      var remainingCapacity = (int)boardRow - patternLines[boardRow].Count;
      var fittingTiles = new List<Tile>();

      if(remainingCapacity > 0) {
        fittingTiles.AddRange(tiles.GetRange(0, Math.Min(tiles.Count, remainingCapacity)));
        patternLines[boardRow].AddRange(fittingTiles);
      }

      var remainingTiles = new List<Tile>(tiles);
      if (fittingTiles.Count > 0) {
        remainingTiles.RemoveRange(0, fittingTiles.Count);
      }
      return remainingTiles;
    }

    public bool PatternLineFull(BoardRow row) {
      return PatternLines[row].Count == (int)row;
    }

    public bool AllPatternLinesProcessed() {
      return patternLines.All(x => x.Value.Count() < (int)x.Key);
    }
  }
}
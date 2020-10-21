using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul {

  public class Board {
    private static int MAX_FLOORLINE_TILES = 7;

    //Negative points row
    public IReadOnlyCollection<Tile> FloorLine => floorLine.AsReadOnly();

    //Left hand side
    public IReadOnlyDictionary<BoardRow, List<Tile>> PatternLines => patternLines;

    //Right hand side
    public Wall Wall { get; }

    private List<Tile> floorLine;
    private Dictionary<BoardRow, List<Tile>> patternLines;

    public Board(WallSide side) {
      floorLine = new List<Tile>(MAX_FLOORLINE_TILES);
      Wall = new Wall(side);
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

      var fittingTiles = tiles.GetRange(0, remainingCapacity - 1);
      floorLine.AddRange(fittingTiles);

      return tiles.Except(fittingTiles).ToList();
    }

    public void PlaceStartingPlayerTile(Tile tile) {
      if(floorLine.Count < MAX_FLOORLINE_TILES) {
        floorLine.Add(tile);
      }
    }

    public List<Tile> PlaceOnPatternLine(BoardRow boardRow, List<Tile> tiles) {
      if(patternLines[boardRow].Exists(t => t.TileColor != tiles.First().TileColor)) {
        throw new AzulGameplayException(string.Format("Cannot place tiles on {0} as it already contains tile(s) of another color.", boardRow));
      }

      var full = patternLines[boardRow].Count == (int)boardRow;
      if(!full) {
        var x = Math.Min(tiles.Count, (int)boardRow - patternLines[boardRow].Count - 1);
        List<Tile> fittingTiles = tiles.GetRange(0, x);
        patternLines[boardRow].AddRange(fittingTiles);

        var remainingTiles = tiles.Except(fittingTiles).ToList();

        return PlaceOnFloorLine(remainingTiles);
      }
      return PlaceOnFloorLine(tiles);
    }
  }
}
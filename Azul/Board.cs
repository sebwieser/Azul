using Azul.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Azul {
  public class Board {

    public Wall Wall { get; private set; }
    public Dictionary<PatternLineRow, PatternLine> PatternLines { get; private set; }
    public FloorLine FloorLine { get; private set; }
    public int Score {
      get {
        return Math.Max(0, scoreToRound + RoundScore);
      }
    }
    public int RoundScore { 
      get { 
        return Wall.RoundScore + FloorLine.RoundScore; 
      } 
    }

    private int scoreToRound;

    public Board(WallSide wallSide) {
      FloorLine = new FloorLine();
      Wall = wallSide.Equals(WallSide.Colored) ? (Wall)new WallColored() : new WallBlank();
      foreach (PatternLineRow capacity in Enum.GetValues(typeof(PatternLineRow))) {
        PatternLines.Add(capacity, new PatternLine(capacity));
      }
      scoreToRound = 0;
    }

    public List<Tile> PlaceOnFloorLine(List<Tile> tiles) {
      var excessTiles = new List<Tile>();
      int numExcessTiles = tiles.Count - FloorLine.EmptySlots;

      if(numExcessTiles > 0) {
        excessTiles = tiles.GetRange(0, numExcessTiles);
        tiles.RemoveRange(0, numExcessTiles);
      }

      FloorLine.PlaceTiles(tiles);

      return excessTiles;
    }
    public List<Tile> PlaceOnPatternLine(List<Tile> tiles, PatternLineRow capacity) {
      var patternLine = PatternLines[capacity];
      var excessTiles = new List<Tile>();
      int numExcessTiles = tiles.Count - patternLine.EmptySlots;

      if(numExcessTiles > 0) {
        excessTiles = tiles.GetRange(0, numExcessTiles);
        tiles.RemoveRange(0, numExcessTiles);
      }

      patternLine.PlaceTiles(tiles);

      return PlaceOnFloorLine(excessTiles);
    }
    public List<Tile> ScoreRow(WallPosition wallPosition) {
      var patternLine = PatternLines[(PatternLineRow)(wallPosition.Row + 1)];
      var tile = patternLine.TakeTileForScoring();
      Wall.PlaceAndScoreTile(tile, wallPosition);

      return patternLine.TakeRemainingTiles();
    }
    public List<Tile> PerformRoundCleanup() {
      UpdateTotalScore();
      Wall.ResetRoundScore();
      return FloorLine.TakeTiles();
    }

    private void UpdateTotalScore() {
      scoreToRound += RoundScore;
    }
  }
}

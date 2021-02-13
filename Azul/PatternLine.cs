using Azul.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Azul {
  public class PatternLine {
    public int Capacity { get; }
    public TileColor Color { get; private set; }
    public int EmptySlots { get { return Capacity - tiles.Count; } }
    public bool IsComplete { get { return EmptySlots == 0; } }
    public int TileCount { get { return tiles.Count; } }

    private List<Tile> tiles;

    public PatternLine(Enums.PatternLineRow capacity) {
      Capacity = (int)capacity;
      Color = TileColor.None;
      tiles = new List<Tile>(Capacity);
    }

    public List<Tile> TakeRemainingTiles() {
      if(IsComplete) {
        throw new AzulGameplayException("Cannot take remaining tiles, first you need to take one tile for wall placement and scoring.");
      }
      var takenTiles = new List<Tile>(tiles);
      tiles.Clear();
      ResetColor();
      return takenTiles;
    }
    public Tile TakeTileForScoring() {
      if(!IsComplete) {
        throw new AzulGameplayException("Pattern line is not complete, cannot take tile for scoring.");
      }

      var takenTile = tiles.First();
      tiles.Remove(takenTile);
      ResetColor();
      return takenTile;
    }
    public void PlaceTiles(List<Tile> pendingTiles)  {
      if(!AtLeastOneTileGettingPlaced(pendingTiles)) {
        return;
      }
      
      if(!AllPlacementChecksPassing(pendingTiles)) {
        throw new AzulPlacementException("Make sure to have enough space on the pattern line and that all tiles are of the same color as the ones already contained.");
      }
      tiles.AddRange(pendingTiles);
      Color = pendingTiles.First().TileColor;
    }

    private bool AllPlacementChecksPassing(List<Tile> tiles) {
      return 
        HasEnoughSpace(tiles) 
        && AllTilesOfSameColor(tiles) 
        && ExistingColorNotViolated(tiles)
        && FirstPlayerTileNotAmongPendingTiles(tiles);
    }
    private bool AtLeastOneTileGettingPlaced(List<Tile> tiles) {
      return tiles != null && tiles.Count() > 0;
    }
    private bool FirstPlayerTileNotAmongPendingTiles(List<Tile> tiles) {
      return !tiles.Exists(t => t.TileColor.Equals(TileColor.FirstPlayer));
    }
    private bool HasEnoughSpace(List<Tile> tiles) {
      return !IsComplete && tiles.Count() <= EmptySlots;
    }
    private bool AllTilesOfSameColor(List<Tile> tiles) {
      return tiles.Distinct().Count() < 2;
    }
    private bool ExistingColorNotViolated(List<Tile> tiles) {
      return Color == TileColor.None || tiles.All(t => t.TileColor.Equals(Color));
    }
    private void ResetColor() {
      if(tiles.Count() == 0) {
        Color = TileColor.None;
      }
    }
  }
}
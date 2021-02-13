using System.Collections.Generic;
using System.Linq;

namespace Azul {

  public class FactoryDisplay : IDisplay {
    private static short MAX_TILES = 4;

    public DisplayType Type => DisplayType.FactoryDisplay;
    public IReadOnlyCollection<Tile> Tiles => tiles.AsReadOnly();
    public bool IsEmpty => tiles.Count == 0;

    private List<Tile> tiles;
    private Centre centre;

    public FactoryDisplay(Centre centre) {
      this.centre = centre;
      tiles = new List<Tile>(MAX_TILES);
    }

    public List<Tile> TakeAll(TileColor tileColor) {
      if(IsEmpty || GetColorCount(tileColor) == 0) {
        throw new AzulGameplayException("Display is empty or doesn't contain the required tile color.");
      }
      if(tileColor.Equals(TileColor.FirstPlayer)) {
        throw new AzulGameplayException("You've chosen a first player tile, this is not allowed. Please pick another color.");
      }

      var chosenTiles = tiles.FindAll(t => t.TileColor.Equals(tileColor));
      var remainingTiles = tiles.Except(chosenTiles).ToList();

      //Move remaining tiles to the middle
      if(remainingTiles.Count > 0) {
        centre.Put(remainingTiles);
        tiles.Clear();
      }

      return chosenTiles;
    }

    public void Put(Tile tile) {
      if(tiles.Count == MAX_TILES) {
        throw new AzulSetupException("Cannot add tile, factory display full");
      }

      tiles.Add(tile);
    }

    public int GetColorCount(TileColor tileColor) {
      return tiles.Count(t => t.TileColor.Equals(tileColor));
    }
  }
}
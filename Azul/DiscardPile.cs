using System.Collections.Generic;

namespace Azul {

  public class DiscardPile {
    public IReadOnlyCollection<Tile> Tiles => tiles.AsReadOnly();

    private List<Tile> tiles;

    public DiscardPile() {
      tiles = new List<Tile>();
    }

    public void Put(List<Tile> discardedTiles) {
      tiles.AddRange(discardedTiles);
    }

    public List<Tile> TakeAll() {
      var allTiles = new List<Tile>(tiles);
      tiles.Clear();
      return allTiles;
    }
  }
}
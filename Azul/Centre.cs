using System.Collections.Generic;
using System.Linq;

namespace Azul {

  public class Centre : IDisplay {
    public DisplayType Type => DisplayType.Centre;
    public IReadOnlyCollection<Tile> Tiles => tiles.AsReadOnly();
    public bool IsEmpty => tiles.Count == 0;

    private List<Tile> tiles;

    public Centre() {
      tiles = new List<Tile>();
    }

    public void Put(List<Tile> tiles) {
      this.tiles.AddRange(tiles);
    }

    public void Put(Tile tile) {
      tiles.Add(tile);
    }

    public List<Tile> TakeAll(TileColor tileColor) {
      var takenTiles = tiles.FindAll(t => t.TileColor.Equals(tileColor) || t.TileColor.Equals(TileColor.FirstPlayer));
      tiles = tiles.Except(takenTiles).ToList();

      return takenTiles;
    }

    public int GetColorCount(TileColor tileColor) {
      return tiles.Count(t => t.TileColor.Equals(tileColor));
    }
  }
}
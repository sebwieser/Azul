using System.Collections.Generic;

namespace Azul {

  public interface IDisplay {
    DisplayType Type { get; }
    IReadOnlyCollection<Tile> Tiles { get; }
    bool IsEmpty { get; }

    void Put(Tile tile);
    List<Tile> TakeAll(TileColor tileColor);

    int GetColorCount(TileColor tileColor);
  }
}
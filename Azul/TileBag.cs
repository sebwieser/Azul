using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul {

  public class TileBag {
    private static int MAX_TILES = 100;

    public int RemainingTiles => tiles.Count;
    public IReadOnlyCollection<Tile> Tiles => tiles.AsReadOnly();

    private List<Tile> tiles;
    private Random rng;

    public TileBag() {
      tiles = new List<Tile>(MAX_TILES);
      rng = new Random();

      //All tile types except for the first player token go into bag
      var tileTypes = Enum.GetValues(typeof(TileColor))
          .Cast<TileColor>()
          .Where(tc => tc != TileColor.FirstPlayer)
          .ToList();

      //Put 20 of tile pieces of each color into bag
      foreach(var tileColor in tileTypes) {
        for(int i = 0; i < 20; i++) {
          tiles.Add(new Tile(tileColor));
        }
      }
    }

    public Tile Fetch() {
      if(RemainingTiles == 0) {
        throw new AzulGameplayException("Cannot fetch tile from an empty bag.");
      }

      var tile = tiles[rng.Next(0, tiles.Count - 1)];
      tiles.Remove(tile);
      return tile;
    }

    public void Put(List<Tile> newTiles) {
      if((newTiles.Count + RemainingTiles) > 100) {
        throw new AzulGameplayException("Cannot put tiles into the bag as it would exceed the maximum bag size.");
      }

      tiles.AddRange(newTiles);
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Azul {
  public class FloorLine {

    public int TileCount { get { return tiles.Count; } }
    public int EmptySlots { get { return MAX_CAPACITY - tiles.Count; } }
    public int RoundScore { get { return CalculateScore(); } }
    public IReadOnlyList<Tile> Tiles { get { return tiles.AsReadOnly(); } }

    private static int MAX_CAPACITY = 7;
    
    private List<Tile> tiles;

    public FloorLine() {
      tiles = new List<Tile>(MAX_CAPACITY);
    }

    public void PlaceTiles(List<Tile> tilesToPlace) {
      if(EmptySlots < tilesToPlace.Count) {
        throw new AzulPlacementException(string.Format("Attempted to place {0} tiles on the FloorLine, while there's only a capacity for {1} more.", tilesToPlace.Count, EmptySlots));
      }

      tiles.AddRange(tilesToPlace);
    }

    public List<Tile> TakeTiles() {
      var takenTiles = new List<Tile>(tiles);
      tiles.Clear();
      return takenTiles;
    }

    private int CalculateScore() {
      switch(tiles.Count) {
        case 0:
          return 0;
        case 1:
          return -1;
        case 2:
          return -2;
        case 3:
          return -4;
        case 4:
          return -6;
        case 5:
          return -8;
        case 6:
          return -11;
        case 7:
          return -14;
        default:
          throw new AzulGameplayException(string.Format("Illegal number of tiles on the FloorLine({0}", tiles.Count));
      }      
    } 
  }
}

using System.Collections.Generic;

namespace Azul {

  public class Wall {

    public WallSide Side { get; }

    public Tile[,] Slots { get; }

    public TileColor[,] SlotConfiguration { get; }
    public int Score { get; private set; }

    public Wall(WallSide side) {
      Side = side;
      Slots = new Tile[5, 5];
      SlotConfiguration = new TileColor[5, 5] {
        {TileColor.Blue,TileColor.Yellow,TileColor.Red,TileColor.Black,TileColor.Turquoise },
        {TileColor.Turquoise,TileColor.Blue,TileColor.Yellow,TileColor.Red,TileColor.Black },
        {TileColor.Black,TileColor.Turquoise,TileColor.Blue,TileColor.Yellow,TileColor.Red },
        {TileColor.Red,TileColor.Black,TileColor.Turquoise,TileColor.Blue,TileColor.Yellow },
        {TileColor.Yellow,TileColor.Red,TileColor.Black,TileColor.Turquoise,TileColor.Blue }
      };
    }

    public bool Place(Tile tile, int x, int y) {
      if(PositionWithinBoundaries(x, y) && Slots[x, y] == null && SlotConfiguration[x, y].Equals(tile.TileColor)) {
        PutAndScore(tile, x, y);
        return true;
      }
      return false;
    }
    private void PutAndScore(Tile tile, int x, int y) {
      Slots[x, y] = tile;

      int tempX = 0;
      int tempY = 0;

      bool containsX = false;
      bool doneX = false;
      bool containsY = false;
      bool doneY = false;

      for(int h = 0, v = 0; h < 5 && v < 5; h++, v++) {
        if(Slots[h, y] != null && !doneX) {
          tempX++;
          if(h == x) {
            containsX = true;
          }
        }
        else if(!containsX) {
          tempX = 0;
        }
        else {
          doneX = true;
        }

        if(Slots[x, v] != null && !doneY) {
          tempY++;
          if(v == y) {
            containsY = true;
          }
        }
        else if(!containsY) {
          tempY = 0;
        }
        else {
          doneY = true;
        }

        if(doneX && doneY) {
          break;
        }
      }

      Score += tempX > 1 && tempY > 1 ? (tempX + tempY) : tempX > 1 ? tempX : tempY;
    }
    private bool PositionWithinBoundaries(int x, int y) {
      if(x < 0 || x > 4 || y < 0 || y > 4) {
        return false;
      }
      return true;
    }

    public bool RowFull(BoardRow row) {
      for(int i = 0; i < 5; i++) {
        if(Slots[(int)row - 1, i] == null) {
          return false;
        }
      }
      return true;
    }
    public int IndexOf(BoardRow row, TileColor tileColor) {
      for(int i = 0; i < 5; i++) {
        if(SlotConfiguration[(int)row - 1, i].Equals(tileColor)) {
          return i;
        }
      }
      throw new AzulGameplayException("No such color on the board row.");
    }

    //TODO: remove, this is just a testing helper:
    public List<TileColor> GetFreeTileColorsInRow(BoardRow row) {
      var l = new List<TileColor>();
      for(int i = 0; i < 5; i++) {
        if(Slots[(int)row - 1, i] == null)
          l.Add(SlotConfiguration[(int)row - 1, i]);
      }
      return l;
    }
  }
}
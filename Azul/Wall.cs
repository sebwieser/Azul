using System.Collections.Generic;

namespace Azul {

  public struct WallPosition {
    public int Row { get; }
    public int Column { get; }

    public WallPosition(int row, int column) {
      Row = row;
      Column = column;
    }

    public bool InBounds() {
      return DimensionInBounds(Row) && DimensionInBounds(Column);
    }
    public static bool DimensionInBounds(int dimension) {
      return dimension >= 0 && dimension < 5;
    }
    public override string ToString() {
      return string.Format("Wall position: Row={0}, Column={1}", Row, Column);
    }
  }

  public abstract class Wall {
    public int RoundScore { get; private set; }
    public Tile[][] Slots { get; private set; }
    public abstract WallSide Side { get; protected set; }

    protected Wall() {
      Slots = new Tile[5][];
      Slots[0] = new Tile[5] { null, null, null, null, null };
      Slots[1] = new Tile[5] { null, null, null, null, null };
      Slots[2] = new Tile[5] { null, null, null, null, null };
      Slots[3] = new Tile[5] { null, null, null, null, null };
      Slots[4] = new Tile[5] { null, null, null, null, null };
    }

    public void PlaceAndScoreTile(Tile tile, WallPosition position) {
      if(!position.InBounds()) {
        throw new AzulPlacementException(string.Format("{0} is out of bounds", position));
      }
      PlaceTile(tile, position);
      ScoreTile(position);
    }
    public IEnumerable<WallPosition> GetValidRowPositionsForTile(Tile tile, int row) {
      if (!WallPosition.DimensionInBounds(row)) {
        throw new AzulPlacementException(string.Format("Row={0} is out of bounds", row));
      }
      for (int column = 0; column < 5; column++) {
        WallPosition position = new WallPosition(row, column);
        if(IsValidPosition(tile.TileColor, position)) {
          yield return position;
        }
      }
    }

    private void PlaceTile(Tile tile, WallPosition position) {
      if(!IsValidPosition(tile.TileColor, position)) {
        throw new AzulPlacementException(string.Format("Cannot place {0} onto {1}", tile, position));
      }
      Slots[position.Row][position.Column] = tile;
    }
    private void ScoreTile(WallPosition position) {
      int verticalScore = 0;
      int horizontalScore = 0;
      bool containsPositionRow = false;
      bool columnDone = false;
      bool containsPositionColumn = false;
      bool rowDone = false;

      for(int i = 0; i < 5; i++) {
        #region Vertical Scoring
        //if slot contains a tile and we didn't yet break the streak after passing through the position
        if(Slots[i][position.Column] != null && !columnDone) {
          verticalScore++;
          if(i == position.Row) {
            containsPositionRow = true;
          }
        }
        //if we broke the streak but haven't yet reached our newly placed tile, reset temporary score for this direction
        else if(!containsPositionRow) {
          verticalScore = 0;
        }
        //getting here means we broke the streak and have already passed through newly placed tile, that's it: scoring for this direction is finished
        else {
          columnDone = true;
        }
        #endregion
        #region Horizontal Scoring
        if(Slots[position.Row][i] != null && !rowDone) {
          horizontalScore++;
          if(i == position.Column) {
            containsPositionColumn = true;
          }
        }
        else if(!containsPositionColumn) {
          horizontalScore = 0;
        }
        else {
          rowDone = true;
        }

        if(columnDone && rowDone) {
          break;
        }
        #endregion
      }

      RoundScore += (verticalScore > 1 && horizontalScore > 1) ? 
          (verticalScore + horizontalScore) : verticalScore > 1 ? 
            verticalScore : horizontalScore;
    }
    protected bool SlotEmpty(WallPosition position) {
      return Slots[position.Row][position.Column] is null;
    }

    public void ResetRoundScore() {
      RoundScore = 0;
    }

    protected abstract bool IsValidPosition(TileColor tileColor, WallPosition position); 

  }
}

namespace Azul {
  public class WallBlank : Wall {
    public override WallSide Side { get; protected set; }

    public WallBlank() : base() {
      Side = WallSide.Blank;
    }

    protected override bool IsValidPosition(TileColor tileColor, WallPosition position) {
      return SlotEmpty(position) && ColorValidForPosition(tileColor, position);
    }
    private bool ColorValidForPosition(TileColor tileColor, WallPosition position) {
      for(int i = 0; i < 5; i++) {
        if(Slots[i][position.Column] != null) {
          if(Slots[i][position.Column].TileColor.Equals(tileColor)) {
            return false;
          }
        }
        if(Slots[position.Row][i] != null) {
          if(Slots[position.Row][i].TileColor.Equals(tileColor)) {
            return false;
          }
        }
      }
      return true;
    }
  }
}


namespace Azul {
  public class WallColored : Wall {
    public override WallSide Side { get; protected set; }

    private static TileColor[][] configuration = new TileColor[5][] {
        new TileColor[] {TileColor.Blue,TileColor.Yellow,TileColor.Red,TileColor.Black,TileColor.Turquoise },
        new TileColor[] {TileColor.Turquoise,TileColor.Blue,TileColor.Yellow,TileColor.Red,TileColor.Black },
        new TileColor[] {TileColor.Black,TileColor.Turquoise,TileColor.Blue,TileColor.Yellow,TileColor.Red },
        new TileColor[] {TileColor.Red,TileColor.Black,TileColor.Turquoise,TileColor.Blue,TileColor.Yellow },
        new TileColor[] {TileColor.Yellow,TileColor.Red,TileColor.Black,TileColor.Turquoise,TileColor.Blue }
      };

    public WallColored() : base() {
      Side = WallSide.Colored;
    }
    protected override bool IsValidPosition(TileColor tileColor, WallPosition position) {
      return SlotEmpty(position) && configuration[position.Row][position.Column].Equals(tileColor);
    }
  }
}

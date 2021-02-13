
using Azul.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Azul.UnitTests {
  public class FloorLineTests {

    public static IEnumerable<object[]> TestData {
      get {
        return new[] {
           new object[] { new List<Tile>(), 0 },
           new object[] { new List<Tile>() {
             new Tile(TileColor.Black)
           }, -1 },
           new object[] { new List<Tile>() {
             new Tile(TileColor.Black),
             new Tile(TileColor.Blue)
           }, -2 },
           new object[] { new List<Tile>() {
             new Tile(TileColor.Black),
             new Tile(TileColor.Blue),
             new Tile(TileColor.Yellow)
           }, -4 },
           new object[] { new List<Tile>() {
             new Tile(TileColor.Black),
             new Tile(TileColor.Blue),
             new Tile(TileColor.Yellow),
             new Tile(TileColor.Red)
           }, -6 },
           new object[] { new List<Tile>() {
             new Tile(TileColor.Black),
             new Tile(TileColor.Blue),
             new Tile(TileColor.Yellow),
             new Tile(TileColor.Red),
             new Tile(TileColor.Turquoise)
           }, -8 },
           new object[] { new List<Tile>() {
             new Tile(TileColor.Black),
             new Tile(TileColor.Blue),
             new Tile(TileColor.Yellow),
             new Tile(TileColor.Red),
             new Tile(TileColor.Turquoise),
             new Tile(TileColor.FirstPlayer)
           }, -11 },
           new object[] { new List<Tile>() {
             new Tile(TileColor.Black),
             new Tile(TileColor.Blue),
             new Tile(TileColor.Yellow),
             new Tile(TileColor.Red),
             new Tile(TileColor.Turquoise),
             new Tile(TileColor.FirstPlayer),
             new Tile(TileColor.Black)
           }, -14 },
        };
      }
    }

    [Fact]
    public void FloorLine_PlaceTiles_Happy() {
      var floorLine = new FloorLine();

      var tiles = new List<Tile>() {
        new Tile(TileColor.FirstPlayer),
        new Tile(TileColor.Black),
        new Tile(TileColor.Black),
        new Tile(TileColor.Turquoise),
        new Tile(TileColor.Yellow),
        new Tile(TileColor.Blue),
        new Tile(TileColor.Red)
      };

      floorLine.PlaceTiles(tiles);

      Assert.True(floorLine.EmptySlots == 0 && floorLine.TileCount == 7 && floorLine.Tiles.Count == 7);
    }
    [Fact]
    public void FloorLine_PlaceTiles_Overflow() {
      var floorLine = new FloorLine();

      var tiles = new List<Tile>() {
        new Tile(TileColor.FirstPlayer),
        new Tile(TileColor.Black),
        new Tile(TileColor.Black),
        new Tile(TileColor.Turquoise),
        new Tile(TileColor.Yellow),
        new Tile(TileColor.Blue),
        new Tile(TileColor.Red),

        new Tile(TileColor.Black) //one extra
      };

      Action placeTiles = () => floorLine.PlaceTiles(tiles);

      Assert.Throws<AzulPlacementException>(placeTiles);
    }
    [Fact]
    public void FloorLine_PlaceTiles_ZeroTilesShouldPass() {
      var floorLine = new FloorLine();

      var tiles = new List<Tile>() {
      };

      floorLine.PlaceTiles(tiles);

      Assert.True(floorLine.EmptySlots == 7 && floorLine.TileCount == 0 && floorLine.Tiles.Count == 0);
    }
    [Theory, MemberData(nameof(TestData))]
    public void FloorLine_PlaceTiles_TestScore(List<Tile> tiles, int expectedScore) {
      var floorLine = new FloorLine();

      floorLine.PlaceTiles(tiles);

      Assert.True(floorLine.RoundScore == expectedScore);
    }
    [Fact]
    public void FloorLine_PlaceTiles_NullRefExceptionWhenListNull() {
      var floorLine = new FloorLine();

      Action placeTiles = () => floorLine.PlaceTiles(null);

      Assert.Throws<NullReferenceException>(placeTiles);
    }
    [Theory, MemberData(nameof(TestData))]
    public void FloorLine_TakeRemainingTiles_Happy(List<Tile> tiles, object _ignored) {
      //not needed in this test, but more practical this way than copying all those list values into another static object
      object expectedScore = _ignored;
      var floorLine = new FloorLine();

      int count = tiles.Count;
      floorLine.PlaceTiles(tiles);

      var remainingTiles = floorLine.TakeTiles();
      var firstNotSecond = remainingTiles.Except(tiles).ToList();
      var secondNotFirst = tiles.Except(remainingTiles).ToList();


      Assert.True(!firstNotSecond.Any() && !secondNotFirst.Any() && floorLine.TileCount == 0 && remainingTiles.Count == count);
    }


  }


}

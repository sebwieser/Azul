using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Azul.UnitTests {
  public class WallTests {
    public static IEnumerable<object[]> TestData {
      get {
        return new[] {
           new object[] { Enums.PatternLineRow.One },
           new object[] { Enums.PatternLineRow.Two },
           new object[] { Enums.PatternLineRow.Three },
           new object[] { Enums.PatternLineRow.Four },
           new object[] { Enums.PatternLineRow.Five },
        };
      }
    }
    [Fact]
    public void BlankWall_GetValidRowPositionsForTile_Simple() {
      Wall wall = new WallBlank();
      Tile tile = new Tile(TileColor.Black);

      List<WallPosition> validPositions = new List<WallPosition>();
      for(int i = 0; i < 5; i++) {
        validPositions.Add(new WallPosition(0, i));
      }

      Assert.Equal(validPositions, wall.GetValidRowPositionsForTile(tile, 0));
    }
    [Fact]
    public void BlankWall_GetValidRowPositionsForTile_NoSpaceForMultipleSameColors() {
      Wall wall = new WallBlank();
      Tile tile = new Tile(TileColor.Black);
      var validPositions = wall.GetValidRowPositionsForTile(tile, 0);

      wall.PlaceAndScoreTile(tile, validPositions.First());

      Assert.Empty(wall.GetValidRowPositionsForTile(tile, 0));
    }
    [Fact]
    public void BlankWall_PlaceAndScoreTile_Simple() {
      Wall wall = new WallBlank();
      Tile tile = new Tile(TileColor.Black);
      var validPositions = wall.GetValidRowPositionsForTile(tile, 0);

      wall.PlaceAndScoreTile(tile, validPositions.First());

      Assert.Equal(1, wall.RoundScore);
    }
    [Fact]
    public void BlankWall_PlaceAndScoreTile_ChainedHorozintalScoringCorrect() {
      Wall wall = new WallBlank();
      int expectedScore = 1 + 2 + 3 + 4 + 5;
      var tileColors = Enum.GetValues(typeof(TileColor)).Cast<TileColor>().Except(new TileColor[] { TileColor.FirstPlayer, TileColor.None });

      foreach(var color in tileColors) {
        var tile = new Tile(color);
        var validPositions = wall.GetValidRowPositionsForTile(tile, 0);
        wall.PlaceAndScoreTile(tile, validPositions.First());
      }

      Assert.Equal(expectedScore, wall.RoundScore);
    }
    [Fact]
    public void BlankWall_PlaceAndScoreTile_ChainedHorozintalScoringCorrectWithSkipping() {
      Wall wall = new WallBlank();
      int expectedScore = 1 + 2 + 1 + 2 + 5;

      wall.PlaceAndScoreTile(new Tile(TileColor.Blue), new WallPosition(0, 0));
      wall.PlaceAndScoreTile(new Tile(TileColor.Yellow), new WallPosition(0, 1));
      //intentionally skip one slot to lose streak:
      wall.PlaceAndScoreTile(new Tile(TileColor.Red), new WallPosition(0, 3));
      wall.PlaceAndScoreTile(new Tile(TileColor.Black), new WallPosition(0, 4));
      //fill skipped slot later for max points:
      wall.PlaceAndScoreTile(new Tile(TileColor.Turquoise), new WallPosition(0, 2));

      Assert.Equal(expectedScore, wall.RoundScore);
    }
 

  }
}

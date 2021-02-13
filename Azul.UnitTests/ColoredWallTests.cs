using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Azul.UnitTests {
  public class ColoredWallTests {

    [Fact]
    public void ColoredWall_GetValidRowPositionsForTile_Simple() {
      Wall wall = new WallColored();
      Tile tile = new Tile(TileColor.Black);
      WallPosition wallPosition = new WallPosition(0, 3);
      List<WallPosition> expectedPositions = new List<WallPosition>() { wallPosition };

      var gotPositions = wall.GetValidRowPositionsForTile(tile, 0);

      Assert.Equal(expectedPositions, gotPositions);
    }
    [Fact]
    public void ColoredWall_GetValidRowPositionsForTile_NoSpaceForMultipleSameColors() {
      Wall wall = new WallColored();
      Tile tile = new Tile(TileColor.Black);
      var validPositions = wall.GetValidRowPositionsForTile(tile, 0);

      wall.PlaceAndScoreTile(tile, validPositions.First());

      Assert.Empty(wall.GetValidRowPositionsForTile(tile, 0));
    }
    [Fact]
    public void ColoredWall_PlaceAndScoreTile_Simple() {
      Wall wall = new WallColored();
      Tile tile = new Tile(TileColor.Black);
      var validPositions = wall.GetValidRowPositionsForTile(tile, 0);

      wall.PlaceAndScoreTile(tile, validPositions.First());

      Assert.Equal(1, wall.RoundScore);
    }
    [Fact]
    public void ColoredWall_PlaceAndScoreTile_ChainedHorozintalScoringCorrect() {
      Wall wall = new WallColored();
      int expectedScore = 1 + 2 + 3 + 4 + 5;

      //First row order: TileColor.Blue,TileColor.Yellow,TileColor.Red,TileColor.Black,TileColor.Turquoise
      wall.PlaceAndScoreTile(new Tile(TileColor.Blue), new WallPosition(0, 0));
      wall.PlaceAndScoreTile(new Tile(TileColor.Yellow), new WallPosition(0, 1));
      wall.PlaceAndScoreTile(new Tile(TileColor.Red), new WallPosition(0, 2));
      wall.PlaceAndScoreTile(new Tile(TileColor.Black), new WallPosition(0, 3));
      wall.PlaceAndScoreTile(new Tile(TileColor.Turquoise), new WallPosition(0, 4));

      Assert.Equal(expectedScore, wall.RoundScore);
    }
    [Fact]
    public void ColoredWall_PlaceAndScoreTile_ChainedHorozintalScoringCorrectWithSkipping() {
      Wall wall = new WallColored();
      int expectedScore = 1 + 2 + 1 + 2 + 5;

      //First row order: TileColor.Blue,TileColor.Yellow,TileColor.Red,TileColor.Black,TileColor.Turquoise
      wall.PlaceAndScoreTile(new Tile(TileColor.Blue), new WallPosition(0, 0));
      wall.PlaceAndScoreTile(new Tile(TileColor.Yellow), new WallPosition(0, 1));
      //intentionally skip one slot to lose streak:
      wall.PlaceAndScoreTile(new Tile(TileColor.Black), new WallPosition(0, 3));
      wall.PlaceAndScoreTile(new Tile(TileColor.Turquoise), new WallPosition(0, 4));
      //fill skipped slot later for max points:
      wall.PlaceAndScoreTile(new Tile(TileColor.Red), new WallPosition(0, 2));

      Assert.Equal(expectedScore, wall.RoundScore);
    }
    [Fact]
    public void ColoredWall_PlaceAndScoreTile_FailsForInvalidPosition() {
      Wall wall = new WallColored();
      Tile tile = new Tile(TileColor.Black);
      WallPosition validPosition = wall.GetValidRowPositionsForTile(tile, 0).First();

      wall.PlaceAndScoreTile(tile, validPosition);
      Action placeAtSamePositionAgain = () => wall.PlaceAndScoreTile(tile, validPosition);

      Assert.Throws<AzulPlacementException>(placeAtSamePositionAgain);
    }
    [Fact]
    public void ColoredWall_PlaceAndScoreTile_CannotPlaceOnSpotForAnotherColor() {
      Wall wall = new WallColored();
      Tile tile = new Tile(TileColor.Black);
      WallPosition validPosition = wall.GetValidRowPositionsForTile(tile, 0).First();

      Action placeAtPositionForAnotherColor = () => wall.PlaceAndScoreTile(tile, new WallPosition(validPosition.Row, (validPosition.Column + 1) % 5));

      Assert.Throws<AzulPlacementException>(placeAtPositionForAnotherColor);
    }
    [Fact]
    public void ColoredWall_ResetRoundScore_ResetsToZero() {
      Wall wall = new WallColored();
      int expectedScore = 1 + 2 + 1 + 2 + 5;

      //First row order: TileColor.Blue,TileColor.Yellow,TileColor.Red,TileColor.Black,TileColor.Turquoise
      wall.PlaceAndScoreTile(new Tile(TileColor.Blue), new WallPosition(0, 0));
      wall.PlaceAndScoreTile(new Tile(TileColor.Yellow), new WallPosition(0, 1));
      //intentionally skip one slot to lose streak:
      wall.PlaceAndScoreTile(new Tile(TileColor.Black), new WallPosition(0, 3));
      wall.PlaceAndScoreTile(new Tile(TileColor.Turquoise), new WallPosition(0, 4));
      //fill skipped slot later for max points:
      wall.PlaceAndScoreTile(new Tile(TileColor.Red), new WallPosition(0, 2));

      //make sure score was != 0 before resetting
      int gotScore = wall.RoundScore;
      wall.ResetRoundScore();

      Assert.True(expectedScore == gotScore && wall.RoundScore == 0);
    }
  }
}

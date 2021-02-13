using Azul.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Azul.UnitTests {
  public class PatternLineTests {
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

    [Theory, MemberData(nameof(TestData))]
    public void PatternLine_Capacity_EqualToConstructedValue(Enums.PatternLineRow capacity) {
      PatternLine patternLine = new PatternLine(capacity);

      Assert.Equal((int)capacity, patternLine.Capacity);
    }
    [Theory, MemberData(nameof(TestData))]
    public void PatternLine_EmptySlots_InitiallyEqualToCapacity(Enums.PatternLineRow capacity) {
      PatternLine patternLine = new PatternLine(capacity);

      Assert.Equal((int)capacity, patternLine.EmptySlots);
    }
    [Theory, MemberData(nameof(TestData))]
    public void PatternLine_Color_InitiallyNone(Enums.PatternLineRow capacity) {
      PatternLine patternLine = new PatternLine(capacity);

      Assert.Equal(TileColor.None, patternLine.Color);
    }
    [Theory, MemberData(nameof(TestData))]
    public void PatternLine_PlaceTiles_EmptyListOfTilesShouldPassQuietly(Enums.PatternLineRow capacity) {
      PatternLine patternLine = new PatternLine(capacity);
      List<Tile> tiles = new List<Tile>();
      patternLine.PlaceTiles(tiles);

      Assert.True(patternLine.EmptySlots == (int)capacity && patternLine.Color == TileColor.None && !patternLine.IsComplete && patternLine.TileCount == 0);
    }
    [Theory, MemberData(nameof(TestData))]
    public void PatternLine_PlaceTiles_Happy(Enums.PatternLineRow capacity) {
      PatternLine patternLine = new PatternLine(capacity);
      List<Tile> tiles = Enumerable.Repeat(new Tile(TileColor.Red), (int)capacity).ToList();
      patternLine.PlaceTiles(tiles);

      Assert.True(patternLine.EmptySlots == 0 && patternLine.Color == TileColor.Red && patternLine.TileCount == (int)capacity && patternLine.IsComplete);
    }
    [Theory, MemberData(nameof(TestData))]
    public void PatternLine_PlaceTiles_ExceedingCapacityThrowsException(Enums.PatternLineRow capacity) {
      PatternLine patternLine = new PatternLine(capacity);
      List<Tile> tiles = Enumerable.Repeat(new Tile(TileColor.Red), (int)capacity + 1).ToList();
      Action placeTiles = () => patternLine.PlaceTiles(tiles);

      Assert.Throws<AzulPlacementException>(placeTiles);
    }
    [Theory, MemberData(nameof(TestData))]
    public void PatternLine_PlaceTiles_MultipleColorsDetectedThrowsException(Enums.PatternLineRow capacity) {
      PatternLine patternLine = new PatternLine(capacity);
      List<Tile> tiles = new List<Tile>();
      if((int)capacity > 1) {
        tiles.AddRange(Enumerable.Repeat(new Tile(TileColor.Red), (int)capacity - 1));
      }
      else {
        tiles.Add(new Tile(TileColor.Red));
      }
      tiles.Add(new Tile(TileColor.Black));
      Action placeTiles = () => patternLine.PlaceTiles(tiles);

      Assert.Throws<AzulPlacementException>(placeTiles);
    }
    [Theory, MemberData(nameof(TestData))]
    public void PatternLine_PlaceTiles_FirstPlayerTileDetectedThrowsException(Enums.PatternLineRow capacity) {
      PatternLine patternLine = new PatternLine(capacity);
      List<Tile> tiles = new List<Tile>();
      tiles.Add(new Tile(TileColor.FirstPlayer));
      Action placeTiles = () => patternLine.PlaceTiles(tiles);

      Assert.Throws<AzulPlacementException>(placeTiles);
    }
    [Fact]
    public void PatternLine_PlaceTiles_ColorViolatedThrowsException() {
      Enums.PatternLineRow capacity = Enums.PatternLineRow.Five;
      PatternLine patternLine = new PatternLine(capacity);
      List<Tile> tiles = new List<Tile>();
      tiles.Add(new Tile(TileColor.Red));
      patternLine.PlaceTiles(tiles);

      List<Tile> additionalTiles = new List<Tile>();
      additionalTiles.AddRange(Enumerable.Repeat(new Tile(TileColor.Blue), patternLine.EmptySlots));

      Action placeTiles = () => patternLine.PlaceTiles(additionalTiles);

      Assert.Throws<AzulPlacementException>(placeTiles);
    }
    [Theory, MemberData(nameof(TestData))]
    public void PatternLine_PlaceTiles_TakeAllRemainingClearsEverything(Enums.PatternLineRow capacity) {
      PatternLine patternLine = new PatternLine(capacity);
      List<Tile> tiles = new List<Tile>();
      if((int)capacity > 1) {
        tiles.AddRange(Enumerable.Repeat(new Tile(TileColor.Blue), (int)capacity - 1));
      }
      patternLine.PlaceTiles(tiles);

      List<Tile> remainingTiles = patternLine.TakeRemainingTiles();

      Assert.True(remainingTiles.Count() == (int)capacity-1 && patternLine.Color == TileColor.None && patternLine.EmptySlots == (int)capacity && patternLine.TileCount == 0);
    }
    [Theory, MemberData(nameof(TestData))]
    public void PatternLine_PlaceTiles_TakeAllRemainingFromFullLineThrowsException(Enums.PatternLineRow capacity) {
      PatternLine patternLine = new PatternLine(capacity);
      List<Tile> tiles = new List<Tile>();
      tiles.AddRange(Enumerable.Repeat(new Tile(TileColor.Blue), (int)capacity));
      patternLine.PlaceTiles(tiles);
      Action takeRemainingTiles = () => patternLine.TakeRemainingTiles();

      Assert.Throws<AzulGameplayException>(takeRemainingTiles);
    }
    [Theory, MemberData(nameof(TestData))]
    public void PatternLine_PlaceTiles_TakingTileFromEmptyLineThrowsException(Enums.PatternLineRow capacity) {
      PatternLine patternLine = new PatternLine(capacity);
      Action takeTile = () => patternLine.TakeTileForScoring();

      Assert.Throws<AzulGameplayException>(takeTile);
    }
    [Theory, MemberData(nameof(TestData))]
    public void PatternLine_PlaceTiles_TakingTileFromNonFullLineThrowsException(Enums.PatternLineRow capacity) {
      PatternLine patternLine = new PatternLine(capacity);
      List<Tile> tiles = new List<Tile>();
      if((int)capacity > 1) {
        tiles.AddRange(Enumerable.Repeat(new Tile(TileColor.Blue), (int)capacity - 1));
      }

      Action takeTile = () => patternLine.TakeTileForScoring();

      Assert.Throws<AzulGameplayException>(takeTile);
    }
    [Fact]
    public void PatternLine_PlaceTiles_TakingTileForScoringCapacity1Happy() {
      Enums.PatternLineRow capacity = Enums.PatternLineRow.One;
      TileColor testingColor = TileColor.Blue;
      PatternLine patternLine = new PatternLine(capacity);
      List<Tile> tiles = new List<Tile>();
      tiles.AddRange(Enumerable.Repeat(new Tile(testingColor), (int)capacity));
      patternLine.PlaceTiles(tiles);

      Tile tile = patternLine.TakeTileForScoring();

      Assert.True(tile.TileColor.Equals(testingColor) && patternLine.EmptySlots == (int)capacity && patternLine.Color == TileColor.None && !patternLine.IsComplete && patternLine.TileCount == 0);
    }
    [Fact]
    public void PatternLine_PlaceTiles_TakingTileForScoringHappy() {
      Enums.PatternLineRow capacity = Enums.PatternLineRow.Five;
      TileColor testingColor = TileColor.Blue;
      PatternLine patternLine = new PatternLine(capacity);
      List<Tile> tiles = new List<Tile>();
      tiles.AddRange(Enumerable.Repeat(new Tile(testingColor), (int)capacity));
      patternLine.PlaceTiles(tiles);

      Tile tile = patternLine.TakeTileForScoring();

      Assert.True(tile.TileColor.Equals(testingColor) && patternLine.EmptySlots == 1 && patternLine.Color == testingColor && !patternLine.IsComplete && patternLine.TileCount == (int)capacity-1);
    }
  }
}

using System;

namespace Azul.Interfaces {
  interface IPlayerState {
    IPlayer Player { get; }

    void PickTilesFromDisplay(IDisplay display, TileColor tileColor);
    void PlacePendingTilesOnPatternLine(PatternLine patternLine);
    void PlacePendingTilesOnFloorLine();
    void MoveTileToWallLine();
  }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Azul.Interfaces {
  public interface IPlayer {
    _Board Board { get; }
    List<Tile> PendingTiles { get; }
    bool OnTurn { get; }
    int Score { get; }

    List<Tile> TakeTiles(TileColor tileColor, IDisplay display);
    List<Tile> PlacePendingTilesOnPatternLine(BoardRow boardRow);
    List<Tile> PlacePendingTilesOnFloorLine();
    void ScoreRow(BoardRow row, int wallPosition, DiscardPile discardPile);
  }
}

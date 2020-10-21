using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul {

  public class Player {

    public event EventHandler TookFirstPlayerTile;
    public event EventHandler ScoredThisRound;
    public event EventHandler TriggeredGameEndCondition;

    public Board Board { get; }
    public int Score => Board.Wall.Score;
    public IReadOnlyCollection<Tile> PendingTiles => pendingTiles.AsReadOnly();

    private List<Tile> pendingTiles;

    public Player(WallSide side) {
      Board = new Board(side);
      pendingTiles = new List<Tile>();
    }

    public void TakeTiles(TileColor tileColor, IDisplay display) {
      var chosenTiles = display.TakeAll(tileColor);
      var startingPlayerTile = chosenTiles.Find(t => t.TileColor.Equals(TileColor.FirstPlayer));

      if(startingPlayerTile != null) {
        Board.PlaceStartingPlayerTile(startingPlayerTile);
        OnPlayerEvent(TookFirstPlayerTile);
        chosenTiles.Remove(startingPlayerTile);
      }

      pendingTiles.AddRange(chosenTiles);
    }

    public void PlacePendingTiles(BoardRow boardRow, DiscardPile discardPile) {
      if(pendingTiles.Count == 0) {
        throw new AzulGameplayException("No tiles staged for placing.");
      }

      var surplusTiles = Board.PlaceOnPatternLine(boardRow, pendingTiles);
      if(surplusTiles.Count > 0) {
        discardPile.Put(surplusTiles);
      }

      pendingTiles.Clear();
    }

    public void PlaceOnFloorLine() {
      Board.PlaceOnFloorLine(pendingTiles);
      pendingTiles.Clear();
    }
    public void ScoreRow(BoardRow row, int wallPosition, DiscardPile discardPile) {
      if(Board.PatternLineFull(row)) {
        var tile = Board.PatternLines[row].First();
        Board.PatternLines[row].Remove(tile);

        if(!Board.Wall.Place(tile, (int)(row) - 1, wallPosition)) {
          discardPile.Put(tile);
        }

        discardPile.Put(Board.PatternLines[row]);
        Board.PatternLines[row].Clear();
      }

      if(Board.Wall.RowFull(row)) {
        OnPlayerEvent(TriggeredGameEndCondition);
      }

      if(Board.AllPatternLinesProcessed()) {
        OnPlayerEvent(ScoredThisRound);
      }
    }

    protected virtual void OnPlayerEvent(EventHandler eh) {
      EventHandler handler = eh;
      handler?.Invoke(this, EventArgs.Empty);
    }

  }
}
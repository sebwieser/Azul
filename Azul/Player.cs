using System.Collections.Generic;

namespace Azul {

  public class Player {
    public Board Board { get; }
    public int Score { get; private set; }
    public IReadOnlyCollection<Tile> PendingTiles => pendingTiles.AsReadOnly();
    public bool ScoredThisRound { get; private set; }
    public bool HasFiveHorizontalTilesInRow { get; private set; }
    public bool TookFirstPlayerTile { get; private set; }

    private Game game;
    private List<Tile> pendingTiles;

    public Player(Game game, WallSide side) {
      this.game = game;
      Board = new Board(side);
      pendingTiles = new List<Tile>();
    }

    public void TakeTiles(TileColor tileColor, IDisplay display) {
      if(game.RoundPhase != RoundPhase.FactoryOffer) {
        throw new AzulGameplayException(string.Format("Cannot take tiles during {0} phase", game.RoundPhase));
      }

      var chosenTiles = display.TakeAll(tileColor);
      var startingPlayerTile = chosenTiles.Find(t => t.TileColor.Equals(TileColor.FirstPlayer));

      if(startingPlayerTile != null) {
        Board.PlaceStartingPlayerTile(startingPlayerTile);
        game.SetNextRoundFirstPlayer(this);
        TookFirstPlayerTile = true;
        chosenTiles.Remove(startingPlayerTile);
      }

      pendingTiles.AddRange(chosenTiles);
    }

    public void PlacePendingTiles(BoardRow boardRow) {
      if(game.RoundPhase != RoundPhase.FactoryOffer) {
        throw new AzulGameplayException(string.Format("Cannot place staged tiles during {0} phase", game.RoundPhase));
      }
      if(pendingTiles.Count == 0) {
        throw new AzulGameplayException("No tiles staged for placing.");
      }

      var surplusTiles = Board.PlaceOnPatternLine(boardRow, pendingTiles);
      game.Discard(surplusTiles);
      pendingTiles.Clear();
    }

    public void PlaceOnFloorLine() {
      Board.PlaceOnFloorLine(pendingTiles);
      pendingTiles.Clear();
    }

    public void CalculateRoundScore() {
      Score += 1;
      ScoredThisRound = true;
    }
  }
}
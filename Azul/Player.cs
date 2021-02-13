using Azul.Enums;
using Azul.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul {

  public class Player : IPlayer {

    internal event EventHandler TookFirstPlayerTile;
    internal event EventHandler ScoredThisRound;
    internal event EventHandler TriggeredGameEndCondition;

    public _Board Board { get; }
    public int Score { get; private set; }
    public virtual List<Tile> PendingTiles { get; set; }
    public bool OnTurn { get; private set; }

    private IGameInternal game;
    private Dictionary<PlayerScoringPhase, PlayerScoringPhase> scoringTransitions;
    private PlayerScoringPhase scoringPhase;

    internal Player(WallSide side, IGameInternal game) {
      Board = new _Board(side);
      PendingTiles = new List<Tile>();
      this.game = game;
      game.ActivePlayerChanged += ActivePlayerChangedHandler;

      scoringPhase = PlayerScoringPhase.NotScoring;
      scoringTransitions = new Dictionary<PlayerScoringPhase, PlayerScoringPhase>() {
        { PlayerScoringPhase.NotScoring, PlayerScoringPhase.ScoringRow1 },
        { PlayerScoringPhase.ScoringRow1, PlayerScoringPhase.ScoringRow2 },
        { PlayerScoringPhase.ScoringRow2, PlayerScoringPhase.ScoringRow3 },
        { PlayerScoringPhase.ScoringRow3, PlayerScoringPhase.ScoringRow4 },
        { PlayerScoringPhase.ScoringRow4, PlayerScoringPhase.ScoringRow5 },
        { PlayerScoringPhase.ScoringRow5, PlayerScoringPhase.ScoringFloorLine },
        { PlayerScoringPhase.ScoringFloorLine, PlayerScoringPhase.ScoringDone },
        { PlayerScoringPhase.ScoringDone, PlayerScoringPhase.NotScoring },
      };
    }

    private PlayerScoringPhase AdvanceScoringPhase() {
      var newPhase = scoringTransitions[scoringPhase];
      if(newPhase == PlayerScoringPhase.ScoringDone) {
        OnPlayerEvent(ScoredThisRound); 
      }
      return newPhase;
    }

    private void ActivePlayerChangedHandler(object sender, Player player) {
      OnTurn = player == this ? true : false;
    }

    public List<Tile> TakeTiles(TileColor tileColor, IDisplay display) {
      if(!OnTurn) {
        throw new AzulGameplayException("Player not on turn.");
      }
      if (game.RoundPhase != RoundPhase.FactoryOffer) {
        throw new AzulGameplayException("Cannot take tiles outside the Factory Offer phase.");
      }

      var chosenTiles = display.TakeAll(tileColor);
      var startingPlayerTile = chosenTiles.Find(t => t.TileColor.Equals(TileColor.FirstPlayer));

      if(startingPlayerTile != null) {
        Board.PlaceStartingPlayerTile(startingPlayerTile);
        OnPlayerEvent(TookFirstPlayerTile);
        chosenTiles.Remove(startingPlayerTile);
      }

      PendingTiles.AddRange(chosenTiles);

      return PendingTiles;
    }

    public List<Tile> PlacePendingTilesOnPatternLine(BoardRow boardRow) {
      if(PendingTiles.Count == 0) {
        throw new AzulGameplayException("No tiles to place. You should Take Tiles first.");
      }
      if(PendingTiles.Exists(t => t.TileColor.Equals(TileColor.FirstPlayer))) {
        throw new AzulGameplayException("Cannot place First Player tile on the Pattern Line, please move it to Floor Line first.");
      }

      var excessTiles = Board.PlaceOnPatternLine(boardRow, PendingTiles);
      PendingTiles.Clear();
      
      return excessTiles;
    }
    public List<Tile> PlacePendingTilesOnFloorLine() {
      if(PendingTiles.Count == 0) {
        throw new AzulGameplayException("No tiles to place. You should Take Tiles first.");
      }

      var excessTiles = Board.PlaceOnFloorLine(PendingTiles);
      PendingTiles.Clear();
      return excessTiles;
    }


    //public int ScoreFloorLine() {
    //  var numTiles = Board.FloorLine.Count;
    //  return 0;
    //}

    public void ScoreRow(BoardRow row, int wallPosition, DiscardPile discardPile) {
      if(Board.PatternLineFull(row)) {
        var tile = Board.PatternLines[row].First();
        Board.PatternLines[row].Remove(tile);

        //if(!Board.Wall.Place(tile, (int)(row) - 1, wallPosition)) {
        //  discardPile.Put(tile);
        //}

        discardPile.Put(Board.PatternLines[row]);
        Board.PatternLines[row].Clear();
      }

      //if(Board.Wall.RowFull(row)) {
      //  OnPlayerEvent(TriggeredGameEndCondition);
      //}

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
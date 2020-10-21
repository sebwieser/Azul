using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul {

  public class Game {
    public int PlayerCount { get; }
    public Player StartingPlayer { get; private set; }
    public Player StartingPlayerNextRound { get; private set; }
    public Player ActivePlayer { get; private set; }
    public RoundPhase RoundPhase { get; private set; }
    public IReadOnlyCollection<Player> Players => players.AsReadOnly();
    public int Round { get; private set; }
    public IReadOnlyCollection<IDisplay> FactoryDisplays => factoryDisplays.AsReadOnly();
    public TileBag TileBag { get; }
    public DiscardPile DiscardPile { get; }
    public IDisplay Centre => centre;

    public IReadOnlyCollection<IDisplay> AllDisplays {
      get {
        var all = new List<IDisplay> {
          centre
        };
        all.AddRange(factoryDisplays);
        return all.AsReadOnly();
      }
    }

    public RoundPhaseStatus RoundPhaseStatus { get; private set; }
    public bool FinalRound { get; private set; }

    private List<FactoryDisplay> factoryDisplays;
    private List<Player> players;
    private Centre centre;
    private bool isSetup;
    private Dictionary<(RoundPhase, RoundPhaseStatus), RoundPhase> phaseTransitions;

    public Game(int playerCount) {
      this.PlayerCount = playerCount;

      if(playerCount < 2 || playerCount > 4) {
        throw new AzulSetupException(string.Format("Cannot start the game with {0} players. Game supports 2-4 players.", playerCount));
      }

      players = new List<Player>(playerCount);
      centre = new Centre();
      factoryDisplays = new List<FactoryDisplay>(2 * playerCount + 1);
      TileBag = new TileBag();
      DiscardPile = new DiscardPile();

      phaseTransitions = new Dictionary<(RoundPhase, RoundPhaseStatus), RoundPhase>()
          {
                    { (RoundPhase.RoundStart, RoundPhaseStatus.NoCondition), RoundPhase.FactoryOffer },
                    { (RoundPhase.FactoryOffer, RoundPhaseStatus.FactoryDisplaysNonEmpty), RoundPhase.FactoryOffer },
                    { (RoundPhase.FactoryOffer, RoundPhaseStatus.FactoryDisplaysEmpty), RoundPhase.WallTiling },
                    { (RoundPhase.WallTiling, RoundPhaseStatus.NotAllPlayersScored), RoundPhase.WallTiling },
                    { (RoundPhase.WallTiling, RoundPhaseStatus.AllPlayersScored), RoundPhase.NextRoundPreparation },
                    { (RoundPhase.NextRoundPreparation, RoundPhaseStatus.GameEndConditionNotMet), RoundPhase.RoundStart },
                    { (RoundPhase.NextRoundPreparation, RoundPhaseStatus.GameEndConditionMet), RoundPhase.GameEnd }
                };

      RoundPhase = RoundPhase.RoundStart;
      RoundPhaseStatus = RoundPhaseStatus.NoCondition;
      Round = 0;
    }

    public RoundPhase AdvancePhase() {
      return phaseTransitions[(RoundPhase, RoundPhaseStatus)];
    }

    public Game Setup() {
      SetupFactoryDisplays();
      SetupPlayers();

      isSetup = true;

      return this;
    }

    private void DecideStartingPlayer() {
      if(StartingPlayerNextRound == null) {
        var rng = new Random();
        ActivePlayer = StartingPlayer = players[rng.Next(0, PlayerCount - 1)];
        return;
      }

      ActivePlayer = StartingPlayer = StartingPlayerNextRound;
      StartingPlayerNextRound = null;
    }

    private void SetupPlayers() {
      for(int i = 0; i < PlayerCount; i++) {
        players.Add(new Player(this, WallSide.Colored));
      }
    }

    private void SetupFactoryDisplays() {
      for(int i = 0; i < (2 * PlayerCount + 1); i++) {
        factoryDisplays.Add(new FactoryDisplay(centre));
      }

      FillFactoryDisplays();
      centre.Put(new Tile(TileColor.FirstPlayer));
    }

    private void FillFactoryDisplays() {
      foreach(var factoryDisplay in factoryDisplays) {
        for(var i = 0; i < 4; i++) {
          if(TileBag.RemainingTiles == 0) {
            var discardedTiles = DiscardPile.TakeAll();
            TileBag.Put(discardedTiles);
          }

          Tile t = TileBag.Fetch();
          factoryDisplay.Put(t);
        }
      }
    }

    public void SetNextRoundFirstPlayer(Player player) {
      if(StartingPlayerNextRound != null) {
        throw new AzulGameplayException("First player already set for next round");
      }
      StartingPlayerNextRound = player;
    }

    public void SetNextRoundFirstPlayer() {
      StartingPlayerNextRound = players.Find(p => p.TookFirstPlayerTile);
    }

    public void Discard(List<Tile> tiles) {
      DiscardPile.Put(tiles);
    }

    private void CheckGameEndCondition() {
      if(!FinalRound) {
        foreach(var patternLine in ActivePlayer.Board.PatternLines) {
          if(patternLine.Value.Count == (int)patternLine.Key && ActivePlayer.Board.Wall.Rows[patternLine.Key].Count >= 4) {
            FinalRound = true;
            return;
          }
        }
      }
    }

    //TODO: this is far from done
    public bool Advance() {
      if(!isSetup) {
        throw new AzulSetupException("Game is not set up!");
      }

      switch(RoundPhase) {
        case RoundPhase.RoundStart:
          Round++;
          RoundPhaseStatus = RoundPhaseStatus.NoCondition;
          DecideStartingPlayer();
          break;

        case RoundPhase.FactoryOffer:
          if(AllDisplaysEmpty()) {
            RoundPhaseStatus = RoundPhaseStatus.FactoryDisplaysEmpty;
            SetNextRoundFirstPlayer();
            break;
          }
          RoundPhaseStatus = RoundPhaseStatus.FactoryDisplaysNonEmpty;
          AdvancePlayerTurnOrder();
          //players take turns picking tiles from displays and place them on the pattern line part of the board
          //this continues until all displays are empty (including centre), triggering next phase
          break;

        case RoundPhase.WallTiling:
          if(AllPlayersScoredThisRound()) {
            RoundPhaseStatus = RoundPhaseStatus.AllPlayersScored;
            break;
          }
          RoundPhaseStatus = RoundPhaseStatus.NotAllPlayersScored;
          break;

        case RoundPhase.NextRoundPreparation:
          CheckGameEndCondition();
          if(FinalRound) {
            RoundPhaseStatus = RoundPhaseStatus.GameEndConditionMet;
            break;
          }
          RoundPhaseStatus = RoundPhaseStatus.GameEndConditionNotMet;
          FillFactoryDisplays();
          break;

        case RoundPhase.GameEnd:
          RoundPhaseStatus = RoundPhaseStatus.NoCondition;
          break;

        default:
          throw new AzulGameplayException("Unknown game state!");
      }

      RoundPhase = AdvancePhase();
      return RoundPhase != RoundPhase.GameEnd;
    }

    private bool AllDisplaysEmpty() {
      return !(factoryDisplays.Any(d => !d.IsEmpty) || !centre.IsEmpty);
    }

    private void AdvancePlayerTurnOrder() {
      ActivePlayer = players[(players.IndexOf(ActivePlayer) + 1) % PlayerCount];
    }

    private bool AllPlayersScoredThisRound() {
      return players.All(p => p.ScoredThisRound);
    }
  }
}
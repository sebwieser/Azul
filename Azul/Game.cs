using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul {

  public class Game : IGameInternal {

    public event EventHandler<Player> ActivePlayerChanged;

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
    public int PlayersScoredThisRound { get; private set; }
    public bool AllPlayersScoredThisRound => PlayerCount == PlayersScoredThisRound;

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
    private Dictionary<(RoundPhase, RoundPhaseStatus), RoundPhase> phaseTransitions;

    public Game(int playerCount) {
      this.PlayerCount = playerCount;

      if(playerCount < 2 || playerCount > 4) {
        throw new AzulSetupException(string.Format("Cannot start the game with {0} players. Game supports 2-4 players.", playerCount));
      }

      TileBag = new TileBag();
      DiscardPile = new DiscardPile();

      players = new List<Player>(playerCount);
      centre = new Centre();
      factoryDisplays = new List<FactoryDisplay>(2 * playerCount + 1);

      RoundPhase = RoundPhase.RoundStart;
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

      CreateFactoryDisplays();
      CreatePlayers();
    }
    private void CreatePlayers() {
      for(int i = 0; i < PlayerCount; i++) {
        var p = new Player(WallSide.Colored, this);
        p.TookFirstPlayerTile += TookFirstPlayerTileHandler;
        p.TriggeredGameEndCondition += TriggeredGameEndConditionHandler;
        p.ScoredThisRound += ScoredThisRoundHandler;
        players.Add(p);
      }
    }
    private void CreateFactoryDisplays() {
      for(int i = 0; i < (2 * PlayerCount + 1); i++) {
        factoryDisplays.Add(new FactoryDisplay(centre));
      }
      centre.Put(new Tile(TileColor.FirstPlayer));
    }
    private void PrepareRound() {
      Round++;
      RoundPhaseStatus = RoundPhaseStatus.NoCondition;
      PlayersScoredThisRound = 0;
      DecideStartingPlayer();
      FillFactoryDisplays();
    }
    private RoundPhase AdvancePhase() {
      var currentRoundPhase = RoundPhase;
      var newRoundPhase = phaseTransitions[(RoundPhase, RoundPhaseStatus)];
      if (currentRoundPhase != newRoundPhase) {
      //  OnRoundPhaseChanged(newRoundPhase);
      }
      return newRoundPhase;
    }

    protected virtual void OnActivePlayerChanged(Player newPlayer) {
      ActivePlayerChanged?.Invoke(this, newPlayer);
    }


    private void DecideStartingPlayer() {
      if(StartingPlayerNextRound == null) {
        var rng = new Random();
        ActivePlayer = StartingPlayer = players[rng.Next(0, PlayerCount - 1)];
        OnActivePlayerChanged(ActivePlayer);
        return;
      }

      ActivePlayer = StartingPlayer = StartingPlayerNextRound;
      OnActivePlayerChanged(ActivePlayer);
      StartingPlayerNextRound = null;
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
    private void TookFirstPlayerTileHandler(object sender, EventArgs eventArgs) {
      StartingPlayerNextRound = (Player)sender;
    }
    private void TriggeredGameEndConditionHandler(object sender, EventArgs eventArgs) {
      if(!FinalRound) {
        FinalRound = true;
      }
    }
    private void ScoredThisRoundHandler(object sender, EventArgs eventArgs) {
      PlayersScoredThisRound++;
    }
    public bool Advance() {
      switch(RoundPhase) {
        case RoundPhase.RoundStart:
          PrepareRound();
          break;
        case RoundPhase.FactoryOffer:
          if(AllDisplaysEmpty()) {
            RoundPhaseStatus = RoundPhaseStatus.FactoryDisplaysEmpty;
            break;
          }
          RoundPhaseStatus = RoundPhaseStatus.FactoryDisplaysNonEmpty;
          AdvancePlayerTurnOrder();
          //players take turns picking tiles from displays and place them on the pattern line part of the board
          //this continues until all displays are empty (including centre), triggering next phase
          break;

        case RoundPhase.WallTiling:
          if(AllPlayersScoredThisRound) {
            RoundPhaseStatus = RoundPhaseStatus.AllPlayersScored;
            break;
          }
          RoundPhaseStatus = RoundPhaseStatus.NotAllPlayersScored;
          break;

        case RoundPhase.NextRoundPreparation:
          if(FinalRound) {
            RoundPhaseStatus = RoundPhaseStatus.GameEndConditionMet;
            break;
          }
          RoundPhaseStatus = RoundPhaseStatus.GameEndConditionNotMet;
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
      OnActivePlayerChanged(ActivePlayer);
    }

    public void Discard(List<Tile> tiles) {
      DiscardPile.Put(tiles);
    }

    public void Discard(Tile tile) {
      DiscardPile.Put(tile);
    }
  }
}
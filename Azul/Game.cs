using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Azul
{
    public class Game
    {
        public int PlayerCount { get; }
        public Player StartingPlayer { get; private set; }
        public Player StartingPlayerNextRound { get; private set; }
        public Player ActivePlayer { get; private set; }
        public RoundPhase RoundPhase { get; private set; }
        public IReadOnlyCollection<Player> Players { get { return _players.AsReadOnly(); } }
        public int Round { get; private set; }
        public IReadOnlyCollection<IDisplay> FactoryDisplays { get { return _factoryDisplays.AsReadOnly(); } }
        public TileBag TileBag { get; }
        public DiscardPile DiscardPile { get; }
        public IDisplay Centre { get { return _centre; } }
        public IReadOnlyCollection<IDisplay> AllDisplays 
        { 
            get 
            { 
                var all = new List<IDisplay>();
                all.Add(_centre);
                all.AddRange(_factoryDisplays);
                return all.AsReadOnly();
            } 
        }
        public RoundPhaseStatus RoundPhaseStatus { get; private set; }
        public bool FinalRound { get; private set; }

        private List<FactoryDisplay> _factoryDisplays;
        private List<Player> _players;
        private Centre _centre;
        private bool _isSetup;
        private Dictionary<(RoundPhase, RoundPhaseStatus), RoundPhase> _phaseTransitions;

        public Game(int playerCount)
        {
            this.PlayerCount = playerCount;

            if(playerCount < 2 || playerCount > 4)
            {
                throw new AzulSetupException(string.Format("Cannot start the game with {0} players. Game supports 2-4 players.", playerCount));
            }

            _players = new List<Player>(playerCount);
            _centre = new Centre();
            _factoryDisplays = new List<FactoryDisplay>(2 * playerCount + 1);
            TileBag = new TileBag();
            DiscardPile = new DiscardPile();

            _phaseTransitions = new Dictionary<(RoundPhase, RoundPhaseStatus), RoundPhase>()
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

        public RoundPhase AdvancePhase()
        {
            return _phaseTransitions[(RoundPhase, RoundPhaseStatus)];
        }

        public Game Setup()
        {
            SetupFactoryDisplays();
            SetupPlayers();

            _isSetup = true;

            return this;
        }

        private void DecideStartingPlayer()
        {
            if(StartingPlayerNextRound == null)
            {
                var rng = new Random();
                ActivePlayer = StartingPlayer = _players[rng.Next(0, PlayerCount - 1)];
                return;
            }

            ActivePlayer = StartingPlayer = StartingPlayerNextRound;
            StartingPlayerNextRound = null;
        }

        private void SetupPlayers()
        {
            for (int i = 0; i < PlayerCount; i++)
            {
                _players.Add(new Player(this, WallSide.Colored));
            }
        }

        private void SetupFactoryDisplays()
        {
            for(int i = 0; i < (2 * PlayerCount + 1); i++)
            {
                _factoryDisplays.Add(new FactoryDisplay(_centre));
            }

            FillFactoryDisplays();
            _centre.Put(new Tile(TileColor.FirstPlayer));
        }

        private void FillFactoryDisplays()
        {
            foreach (var factoryDisplay in _factoryDisplays)
            {
                for (var i = 0; i < 4; i++)
                {
                    if(TileBag.RemainingTiles == 0)
                    {
                        var discardedTiles = DiscardPile.TakeAll();
                        TileBag.Put(discardedTiles);
                    }

                    Tile t = TileBag.Fetch();
                    factoryDisplay.Put(t);
                }
            }
        }

        public void SetNextRoundFirstPlayer(Player player)
        {
            if(StartingPlayerNextRound != null)
            {
                throw new AzulGameplayException("First player already set for next round");
            }
            StartingPlayerNextRound = player;
        }
        public void SetNextRoundFirstPlayer()
        {
            StartingPlayerNextRound = _players.Find(p => p.TookFirstPlayerTile);
        }

        public void Discard(List<Tile> tiles)
        {
            DiscardPile.Put(tiles);
        }

        private void CheckGameEndCondition()
        {
            if (!FinalRound)
            {
                foreach (var patternLine in ActivePlayer.Board.PatternLines)
                {
                    if (patternLine.Value.Count == (int)patternLine.Key && ActivePlayer.Board.Wall.Rows[patternLine.Key].Count >= 4)
                    {
                        FinalRound = true;
                        return;
                    }
                }
            }
        }

        //TODO: this is far from done
        public bool Advance()
        {
            if (!_isSetup)
            {
                throw new AzulSetupException("Game is not set up!");
            }

            switch (RoundPhase)
            {
                case RoundPhase.RoundStart:
                    Round++;
                    RoundPhaseStatus = RoundPhaseStatus.NoCondition;
                    DecideStartingPlayer();
                    break;
                case RoundPhase.FactoryOffer:
                    if (AllDisplaysEmpty())
                    {
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
                    if (AllPlayersScoredThisRound())
                    {
                        RoundPhaseStatus = RoundPhaseStatus.AllPlayersScored;
                        break;
                    }
                    RoundPhaseStatus = RoundPhaseStatus.NotAllPlayersScored;
                    break;
                case RoundPhase.NextRoundPreparation:
                    CheckGameEndCondition();
                    if (FinalRound)
                    {
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

        private bool AllDisplaysEmpty()
        {
            return !(_factoryDisplays.Any(d => !d.IsEmpty) || !_centre.IsEmpty);
        }

        private void AdvancePlayerTurnOrder()
        {
            ActivePlayer = _players[(_players.IndexOf(ActivePlayer) + 1) % PlayerCount];
        }
        private bool AllPlayersScoredThisRound()
        {
            return _players.All(p => p.ScoredThisRound);
        }
    }
}

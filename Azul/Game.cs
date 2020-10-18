using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Azul
{
    public class Game : IGameState
    {
        public int PlayerCount { get; }
        public IPlayerState StartingPlayer { get { return _startingPlayer; } }
        public IPlayerState StartingPlayerNextRound { get { return _startingPlayerNextRound; } }
        public IPlayerState ActivePlayer { get { return _activePlayer; } }
        public RoundPhase RoundPhase { get; private set; }
        public IReadOnlyList<IPlayerState> Players { get { return _players.AsReadOnly(); } }
        public int Round { get; private set; }
        public IEnumerable<IDisplay> FactoryDisplays { get { return _factoryDisplays.AsReadOnly(); } }
        public ITileBagState TileBag { get { return _tileBag; } }
        public IDiscardPileState DiscardPile { get { return _discardPile; } }
        public IDisplay Centre { get { return _centre; } }
        public bool FinalRound { get; private set; }

        private List<FactoryDisplay> _factoryDisplays;
        private List<Player> _players;
        private Centre _centre;
        private TileBag _tileBag;
        private DiscardPile _discardPile;
        private bool isSetup;
        private Player _activePlayer;
        private Player _startingPlayer;
        private Player _startingPlayerNextRound;

        public Game(int playerCount)
        {
            this.PlayerCount = playerCount;

            if(playerCount < 2 || playerCount > 4)
            {
                throw new AzulSetupException(string.Format("Cannot start the game with {0} players. Game supports 2-4 players.", playerCount));
            }

            _players = new List<Player>(playerCount);
            _tileBag = new TileBag();
            _discardPile = new DiscardPile();
            _centre = new Centre();
            _factoryDisplays = new List<FactoryDisplay>(2 * playerCount + 1);

            RoundPhase = RoundPhase.FactoryOffer;
            Round = 1;
        }

        public Game Setup()
        {
            SetupFactoryDisplays();
            SetupPlayers();
            DecideStartingPlayer();

            isSetup = true;

            return this;
        }
        public Game Start()
        {  
            if (!isSetup)
            {
                throw new AzulSetupException("Game is not set up, cannot start.");
            }

            _activePlayer = _startingPlayer;

            return this;
        }

        private void DecideStartingPlayer()
        {
            var rng = new Random();
            _startingPlayer = _players[rng.Next(0, PlayerCount - 1)];
        }

        private void SetupPlayers()
        {
            for (int i = 0; i < PlayerCount; i++)
            {
                _players.Add(new Player(this, WallSide.COLORED));
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
                    if(_tileBag.RemainingTiles == 0)
                    {
                        var discardedTiles = _discardPile.TakeAll();
                        _tileBag.Put(discardedTiles);
                    }

                    Tile t = _tileBag.Fetch();
                    factoryDisplay.Put(t);
                }
            }
        }

        public void SetNextRoundFirstPlayer(Player player)
        {
            if(_startingPlayerNextRound != null)
            {
                throw new AzulGameplayException("First player already set for next round");
            }
            _startingPlayerNextRound = player;
        }

        public void Discard(List<Tile> tiles)
        {
            _discardPile.Put(tiles);
        }

        private void CheckGameEndCondition()
        {
            if (!FinalRound)
            {
                foreach (var patternLine in _activePlayer.Board.PatternLines)
                {
                    if (patternLine.Value.Count == (int)patternLine.Key && _activePlayer.Board.Wall.Rows[patternLine.Key].Count >= 4)
                    {
                        FinalRound = true;
                        return;
                    }
                }
            }
        }

        //TODO: this is far from done
        public void Advance()
        {
            //ze state machine meat goez here
            switch (RoundPhase)
            {
                case RoundPhase.FactoryOffer:
                    CheckGameEndCondition();
                    if (AllDisplaysDepleted())
                    {
                        AdvancePhase();
                        break;
                    }
                    AdvanceTurnOrder();
                    //players take turns picking tiles from displays and place them on the pattern line part of the board
                    //this continues until all displays are empty (including centre), triggering next phase
                    break;
                case RoundPhase.WallTiling:
                    if (AllPlayersScoredThisRound())
                    {
                        AdvancePhase();
                    }
                    //players can simultaniously move their complete patterns to the wall, perform scoring and all those shenanigans
                    //when everyone reports as done, move to final phase of the round
                    break;
                case RoundPhase.NextRoundPreparation:
                    if (FinalRound)
                    {
                        //proceed to scoring and end the game
                        break;
                    }
                    FillFactoryDisplays();
                    break;
                default:
                    throw new AzulGameplayException("Unknown game state!");
            }
        }

        private bool AllDisplaysDepleted()
        {
            foreach(IDisplay d in _factoryDisplays)
            {
                if (!d.IsEmpty)
                {
                    return false;
                }
            }
            return ((IDisplay)_centre).IsEmpty;
        }

        private void AdvanceTurnOrder()
        {
            _activePlayer = _players[(_players.IndexOf(_activePlayer) + 1) % PlayerCount];
        }
        private bool AllPlayersScoredThisRound()
        {
            foreach(var p in _players)
            {
                if (!p.ScoredThisRound)
                {
                    return false;
                }
            }
            return true;
        }
        private void AdvancePhase()
        {
            RoundPhase = (RoundPhase)Enum.Parse(typeof(RoundPhase),(((int)RoundPhase + 1) % Enum.GetNames(typeof(RoundPhase)).Length).ToString());
        }
    }
}

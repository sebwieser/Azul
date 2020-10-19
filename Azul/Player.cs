using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Azul
{
    public class Player
    {
        public Board Board { get; }
        public int Score { get; private set; }
        public IReadOnlyCollection<Tile> PendingTiles { get { return _pendingTiles.AsReadOnly(); } }
        public bool ScoredThisRound { get; private set; }
        public bool HasFiveHorizontalTilesInRow { get; private set; }
        public bool TookFirstPlayerTile { get; private set; }

        private Game _game;
        private List<Tile> _pendingTiles;

        public Player(Game game, WallSide side)
        {
            _game = game;
            Board = new Board(side);
            _pendingTiles = new List<Tile>();
        }

        public void TakeTiles(TileColor tileColor, IDisplay display)
        {
            if(_game.RoundPhase != RoundPhase.FactoryOffer)
            {
                throw new AzulGameplayException(string.Format("Cannot take tiles during {0} phase", _game.RoundPhase));
            }

            var chosenTiles = display.TakeAll(tileColor);
            var startingPlayerTile = chosenTiles.Find(t => t.TileColor.Equals(TileColor.FirstPlayer));
            
            if (startingPlayerTile != null)
            {
                Board.PlaceStartingPlayerTile(startingPlayerTile);
                _game.SetNextRoundFirstPlayer(this);
                TookFirstPlayerTile = true;
                chosenTiles.Remove(startingPlayerTile);
            }

            _pendingTiles.AddRange(chosenTiles);
        }

        public void PlacePendingTiles(BoardRow boardRow)
        {
            if (_game.RoundPhase != RoundPhase.FactoryOffer)
            {
                throw new AzulGameplayException(string.Format("Cannot place staged tiles during {0} phase", _game.RoundPhase));
            }
            if (_pendingTiles.Count == 0)
            {
                throw new AzulGameplayException("No tiles staged for placing.");
            }

            var surplusTiles = Board.PlaceOnPatternLine(boardRow, _pendingTiles);
            _game.Discard(surplusTiles);
            _pendingTiles.Clear();
        }
        public void PlaceOnFloorLine()
        {
            Board.PlaceOnFloorLine(_pendingTiles);
            _pendingTiles.Clear();
        }
        public void CalculateRoundScore()
        {
            Score += 1;
            ScoredThisRound = true;
        }
    }
}
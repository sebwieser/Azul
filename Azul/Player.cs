using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Azul
{
    public class Player : IPlayerState
    {
        public IBoardState Board { get { return _board; } }
        public int Score { get; private set; }
        public IEnumerable<Tile> PendingTiles { get { return _pendingTiles.AsReadOnly(); } }
        public bool ScoredThisRound { get; private set; }

        private IGameState _gameState;
        private Board _board;
        private List<Tile> _pendingTiles;

        public Player(IGameState gameState, WallSide side)
        {
            _gameState = gameState;
            _board = new Board(side);
            _pendingTiles = new List<Tile>();
        }

        public void TakeTiles(TileColor tileColor, AbstractDisplay display)
        {
            if(_gameState.RoundPhase != RoundPhase.FactoryOffer)
            {
                throw new AzulGameplayException(string.Format("Cannot take tiles during {0} phase", _gameState.RoundPhase));
            }

            var chosenTiles = display.TakeAll(tileColor);
            var startingPlayerTile = chosenTiles.Find(t => t.TileColor.Equals(TileColor.FirstPlayer));
            
            if (startingPlayerTile != null)
            {
                _board.PlaceStartingPlayerTile(startingPlayerTile);
                _gameState.SetNextRoundFirstPlayer(this);
                chosenTiles.Remove(startingPlayerTile);
            }

            _pendingTiles.AddRange(chosenTiles);
        }

        public void PlacePendingTiles(BoardRow boardRow)
        {
            if (_gameState.RoundPhase != RoundPhase.FactoryOffer)
            {
                throw new AzulGameplayException(string.Format("Cannot place staged tiles during {0} phase", _gameState.RoundPhase));
            }
            if (_pendingTiles.Count == 0)
            {
                throw new AzulGameplayException("No tiles staged for placing.");
            }

            var surplusTiles = _board.PlaceOnPatternLine(boardRow, _pendingTiles);
            _gameState.Discard(surplusTiles);
        }
        public void PlaceOnFloorLine()
        {
            _board.PlaceOnFloorLine(_pendingTiles);
            _pendingTiles.Clear();
        }
        public void CalculateRoundScore()
        {
            throw new NotImplementedException();
        }
    }
}
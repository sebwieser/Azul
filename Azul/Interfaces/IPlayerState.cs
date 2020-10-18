using System.Collections.Generic;

namespace Azul
{
    public interface IPlayerState
    {
        int Score { get; }
        IEnumerable<Tile> PendingTiles { get; }
        IBoardState Board { get; }
        bool ScoredThisRound { get; }
    }
}
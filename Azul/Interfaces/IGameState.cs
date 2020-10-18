using System.Collections.Generic;

namespace Azul
{
    public interface IGameState
    {
        IPlayerState StartingPlayer { get; }
        IPlayerState StartingPlayerNextRound { get; }
        RoundPhase RoundPhase { get; }
        IPlayerState ActivePlayer { get; }
        int Round { get; }
        IEnumerable<IDisplay> FactoryDisplays { get; }
        bool FinalRound { get; }

        void SetNextRoundFirstPlayer(Player player);
        void Discard(List<Tile> tiles);
    }
}
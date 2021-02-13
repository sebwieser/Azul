using System;
using System.Collections.Generic;

namespace Azul {
  internal interface IGameInternal {
    event EventHandler<Player> ActivePlayerChanged;

    RoundPhase RoundPhase { get; }

    void Discard(List<Tile> tiles);
    void Discard(Tile tile);
  }

  public interface IGame {

  }
}
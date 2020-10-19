using System;
using System.Collections.Generic;
using System.Text;

namespace Azul
{
    public class DiscardPile
    {
        public IReadOnlyCollection<Tile> Tiles { get { return _tiles.AsReadOnly(); } }

        private List<Tile> _tiles;
        public DiscardPile()
        {
            _tiles = new List<Tile>();
        }
        public void Put(List<Tile> discardedTiles)
        {
            _tiles.AddRange(discardedTiles);
        }
        public List<Tile> TakeAll()
        {
            var allTiles = new List<Tile>(_tiles);
            _tiles.Clear();
            return allTiles;
        }
    }
}

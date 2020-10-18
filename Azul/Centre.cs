using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul
{
    public class Centre: AbstractDisplay, IDisplay
    {
        public IEnumerable<Tile> Tiles { get { return _tiles.AsReadOnly(); } }
        public bool IsEmpty { get { return _tiles.Count == 0; } }

        private List<Tile> _tiles;

        public Centre()
        {
            _tiles = new List<Tile>();
        }

        public void Put(List<Tile> tiles)
        {
            _tiles.AddRange(tiles);
        }

        public override void Put(Tile tile)
        {
            _tiles.Add(tile);
        }

        public override List<Tile> TakeAll(TileColor tileColor)
        {
            var takenTiles = _tiles.FindAll(t => t.TileColor.Equals(tileColor) || t.TileColor.Equals(TileColor.FirstPlayer));
            _tiles = _tiles.Except(takenTiles).ToList();

            return takenTiles;
        }
    }
}
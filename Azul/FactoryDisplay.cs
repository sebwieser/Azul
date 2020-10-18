using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul
{
    public class FactoryDisplay : AbstractDisplay, IDisplay
    {
        private static short MAX_TILES = 4;

        public IEnumerable<Tile> Tiles { get { return _tiles.AsReadOnly(); } }
        public bool IsEmpty { get { return _tiles.Count == 0; } }

        private List<Tile> _tiles;
        private Centre _centre;

        public FactoryDisplay(Centre centre)
        {
            this._centre = centre;
            _tiles = new List<Tile>(MAX_TILES);
        }

        public override List<Tile> TakeAll(TileColor tileColor)
        {
            var chosenTiles = _tiles.FindAll(t => t.TileColor.Equals(tileColor));
            _tiles = _tiles.Except(chosenTiles).ToList();

            //Move remaining tiles to the middle
            if (_tiles.Count > 0)
            {
                _centre.Put(_tiles);
                _tiles.Clear();
            }

            return chosenTiles;
        }

        public override void Put(Tile tile)
        {
            if (_tiles.Count == MAX_TILES)
            {
                throw new AzulSetupException("Cannot add tile, factory display full");
            }
            
            _tiles.Add(tile);
        }
    }
}
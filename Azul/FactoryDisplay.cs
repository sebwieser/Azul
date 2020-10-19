using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul
{
    public class FactoryDisplay : IDisplay
    {
        private static short MAX_TILES = 4;

        public DisplayType Type { get { return DisplayType.FactoryDisplay; } }
        public IReadOnlyCollection<Tile> Tiles { get { return _tiles.AsReadOnly(); } }
        public bool IsEmpty { get { return _tiles.Count == 0; } }

        private List<Tile> _tiles;
        private Centre _centre;

        public FactoryDisplay(Centre centre)
        {
            this._centre = centre;
            _tiles = new List<Tile>(MAX_TILES);
        }

        public List<Tile> TakeAll(TileColor tileColor)
        {
            var chosenTiles = _tiles.FindAll(t => t.TileColor.Equals(tileColor));
            var remainingTiles = _tiles.Except(chosenTiles).ToList();

            //Move remaining tiles to the middle
            if (remainingTiles.Count > 0)
            {
                _centre.Put(remainingTiles);
                _tiles.Clear();
            }

            return chosenTiles;
        }

        public void Put(Tile tile)
        {
            if (_tiles.Count == MAX_TILES)
            {
                throw new AzulSetupException("Cannot add tile, factory display full");
            }
            
            _tiles.Add(tile);
        }
    }
}
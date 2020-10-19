using System;
using System.Collections.Generic;
using System.Linq;

namespace Azul
{
    public class TileBag
    {
        private static int MAX_TILES = 100;

        public int RemainingTiles { get { return _tiles.Count; } }
        public IReadOnlyCollection<Tile> Tiles { get { return _tiles.AsReadOnly(); } }


        private List<Tile> _tiles;
        private Random _rng;

        public TileBag()
        {
            _tiles = new List<Tile>(MAX_TILES);
            _rng = new Random();

            //All tile types except for the first player token go into bag
            var tileTypes = Enum.GetValues(typeof(TileColor))
                .Cast<TileColor>()
                .Where(tc => tc != TileColor.FirstPlayer)
                .ToList();

            //Put 20 of tile pieces of each color into bag
            foreach (var tileColor in tileTypes)
            {
                for(int i = 0; i < 20; i++)
                {
                    _tiles.Add(new Tile(tileColor));
                }
            }
        }

        public Tile Fetch()
        {
            if (RemainingTiles == 0)
            {
                throw new AzulGameplayException("Cannot fetch tile from an empty bag.");
            }

            var tile = _tiles[_rng.Next(0, _tiles.Count - 1)];
            _tiles.Remove(tile);
            return tile;
        }

        public void Put(List<Tile> newTiles)
        {
            if ((newTiles.Count + RemainingTiles) > 100)
            {
                throw new AzulGameplayException("Cannot put tiles into the bag as it would exceed the maximum bag size.");
            }

            _tiles.AddRange(newTiles);
        }
    }
}
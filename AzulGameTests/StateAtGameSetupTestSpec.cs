using NUnit.Framework;
using Azul;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Immutable;

namespace Azul.Tests
{
    public class StateAtGameSetupTestSpec
    {
        private Game game2p;
        private Game game3p;
        private Game game4p;

        [SetUp]
        public void Setup()
        {
            game2p = new Game(2).Setup();
            game3p = new Game(3).Setup();
            game4p = new Game(4).Setup();
        }
        [Test]
        public void TestStartingPlayerDecided()
        {
            Assert.Contains(game2p.StartingPlayer, game2p.Players.ToList());
            Assert.Contains(game3p.StartingPlayer, game3p.Players.ToList());
            Assert.Contains(game4p.StartingPlayer, game4p.Players.ToList());

            var startingPlayer2p = game2p.Players.Where(p => p == game2p.StartingPlayer);
            var startingPlayer3p = game3p.Players.Where(p => p == game3p.StartingPlayer);
            var startingPlayer4p = game4p.Players.Where(p => p == game4p.StartingPlayer);

            Assert.IsNotNull(startingPlayer2p);
            Assert.IsNotNull(startingPlayer3p);
            Assert.IsNotNull(startingPlayer4p);
        }
        [Test]
        public void TestNextRoundFirstPlayerUnknown()
        {
            Assert.IsNull(game2p.StartingPlayerNextRound);
            Assert.IsNull(game3p.StartingPlayerNextRound);
            Assert.IsNull(game4p.StartingPlayerNextRound);
        }
        [Test]
        public void TestGameCannotStartWithoutSetup()
        {
            for (int playerCount = 2; playerCount <= 4; playerCount++)
            {
                Game game = new Game(playerCount);
                Assert.Throws(typeof(AzulSetupException), () => { game.Start(); });
            }
        }
        [Test]
        public void TestGameSupports2To4Players()
        {
            Assert.Throws(typeof(AzulSetupException), () => { new Game(1); });
            Assert.Throws(typeof(AzulSetupException), () => { new Game(-1); });
            Assert.Throws(typeof(AzulSetupException), () => { new Game(0); });
            Assert.Throws(typeof(AzulSetupException), () => { new Game(5); });
            Assert.Throws(typeof(AzulSetupException), () => { new Game(1000); });
        }
        [Test]
        public void TestTilesInBag()
        {
            for (int playerCount = 2; playerCount <= 4; playerCount++)
            {
                Game game = new Game(playerCount);

                //before the game is set up, bag should contain 20 pieces of each tile color
                List<Tile> extractedTiles = game.TileBag.Tiles.ToList();

                Assert.AreEqual(100, extractedTiles.Count);
                Assert.AreEqual(100, game.TileBag.RemainingTiles);
                Assert.AreEqual(20, extractedTiles.FindAll(t => t.TileColor.Equals(TileColor.Black)).Count);
                Assert.AreEqual(20, extractedTiles.FindAll(t => t.TileColor.Equals(TileColor.Red)).Count);
                Assert.AreEqual(20, extractedTiles.FindAll(t => t.TileColor.Equals(TileColor.Yellow)).Count);
                Assert.AreEqual(20, extractedTiles.FindAll(t => t.TileColor.Equals(TileColor.Blue)).Count);
                Assert.AreEqual(20, extractedTiles.FindAll(t => t.TileColor.Equals(TileColor.Turquoise)).Count);
            }

        }
        [Test]
        public void TestNumberOfFactoryDisplays()
        {
            Assert.AreEqual(5, game2p.FactoryDisplays.Count());
            Assert.AreEqual(7, game3p.FactoryDisplays.Count());
            Assert.AreEqual(9, game4p.FactoryDisplays.Count());
        }
        [Test]
        public void TestNumberOfTilesInEachFactoryDisplay()
        {
            foreach (var fd in game2p.FactoryDisplays)
            {
                Assert.AreEqual(4, fd.Tiles.Count());
            }
            foreach (var fd in game3p.FactoryDisplays)
            {
                Assert.AreEqual(4, fd.Tiles.Count());
            }
            foreach (var fd in game4p.FactoryDisplays)
            {
                Assert.AreEqual(4, fd.Tiles.Count());
            }
        }
        [Test]
        public void TestDiscardPileEmptyAtStart()
        {
            Assert.AreEqual(0, game2p.DiscardPile.Tiles.Count());
            Assert.AreEqual(0, game3p.DiscardPile.Tiles.Count());
            Assert.AreEqual(0, game4p.DiscardPile.Tiles.Count());
        }
        [Test]
        public void TestCentreDisplayContainsOnlyStartingTileAtStart()
        {
            Assert.AreEqual(1, game2p.Centre.Tiles.Count());
            Assert.AreEqual(1, game3p.Centre.Tiles.Count()); 
            Assert.AreEqual(1, game4p.Centre.Tiles.Count());
        }
        [Test]
        public void TestNumberOfPlayers()
        {
            Assert.AreEqual(2, game2p.PlayerCount);
            Assert.AreEqual(2, game2p.Players.Count);

            Assert.AreEqual(3, game3p.PlayerCount);
            Assert.AreEqual(3, game3p.Players.Count);

            Assert.AreEqual(4, game4p.PlayerCount);
            Assert.AreEqual(4, game4p.Players.Count);
        }
        [Test]
        public void TestRemainingBagTilesAfterSetup()
        {
            Assert.AreEqual(100 - (5 * 4), game2p.TileBag.RemainingTiles);
            Assert.AreEqual(100 - (7 * 4), game3p.TileBag.RemainingTiles);
            Assert.AreEqual(100 - (9 * 4), game4p.TileBag.RemainingTiles);
        }
    }
}
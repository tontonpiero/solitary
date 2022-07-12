using NUnit.Framework;
using Solitary.Core;
using System.Collections.Generic;

namespace Solitary.Tests
{
    class UnitTestGame
    {
        [Test]
        public void Test_Start_Game()
        {
            Game game = new Game();

            Assert.That(game.State, Is.EqualTo(GameState.NotStarted));

            game.Start();

            Assert.That(game.State, Is.EqualTo(GameState.Started));
            Assert.That(game.StockDeck, Is.Not.Null);
            Assert.That(game.WasteDeck, Is.Not.Null);
            Assert.That(game.WasteDeck.Count, Is.EqualTo(0));
            Assert.That(game.ColumnDecks, Is.Not.Null);
            Assert.That(game.ColumnDecks.Length, Is.EqualTo(Game.ColumnsCount));
            Assert.That(game.FoundationDecks, Is.Not.Null);
            Assert.That(game.FoundationDecks.Length, Is.EqualTo(Game.FoundationsCount));

            int columnCardsCount = 0;
            for (int i = 0; i < Game.ColumnsCount; i++)
            {
                Assert.That(game.ColumnDecks[i], Is.Not.Null);
                Assert.That(game.ColumnDecks[i].Count, Is.EqualTo(i + 1));
                columnCardsCount += game.ColumnDecks[i].Count;
            }

            for (int i = 0; i < Game.FoundationsCount; i++)
            {
                Assert.That(game.FoundationDecks[i], Is.Not.Null);
                Assert.That(game.FoundationDecks[i].Count, Is.EqualTo(0));
            }

            Assert.That(game.StockDeck.Count, Is.EqualTo(52 - columnCardsCount));
        }
    }
}

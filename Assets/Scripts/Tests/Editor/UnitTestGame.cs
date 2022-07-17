using NUnit.Framework;
using Solitary.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Solitary.Tests
{
    class UnitTestGame
    {
        private Card.Factory cardFactory = new Card.Factory();

        [Test]
        public void Test_Start_Game()
        {
            Game game = CreateTestableGame(out _);

            Assert.That(game.State, Is.EqualTo(GameState.NotStarted));

            game.Start();

            Assert.That(game.State, Is.EqualTo(GameState.Started));
            Assert.That(game.StockDeck, Is.Not.Null);
            Assert.That(game.StockDeck.Count, Is.EqualTo(52));
            Assert.That(game.WasteDeck, Is.Not.Null);
            Assert.That(game.WasteDeck.Count, Is.EqualTo(0));
            Assert.That(game.ColumnDecks, Is.Not.Null);
            Assert.That(game.ColumnDecks.Length, Is.EqualTo(Game.ColumnsCount));
            Assert.That(game.FoundationDecks, Is.Not.Null);
            Assert.That(game.FoundationDecks.Length, Is.EqualTo(Game.FoundationsCount));
        }

        [Test]
        public void Test_Initialize_Game_Columns()
        {
            Game game = CreateTestableGame(out _);
            game.Start();

            game.InitializeColumns();

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

        [Test]
        public void Test_CanMoveCard()
        {
            Game game = CreateTestableGame(out _);
            game.Start();

            Assert.That(game.CanMoveCard(game.StockDeck, game.WasteDeck, game.StockDeck.TopCard), Is.True);
            Assert.That(game.CanMoveCard(game.StockDeck, game.FoundationDecks[0], game.StockDeck.TopCard), Is.False);
            Assert.That(game.CanMoveCard(game.StockDeck, game.ColumnDecks[0], game.StockDeck.TopCard), Is.False);
        }

        [Test]
        public void Test_MoveCommand()
        {
            Game game = CreateTestableGame(out TestCommandInvoker commandInvoker);
            game.Start();

            int moveAmount = 3;
            int initialStockCount = game.StockDeck.Count;
            int initialWasteCount = game.WasteDeck.Count;
            Card initialStockTopCard = game.StockDeck.TopCard;
            game.MoveCards(game.StockDeck, game.WasteDeck, moveAmount);

            Assert.That(game.StockDeck.Count, Is.EqualTo(initialStockCount - moveAmount));
            Assert.That(game.WasteDeck.Count, Is.EqualTo(initialWasteCount + moveAmount));
            Assert.That(game.WasteDeck.TopCard, Is.EqualTo(initialStockTopCard));
            Assert.That(commandInvoker.Count, Is.EqualTo(1));
        }

        [Test]
        public void Test_UndoCommand()
        {
            Game game = CreateTestableGame(out TestCommandInvoker commandInvoker);
            game.Start();

            int moveAmount = 3;
            int initialStockCount = game.StockDeck.Count;
            int initialWasteCount = game.WasteDeck.Count;
            Card initialStockTopCard = game.StockDeck.TopCard;

            game.MoveCards(game.StockDeck, game.WasteDeck, moveAmount);
            game.UndoLastMove();

            Assert.That(game.StockDeck.Count, Is.EqualTo(initialStockCount));
            Assert.That(game.WasteDeck.Count, Is.EqualTo(initialWasteCount));
            Assert.That(game.StockDeck.TopCard, Is.EqualTo(initialStockTopCard));
            Assert.That(commandInvoker.Count, Is.EqualTo(0));
        }

        [Test]
        public void Test_Can_Move_From_Stock_To_Waste()
        {
            Game game = CreateTestableGame(out _);
            game.Start();

            Assert.That(game.CanMoveCard(game.StockDeck, game.WasteDeck, game.StockDeck.TopCard), Is.True);
        }

        [Test]
        public void Test_Cant_Move_From_Stock_To_Fundation()
        {
            Game game = CreateTestableGame(out _);
            game.Start();

            Assert.That(game.CanMoveCard(game.StockDeck, game.FoundationDecks[0], game.StockDeck.TopCard), Is.False);
        }

        [Test]
        public void Test_Cant_Move_From_Stock_To_Column()
        {
            Game game = CreateTestableGame(out _);
            game.Start();

            Assert.That(game.CanMoveCard(game.StockDeck, game.ColumnDecks[0], game.StockDeck.TopCard), Is.False);
        }

        [Test]
        public void Test_Cant_Move_From_Stock_To_Stock()
        {
            Game game = CreateTestableGame(out _);
            game.Start();

            Assert.That(game.CanMoveCard(game.StockDeck, game.StockDeck, game.StockDeck.TopCard), Is.False);
        }

        [Test]
        public void Test_Cant_Move_From_Waste_To_Stock()
        {
            Game game = CreateTestableGame(out _);
            game.Start();
            game.MoveCards(game.StockDeck, game.WasteDeck, 1);

            Assert.That(game.CanMoveCard(game.WasteDeck, game.StockDeck, game.WasteDeck.TopCard), Is.False);
        }

        [Test]
        public void Test_Cant_Move_From_Waste_To_Waste()
        {
            Game game = CreateTestableGame(out _);
            game.Start();
            game.MoveCards(game.StockDeck, game.WasteDeck, 1);

            Assert.That(game.CanMoveCard(game.WasteDeck, game.WasteDeck, game.WasteDeck.TopCard), Is.False);
        }

        [Test]
        public void Test_Can_Move_From_Waste_To_Foundation()
        {
            Game game = CreateTestableGame(out _);
            game.Start();
            game.StockDeck.Push(cardFactory.Create(CardRank.Ace, CardSuit.Hearts));
            game.MoveCards(game.StockDeck, game.WasteDeck, 1);

            Assert.That(game.CanMoveCard(game.WasteDeck, game.FoundationDecks[0], game.WasteDeck.TopCard), Is.True);
        }

        [Test]
        public void Test_Can_Move_From_Waste_To_Column()
        {
            Game game = CreateTestableGame(out _);
            game.Start();
            game.StockDeck.Push(cardFactory.Create(CardRank.King, CardSuit.Hearts));
            game.MoveCards(game.StockDeck, game.WasteDeck, 1);

            Assert.That(game.CanMoveCard(game.WasteDeck, game.ColumnDecks[0], game.WasteDeck.TopCard), Is.True);
        }

        [Test]
        public void Test_Can_Move_From_Column_To_Column()
        {
            Game game = CreateTestableGame(out _);
            game.Start();
            game.ColumnDecks[0].Push(cardFactory.Create(CardRank.Eight, CardSuit.Hearts));
            game.ColumnDecks[1].Push(cardFactory.Create(CardRank.Nine, CardSuit.Spades));

            Assert.That(game.CanMoveCard(game.ColumnDecks[0], game.ColumnDecks[1], game.ColumnDecks[0].TopCard), Is.True);
            Assert.That(game.CanMoveCard(game.ColumnDecks[1], game.ColumnDecks[0], game.ColumnDecks[1].TopCard), Is.False);
        }

        [Test]
        public void Test_Cant_Move_From_Column_To_Waste()
        {
            Game game = CreateTestableGame(out _);
            game.Start();

            Assert.That(game.CanMoveCard(game.ColumnDecks[0], game.WasteDeck, game.ColumnDecks[0].TopCard), Is.False);
        }

        [Test]
        public void Test_Cant_Move_From_Column_To_Stock()
        {
            Game game = CreateTestableGame(out _);
            game.Start();

            Assert.That(game.CanMoveCard(game.ColumnDecks[0], game.StockDeck, game.ColumnDecks[0].TopCard), Is.False);
        }

        [Test]
        public void Test_Can_Move_From_Column_To_Foundation()
        {
            Game game = CreateTestableGame(out _);
            game.Start();
            game.ColumnDecks[0].Push(cardFactory.Create(CardRank.Ace, CardSuit.Hearts));

            Assert.That(game.CanMoveCard(game.ColumnDecks[0], game.FoundationDecks[0], game.ColumnDecks[0].TopCard), Is.True);
            Assert.That(game.CanMoveCard(game.ColumnDecks[0], game.FoundationDecks[1], game.ColumnDecks[0].TopCard), Is.False);
        }

        [Test]
        public void Test_Cant_Move_From_Foundation_To_Foundation()
        {
            Game game = CreateTestableGame(out _);
            game.Start();
            game.FoundationDecks[0].Push(cardFactory.Create(CardRank.Ace, CardSuit.Hearts));

            Assert.That(game.CanMoveCard(game.FoundationDecks[0], game.FoundationDecks[1], game.FoundationDecks[0].TopCard), Is.False);
        }

        [Test]
        public void Test_Cant_Move_From_Foundation_To_Waste()
        {
            Game game = CreateTestableGame(out _);
            game.Start();
            game.FoundationDecks[0].Push(cardFactory.Create(CardRank.Ace, CardSuit.Hearts));

            Assert.That(game.CanMoveCard(game.FoundationDecks[0], game.WasteDeck, game.FoundationDecks[0].TopCard), Is.False);
        }

        [Test]
        public void Test_Cant_Move_From_Foundation_To_Stock()
        {
            Game game = CreateTestableGame(out _);
            game.Start();
            game.FoundationDecks[0].Push(cardFactory.Create(CardRank.Ace, CardSuit.Hearts));

            Assert.That(game.CanMoveCard(game.FoundationDecks[0], game.StockDeck, game.FoundationDecks[0].TopCard), Is.False);
        }

        [Test]
        public void Test_Can_Move_From_Foundation_To_Column()
        {
            Game game = CreateTestableGame(out _);
            game.Start();
            game.FoundationDecks[0].Push(cardFactory.Create(CardRank.Ace, CardSuit.Hearts));
            game.ColumnDecks[0].Push(cardFactory.Create(CardRank.Two, CardSuit.Spades));
            game.ColumnDecks[1].Push(cardFactory.Create(CardRank.Five, CardSuit.Spades));

            Assert.That(game.CanMoveCard(game.FoundationDecks[0], game.ColumnDecks[0], game.FoundationDecks[0].TopCard), Is.True);
            Assert.That(game.CanMoveCard(game.FoundationDecks[0], game.ColumnDecks[1], game.FoundationDecks[0].TopCard), Is.False);
        }

        [Test]
        public void Test_Game_Score_And_Moves()
        {
            Game game = CreateTestableGame(out _);
            game.Start();

            game.ColumnDecks[0].Push(cardFactory.Create(CardRank.Seven, CardSuit.Hearts));
            game.WasteDeck.Push(cardFactory.Create(CardRank.Five, CardSuit.Diamonds));
            game.WasteDeck.Push(cardFactory.Create(CardRank.Six, CardSuit.Spades));

            Assert.That(game.Score, Is.EqualTo(0));
            Assert.That(game.Moves, Is.EqualTo(0));

            game.MoveCards(game.WasteDeck, game.ColumnDecks[0], 1);

            Assert.That(game.Score, Is.EqualTo(ScoreCalculator.WasteToColumnPoints));
            Assert.That(game.Moves, Is.EqualTo(1));

            game.MoveCards(game.WasteDeck, game.ColumnDecks[0], 1);

            Assert.That(game.Score, Is.EqualTo(ScoreCalculator.WasteToColumnPoints * 2));
            Assert.That(game.Moves, Is.EqualTo(2));

            game.UndoLastMove();

            Assert.That(game.Score, Is.EqualTo(ScoreCalculator.WasteToColumnPoints));
            Assert.That(game.Moves, Is.EqualTo(3));

            game.MoveCards(game.WasteDeck, game.ColumnDecks[0], 1);

            Assert.That(game.Score, Is.EqualTo(ScoreCalculator.WasteToColumnPoints * 2));
            Assert.That(game.Moves, Is.EqualTo(4));
        }

        private Game CreateTestableGame(out TestCommandInvoker commandInvoker)
        {
            commandInvoker = new TestCommandInvoker();
            return new Game.Builder()
                .WithCommandInvoker(commandInvoker)
                .Build();
        }

        public class TestCommandInvoker : CommandInvoker
        {
            public int Count => commands.Count;
        }
    }
}

using NUnit.Framework;
using Solitary.Core;

namespace Solitary.Tests
{
    class UnitTestScoreCalculator
    {
        private IDeckFactory deckFactory = new Deck.Factory();
        private ICardFactory cardFactory = new Card.Factory();

        [Test]
        public void Test_Reserve_To_Foundation()
        {
            ReserveDeck ReserveDeck = deckFactory.CreateReserveDeck();
            FoundationDeck foundationDeck = deckFactory.CreateFoundationDeck(CardSuit.Hearts);

            int points = ScoreCalculator.GetMovePoints(ReserveDeck, foundationDeck, 1, new GameSettings());
            Assert.That(points, Is.EqualTo(ScoreCalculator.ReserveToFoundationPoints));
        }

        [Test]
        public void Test_Reserve_To_Column()
        {
            ReserveDeck ReserveDeck = deckFactory.CreateReserveDeck();
            ColumnDeck columnDeck = deckFactory.CreateColumnDeck(0);

            int points = ScoreCalculator.GetMovePoints(ReserveDeck, columnDeck, 1, new GameSettings());
            Assert.That(points, Is.EqualTo(ScoreCalculator.ReserveToColumnPoints));
        }

        [Test]
        public void Test_Column_To_Foundation()
        {
            ColumnDeck columnDeck = deckFactory.CreateColumnDeck(0);
            FoundationDeck foundationDeck = deckFactory.CreateFoundationDeck(CardSuit.Hearts);

            int points = ScoreCalculator.GetMovePoints(columnDeck, foundationDeck, 1, new GameSettings());
            Assert.That(points, Is.EqualTo(ScoreCalculator.ColumnToFoundationPoints));
        }

        [Test]
        public void Test_Foundation_To_Column()
        {
            FoundationDeck foundationDeck = deckFactory.CreateFoundationDeck(CardSuit.Hearts);
            ColumnDeck columnDeck = deckFactory.CreateColumnDeck(0);

            int points = ScoreCalculator.GetMovePoints(foundationDeck, columnDeck, 1, new GameSettings());
            Assert.That(points, Is.EqualTo(ScoreCalculator.FoundationToColumnPoints));
        }

        [Test]
        public void Test_Column_To_Column()
        {
            ColumnDeck columnDeck1 = deckFactory.CreateColumnDeck(0);
            ColumnDeck columnDeck2 = deckFactory.CreateColumnDeck(0);

            int points = ScoreCalculator.GetMovePoints(columnDeck1, columnDeck2, 1, new GameSettings());
            Assert.That(points, Is.EqualTo(0));
        }

        [Test]
        public void Test_TurnOverCard()
        {
            ColumnDeck columnDeck1 = deckFactory.CreateColumnDeck(0);
            ColumnDeck columnDeck2 = deckFactory.CreateColumnDeck(0);

            Card card = cardFactory.Create(CardRank.Queen, CardSuit.Clubs);
            columnDeck1.Push(card);
            card.Hide();
            columnDeck1.Push(cardFactory.Create(CardRank.Five, CardSuit.Hearts));

            int points = ScoreCalculator.GetMovePoints(columnDeck1, columnDeck2, 1, new GameSettings());
            Assert.That(points, Is.EqualTo(ScoreCalculator.TurnOverColumnCardPoints));

            points = ScoreCalculator.GetMovePoints(columnDeck2, columnDeck1, 1, new GameSettings());
            Assert.That(points, Is.EqualTo(0));
        }

        [Test]
        public void Test_TurnOverCard_WhileMovingSeveralCards()
        {
            ColumnDeck columnDeck1 = deckFactory.CreateColumnDeck(0);
            ColumnDeck columnDeck2 = deckFactory.CreateColumnDeck(0);

            Card card = cardFactory.Create(CardRank.Queen, CardSuit.Clubs);
            columnDeck1.Push(card);
            card.Hide();
            columnDeck1.Push(cardFactory.Create(CardRank.Five, CardSuit.Hearts));
            columnDeck1.Push(cardFactory.Create(CardRank.Four, CardSuit.Spades));

            int points = ScoreCalculator.GetMovePoints(columnDeck1, columnDeck2, 2, new GameSettings());
            Assert.That(points, Is.EqualTo(ScoreCalculator.TurnOverColumnCardPoints));
        }

        [Test]
        public void Test_Recycle_OneCardMode()
        {
            Deck sourceDeck = deckFactory.CreateReserveDeck();
            Deck destinationDeck = deckFactory.CreateStockDeck();

            int points = ScoreCalculator.GetMovePoints(sourceDeck, destinationDeck, 1, new GameSettings() { ThreeCardsMode = false });
            Assert.That(points, Is.EqualTo(ScoreCalculator.RecycleReserve1Points));
        }

        [Test]
        public void Test_Recycle_ThreeCardMode()
        {
            Deck sourceDeck = deckFactory.CreateReserveDeck();
            Deck destinationDeck = deckFactory.CreateStockDeck();

            int points = ScoreCalculator.GetMovePoints(sourceDeck, destinationDeck, 1, new GameSettings() { ThreeCardsMode = true });
            Assert.That(points, Is.EqualTo(ScoreCalculator.RecycleReserve3Points));
        }
    }
}

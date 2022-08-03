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

            int points = ScoreCalculator.GetMovePoints(ReserveDeck, foundationDeck);
            Assert.That(points, Is.EqualTo(ScoreCalculator.ReserveToFoundationPoints));
        }

        [Test]
        public void Test_Reserve_To_Column()
        {
            ReserveDeck ReserveDeck = deckFactory.CreateReserveDeck();
            ColumnDeck columnDeck = deckFactory.CreateColumnDeck();

            int points = ScoreCalculator.GetMovePoints(ReserveDeck, columnDeck);
            Assert.That(points, Is.EqualTo(ScoreCalculator.ReserveToColumnPoints));
        }

        [Test]
        public void Test_Column_To_Foundation()
        {
            ColumnDeck columnDeck = deckFactory.CreateColumnDeck();
            FoundationDeck foundationDeck = deckFactory.CreateFoundationDeck(CardSuit.Hearts);

            int points = ScoreCalculator.GetMovePoints(columnDeck, foundationDeck);
            Assert.That(points, Is.EqualTo(ScoreCalculator.ColumnToFoundationPoints));
        }

        [Test]
        public void Test_Foundation_To_Column()
        {
            FoundationDeck foundationDeck = deckFactory.CreateFoundationDeck(CardSuit.Hearts);
            ColumnDeck columnDeck = deckFactory.CreateColumnDeck();

            int points = ScoreCalculator.GetMovePoints(foundationDeck, columnDeck);
            Assert.That(points, Is.EqualTo(ScoreCalculator.FoundationToColumnPoints));
        }

        [Test]
        public void Test_Column_To_Column()
        {
            ColumnDeck columnDeck1 = deckFactory.CreateColumnDeck();
            ColumnDeck columnDeck2 = deckFactory.CreateColumnDeck();

            int points = ScoreCalculator.GetMovePoints(columnDeck1, columnDeck2);
            Assert.That(points, Is.EqualTo(0));
        }

        [Test]
        public void Test_TurnOverCard()
        {
            ColumnDeck columnDeck1 = deckFactory.CreateColumnDeck();
            ColumnDeck columnDeck2 = deckFactory.CreateColumnDeck();

            Card card = cardFactory.Create(CardRank.Queen, CardSuit.Clubs);
            columnDeck1.Push(card);
            card.Hide();
            columnDeck1.Push(cardFactory.Create(CardRank.Five, CardSuit.Hearts));

            int points = ScoreCalculator.GetMovePoints(columnDeck1, columnDeck2);
            Assert.That(points, Is.EqualTo(ScoreCalculator.TurnOverColumnCardPoints));

            points = ScoreCalculator.GetMovePoints(columnDeck2, columnDeck1);
            Assert.That(points, Is.EqualTo(0));
        }
    }
}

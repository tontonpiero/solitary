using NUnit.Framework;
using Solitary.Core;

namespace Solitary.Tests
{
    class UnitTestScoreCalculator
    {
        private IDeckFactory deckFactory = new Deck.Factory();

        [Test]
        public void Test_Waste_To_Foundation()
        {
            WasteDeck wasteDeck = deckFactory.CreateWasteDeck();
            FoundationDeck foundationDeck = deckFactory.CreateFoundationDeck(CardSuit.Hearts);

            int points = ScoreCalculator.GetMovePoints(wasteDeck, foundationDeck);
            Assert.That(points, Is.EqualTo(ScoreCalculator.WasteToFoundationPoints));
        }

        [Test]
        public void Test_Waste_To_Column()
        {
            WasteDeck wasteDeck = deckFactory.CreateWasteDeck();
            ColumnDeck columnDeck = deckFactory.CreateColumnDeck();

            int points = ScoreCalculator.GetMovePoints(wasteDeck, columnDeck);
            Assert.That(points, Is.EqualTo(ScoreCalculator.WasteToColumnPoints));
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
    }
}

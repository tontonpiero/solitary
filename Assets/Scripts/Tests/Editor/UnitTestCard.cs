using NUnit.Framework;
using Solitary.Core;

namespace Solitary.Tests
{
    class UnitTestCard
    {
        private Card.Factory testCardFactory = new Card.Factory();

        [Test]
        public void Test_Create_Card()
        {
            Card card = testCardFactory.Create(CardRank.Ten, CardSuit.Clubs);

            Assert.That(card, Is.Not.Null);
            Assert.That(card.Rank, Is.EqualTo(CardRank.Ten));
            Assert.That(card.Suit, Is.EqualTo(CardSuit.Clubs));
        }

        [Test]
        public void Test_Card_Value()
        {
            Card card = testCardFactory.Create(CardRank.Ten, CardSuit.Clubs);

            Assert.That(card.Value, Is.EqualTo((int)CardRank.Ten));
        }

        [Test]
        public void Test_Cards_Equals()
        {
            Card card1 = testCardFactory.Create(CardRank.Ten, CardSuit.Clubs);
            Card card2 = testCardFactory.Create(CardRank.Ten, CardSuit.Clubs);

            Assert.That(card1, Is.EqualTo(card2));
            Assert.That(card2, Is.EqualTo(card1));
        }

        [Test]
        public void Test_Cards_Not_Equals()
        {
            Card card1 = testCardFactory.Create(CardRank.Ten, CardSuit.Clubs);
            Card card2 = testCardFactory.Create(CardRank.Ten, CardSuit.Hearts);
            Card card3 = testCardFactory.Create(CardRank.King, CardSuit.Clubs);

            Assert.That(card1, Is.Not.EqualTo(card2));
            Assert.That(card2, Is.Not.EqualTo(card1));

            Assert.That(card1, Is.Not.EqualTo(card3));
            Assert.That(card3, Is.Not.EqualTo(card1));
        }
    }
}

using NUnit.Framework;
using Solitary.Core;
using System.Collections.Generic;
using System.Linq;

namespace Solitary.Tests
{
    class UnitTestDeck
    {
        private Card.Factory testCardFactory = new Card.Factory();

        [Test]
        public void Test_Get_Card()
        {
            StockDeck deck = new StockDeck(testCardFactory);

            deck.Fill();
            Assert.That(deck.GetCard(0), Is.EqualTo(deck.TopCard));
        }

        [Test]
        public void Test_Fill_StockDeck()
        {
            StockDeck deck = new StockDeck(testCardFactory);

            Assert.That(deck.Count, Is.EqualTo(0));
            deck.Fill();
            Assert.That(deck.Count, Is.EqualTo(52));
        }

        [Test]
        public void Test_Shuffle_StockDeck()
        {
            StockDeck deck = new StockDeck(testCardFactory);
            deck.Fill();
            Card previousTopCard = deck.TopCard;
            deck.Shuffle(0);

            Assert.That(deck.Count, Is.EqualTo(52));
            // To improve: it can fail 1 time out of 52 if shuffle set the same top card
            Assert.That(deck.TopCard, Is.Not.EqualTo(previousTopCard));
        }

        [Test]
        public void Test_Shuffle_EmptyStockDeck()
        {
            StockDeck deck = new StockDeck(testCardFactory);
            deck.Shuffle(0);

            Assert.That(deck.Count, Is.EqualTo(0));
        }

        [Test]
        public void Test_Push_To_StockDeck()
        {
            StockDeck deck = new StockDeck(testCardFactory);
            Card card = testCardFactory.Create(CardRank.King, CardSuit.Hearts);

            deck.Push(card);

            Assert.That(deck.Count, Is.EqualTo(1));
            Assert.That(deck.TopCard, Is.EqualTo(card));
        }

        [Test]
        public void Test_StockDeck_Dispatch_OnChangedEvent_AfterPush()
        {
            StockDeck deck = new StockDeck(testCardFactory);
            Card card = testCardFactory.Create(CardRank.King, CardSuit.Hearts);
            bool hasChanged = false;
            deck.OnCardsAdded += (deck, cards) => hasChanged = true;

            Assert.That(hasChanged, Is.False);
            deck.Push(card);
            Assert.That(hasChanged, Is.True);
        }

        [Test]
        public void Test_StockDeck_Dispatch_OnChangedEvent_AfterPushSeveral()
        {
            StockDeck deck = new StockDeck(testCardFactory);
            IEnumerable<Card> cards = new List<Card>()
            {
                testCardFactory.Create(CardRank.Ace, CardSuit.Hearts),
                testCardFactory.Create(CardRank.Two, CardSuit.Hearts)
            };
            bool hasChanged = false;
            deck.OnCardsAdded += (deck, cards) => hasChanged = true;

            Assert.That(hasChanged, Is.False);
            deck.Push(cards);
            Assert.That(hasChanged, Is.True);
        }

        [Test]
        public void Test_StockDeck_Dispatch_OnChangedEvent_AfterPickOne()
        {
            StockDeck deck = new StockDeck(testCardFactory);
            deck.Fill();
            bool hasChanged = false;
            deck.OnCardsRemoved += (deck, cards) => hasChanged = true;

            Assert.That(hasChanged, Is.False);
            deck.Pick();
            Assert.That(hasChanged, Is.True);
        }

        [Test]
        public void Test_StockDeck_Dispatch_OnChangedEvent_AfterPickSeveral()
        {
            StockDeck deck = new StockDeck(testCardFactory);
            deck.Fill();
            bool hasChanged = false;
            deck.OnCardsRemoved += (deck, cards) => hasChanged = true;

            Assert.That(hasChanged, Is.False);
            deck.Pick(10);
            Assert.That(hasChanged, Is.True);
        }

        [Test]
        public void Test_Pick_From_StockDeck()
        {
            StockDeck deck = new StockDeck(testCardFactory);
            deck.Fill();

            Assert.That(deck.Count, Is.EqualTo(52));

            deck.Pick(10);

            Assert.That(deck.Count, Is.EqualTo(42));

            Card previousTopCard = deck.TopCard;
            Card pickedCard = deck.Pick();

            Assert.That(deck.Count, Is.EqualTo(41));
            Assert.That(pickedCard, Is.EqualTo(previousTopCard));
        }

        [Test]
        public void Test_CanPush_To_FoundationDeck()
        {
            FoundationDeck deck = new FoundationDeck(CardSuit.Hearts);
            Card heartsAceCard = testCardFactory.Create(CardRank.Ace, CardSuit.Hearts);
            Card heartsTwoCard = testCardFactory.Create(CardRank.Two, CardSuit.Hearts);
            Card spadesAceCard = testCardFactory.Create(CardRank.Ace, CardSuit.Spades);
            Card nullCard = null;

            Assert.That(deck.CanPush(nullCard), Is.False);

            Assert.That(deck.CanPush(heartsAceCard), Is.True);
            Assert.That(deck.CanPush(heartsTwoCard), Is.False);
            Assert.That(deck.CanPush(spadesAceCard), Is.False);

            deck.Push(heartsAceCard);

            Assert.That(deck.CanPush(heartsAceCard), Is.False);
            Assert.That(deck.CanPush(heartsTwoCard), Is.True);
            Assert.That(deck.CanPush(spadesAceCard), Is.False);

            deck.Push(heartsTwoCard);

            Assert.That(deck.CanPush(heartsAceCard), Is.False);
            Assert.That(deck.CanPush(heartsTwoCard), Is.False);
            Assert.That(deck.CanPush(spadesAceCard), Is.False);
        }

        [Test]
        public void Test_CanPush_To_ColumnDeck()
        {
            ColumnDeck deck = new ColumnDeck();
            Card card1 = testCardFactory.Create(CardRank.King, CardSuit.Hearts);
            Card card2 = testCardFactory.Create(CardRank.Queen, CardSuit.Spades);
            Card card3 = testCardFactory.Create(CardRank.Jack, CardSuit.Diamonds);
            Card nullCard = null;

            Assert.That(deck.CanPush(nullCard), Is.False);

            Assert.That(deck.CanPush(card1), Is.True);
            Assert.That(deck.CanPush(card2), Is.False);
            Assert.That(deck.CanPush(card3), Is.False);

            deck.Push(card1);

            Assert.That(deck.CanPush(card1), Is.False);
            Assert.That(deck.CanPush(card2), Is.True);
            Assert.That(deck.CanPush(card3), Is.False);

            deck.Push(card2);

            Assert.That(deck.CanPush(card1), Is.False);
            Assert.That(deck.CanPush(card2), Is.False);
            Assert.That(deck.CanPush(card3), Is.True);

            deck.Push(card3);

            Assert.That(deck.CanPush(card1), Is.False);
            Assert.That(deck.CanPush(card2), Is.False);
            Assert.That(deck.CanPush(card3), Is.False);
        }

        [Test]
        public void Test_CanPush_To_ReserveDeck()
        {
            ReserveDeck deck = new ReserveDeck();
            Card card = testCardFactory.Create(CardRank.Ten, CardSuit.Hearts);

            Assert.That(deck.CanPush(card), Is.True);
        }
    }
}

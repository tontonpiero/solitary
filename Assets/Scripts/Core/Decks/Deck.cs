using System;
using System.Collections.Generic;
using System.Linq;

namespace Solitary.Core
{
    public delegate void CardsMovedEventHandler(Deck deck, IEnumerable<Card> cards);

    public abstract class Deck : IDisposable
    {
        public event CardsMovedEventHandler OnCardsAdded;
        public event CardsMovedEventHandler OnCardsRemoved;

        protected Stack<Card> cards = new Stack<Card>();

        public Card TopCard => IsEmpty ? null : cards.Peek();
        public bool IsEmpty => cards.Count == 0;
        public int Count => cards.Count;

        protected Deck() { }

        /// <summary>
        /// Get card at specified index from top (the card stays in the deck)
        /// </summary>
        public Card GetCard(int index) => cards.ElementAtOrDefault(index);

        /// <summary>
        /// Get the first N cards (the cards stays in the deck)
        /// </summary>
        public IEnumerable<Card> GetCards(int amount) => cards.Skip(Math.Max(0, cards.Count - amount));

        virtual public bool CanMoveCardTo(Deck destination, Card card)
        {
            if (destination == null || destination == this) return false;
            return destination.CanPush(card);
        }

        public Card Pick() => Pick(1).FirstOrDefault();

        /// <summary>
        /// Pick the first N cards (the cards are removed from the deck)
        /// </summary>
        public IEnumerable<Card> Pick(int amount)
        {
            if (amount > cards.Count) return null;
            IEnumerable<Card> pickedCards = cards.PopRange(amount);
            pickedCards = pickedCards.Reverse();
            OnChanged();
            OnCardsRemoved?.Invoke(this, pickedCards);
            return pickedCards;
        }

        virtual public bool CanPush(Card card) => false;

        public void Push(Card card)
        {
            Push(new List<Card>() { card });
        }

        public void Push(IEnumerable<Card> newCards)
        {
            cards.PushRange(newCards);
            OnChanged();
            OnCardsAdded?.Invoke(this, newCards);
        }

        virtual protected void OnChanged() { }

        public void Dispose()
        {
            OnCardsAdded = null;
            OnCardsRemoved = null;
        }

        public class Factory : IDeckFactory
        {
            public ColumnDeck CreateColumnDeck() => new ColumnDeck();

            public FoundationDeck CreateFoundationDeck(CardSuit suit) => new FoundationDeck(suit);

            public StockDeck CreateStockDeck() => new StockDeck(new Card.Factory());

            public WasteDeck CreateWasteDeck() => new WasteDeck();
        }
    }

}
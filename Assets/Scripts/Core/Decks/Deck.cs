using System;
using System.Collections.Generic;
using System.Linq;

namespace Solitary.Core
{

    public abstract class Deck
    {
        public event Action OnChanged;

        protected Stack<Card> cards = new Stack<Card>();

        public Card TopCard => IsEmpty ? null : cards.Peek();
        public bool IsEmpty => cards.Count == 0;
        public int Count => cards.Count;

        /// <summary>
        /// Get card at specified index (the card stays in the deck)
        /// </summary>
        public Card GetCard(int index) => cards.ElementAtOrDefault(index);

        /// <summary>
        /// Get the first N cards (the cards stays in the deck)
        /// </summary>
        public IEnumerable<Card> GetCards(int amount) => cards.Skip(Math.Max(0, cards.Count - amount));

        virtual public bool CanPick(int amount = 1) => cards.Count >= amount;

        public Card Pick() => Pick(1).FirstOrDefault();

        public IEnumerable<Card> Pick(int maxAmount)
        {
            if (cards.Count == 0) return null;
            maxAmount = Math.Min(maxAmount, cards.Count);
            IEnumerable<Card> pickedCards = cards.PopRange(maxAmount);
            OnChanged?.Invoke();
            return pickedCards;
        }

        virtual public bool CanPush(Card card) => false;

        virtual public bool CanPush(IEnumerable<Card> newCards) => false;

        public void Push(Card card)
        {
            cards.Push(card);
            OnChanged?.Invoke();
        }

        public void Push(IEnumerable<Card> newCards)
        {
            cards.PushRange(newCards);
            OnChanged?.Invoke();
        }
    }

}
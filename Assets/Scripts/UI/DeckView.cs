using Solitary.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Solitary.UI
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] protected bool revealTopCard = false;
        [SerializeField] protected Vector2 offsetIncrement = Vector2.zero;

        public Stack<CardView> CardViews { get; private set; } = new Stack<CardView>();
        public Deck Deck { get; set; }

        private Vector2 currentOffset = Vector2.zero;

        public void AddCardView(CardView cardView)
        {
            CardViews.Push(cardView);
            cardView.SetTarget(transform, currentOffset);
            CheckTopCard();

            currentOffset += offsetIncrement;
        }

        public void RemoveCardView()
        {
            CardViews.Pop();
            CheckTopCard();

            currentOffset -= offsetIncrement;
        }

        private void CheckTopCard()
        {
            if (revealTopCard && CardViews.Count > 0)
            {
                CardView cardView = CardViews.Peek();
                if (cardView.Card == Deck.TopCard)
                {
                    cardView.Reveal();
                }
            }
        }
    }
}

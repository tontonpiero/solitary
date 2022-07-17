using Solitary.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Solitary.UI
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] protected bool revealTopCard = false;
        [SerializeField] protected Vector2 revealedOffset = Vector2.zero;
        [SerializeField] protected Vector2 hiddenOffset = Vector2.zero;

        public Stack<CardView> CardViews { get; private set; } = new Stack<CardView>();
        public Deck Deck { get; set; }

        public void AddCardView(CardView cardView)
        {
            cardView.SetTarget(GetNextTargetTransform(), GetNextOffset());
            cardView.transform.SetAsLastSibling();
            CardViews.Push(cardView);
            CheckTopCard();
        }

        public void RemoveCardView()
        {
            CardViews.Pop();
            CheckTopCard();
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

        private Transform GetNextTargetTransform()
        {
            if( CardViews.Count == 0 ) return transform;
            return CardViews.Peek().transform;
        }

        private Vector2 GetNextOffset()
        {
            if( CardViews.Count == 0 ) return Vector2.zero;
            return CardViews.Peek().IsRevealed ? revealedOffset : hiddenOffset;
        }
    }
}

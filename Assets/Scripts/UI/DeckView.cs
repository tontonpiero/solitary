using Solitary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Solitary.UI
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] protected bool revealTopCard = false;
        [SerializeField] protected Vector2 revealedOffset = Vector2.zero;
        [SerializeField] protected Vector2 hiddenOffset = Vector2.zero;
        [SerializeField] protected Transform dropZone;

        public List<CardView> CardViews { get; private set; } = new List<CardView>();
        public Deck Deck { get; set; }

        public void AddCardView(CardView cardView)
        {
            cardView.SetDeckView(this);
            cardView.SetTarget(GetNextTargetTransform(), GetNextOffset());
            cardView.transform.SetAsLastSibling();
            CardViews.Add(cardView);
            CheckTopCard();
            UpdateDropZonePosition();
        }

        public void RemoveCardView(CardView cardView)
        {
            CardViews.Remove(cardView);
            CheckTopCard();
            UpdateDropZonePosition();
        }

        private void CheckTopCard()
        {
            if (revealTopCard && CardViews.Count > 0)
            {
                CardView cardView = CardViews.Last();
                if (cardView.Card == Deck.TopCard)
                {
                    cardView.Reveal();
                }
            }
        }

        private Transform GetNextTargetTransform()
        {
            if (CardViews.Count == 0) return transform;
            return CardViews.Last().transform;
        }

        private Vector2 GetNextOffset()
        {
            if (CardViews.Count == 0) return Vector2.zero;
            return CardViews.Last().IsRevealed ? revealedOffset : hiddenOffset;
        }

        private void UpdateDropZonePosition()
        {
            Vector2 position = Vector2.zero;
            foreach (CardView cardView in CardViews)
            {
                position += cardView.IsRevealed ? revealedOffset : hiddenOffset;
            }
            dropZone.localPosition = position;
        }

        public bool IsCardOverDropZone(CardView cardView)
        {
            if (dropZone == null || !dropZone.gameObject.activeSelf) return false;
            RectTransform rectTransform = cardView.GetComponent<RectTransform>();
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, dropZone.position);
        }

        public int GetCardIndexFromTop(CardView cardView)
        {
            return CardViews.Count - CardViews.IndexOf(cardView) - 1;
        }
    }
}

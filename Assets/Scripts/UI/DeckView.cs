using Solitary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Solitary.UI
{
    public class DeckView : MonoBehaviour, IPointerDownHandler
    {
        [Header("Deck special behaviours")]
        [SerializeField] protected DeckViewBehaviour behaviour = DeckViewBehaviour.None;
        [SerializeField] protected Transform dropZone;

        [Header("Cards Layout")]
        [SerializeField] protected Vector2 revealedOffset = Vector2.zero;
        [SerializeField] protected Vector2 hiddenOffset = Vector2.zero;

        public event Action<DeckView> OnDoubleClickDeck; 

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
            if (CardViews.Count == 0) return;
            CardView cardView = CardViews.Last();
            switch (behaviour)
            {
                case DeckViewBehaviour.AllCardsHidden:
                    cardView.Hide();
                    break;
                case DeckViewBehaviour.AllCardsRevealed:
                    cardView.Reveal();
                    break;
                case DeckViewBehaviour.TopCardRevealed:
                    if (cardView.Card == Deck.TopCard)
                    {
                        cardView.Reveal();
                    }
                    break;
            }
        }

        private Transform GetNextTargetTransform()
        {
            if (CardViews.Count == 0) return transform;
            return CardViews.Last().transform;
        }

        public IEnumerable<CardView> GetTopCards(int amount)
        {
            if (amount > CardViews.Count) return null;
            return CardViews.GetRange(CardViews.Count - amount, amount);
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

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDoubleClickDeck?.Invoke(this);
        }

        private void OnDestroy()
        {
            OnDoubleClickDeck = null;
        }
    }
}

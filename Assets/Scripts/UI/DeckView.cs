using Solitary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Solitary.UI
{
    public class DeckView : MonoBehaviour, IPointerDownHandler
    {
        [Header("Drop zone")]
        [SerializeField] protected Transform dropZone;

        [Header("Cards Layout")]
        [SerializeField] protected Vector2 revealedOffset = Vector2.zero;
        [SerializeField] protected Vector2 hiddenOffset = Vector2.zero;

        public event Action<DeckView> OnDoubleClickDeck;

        public List<CardView> CardViews { get; private set; } = new List<CardView>();
        public Deck Deck { get; set; }

        virtual public void AddCardView(CardView cardView)
        {
            cardView.SetDeckView(this);
            cardView.SetTarget(GetNextTargetTransform(), GetNextOffset());
            cardView.transform.SetAsLastSibling();
            CardViews.Add(cardView);
            UpdateDropZonePosition();
        }

        public void RemoveCardView(CardView cardView)
        {
            CardViews.Remove(cardView);
            UpdateDropZonePosition();
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
            if (CardViews.Count > 0)
            {
                dropZone.transform.SetParent(CardViews.Last().transform);
            }
            else
            {
                dropZone.transform.SetParent(transform);
            }
            dropZone.localPosition = Vector3.zero;
            dropZone.localScale = Vector3.one;
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

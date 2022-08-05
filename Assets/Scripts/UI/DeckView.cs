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
        [Tooltip("Number of cards affected by layout, the others will be placed at deckview position.\n0 means all cards are affected by layout")]
        [SerializeField] protected uint layoutCount = 0;
        [SerializeField] protected InteractionMode interactionMode = InteractionMode.AllRevealedCards;

        public event Action<DeckView> OnDoubleClickDeck;

        public List<CardView> CardViews { get; private set; } = new List<CardView>();
        public Deck Deck { get; set; }

        private Vector2 screenScale = new Vector2(Screen.width / 600f, Screen.height / 900f);

        virtual public void AddCardView(CardView cardView)
        {
            cardView.SetDeckView(this);

            if (layoutCount == 0)
            {
                cardView.SetTarget(GetNextTargetTransform(), GetNextOffset());
                cardView.transform.SetAsLastSibling();
            }

            CardViews.Add(cardView);

            if (layoutCount > 0)
            {
                UpdateAllCardsPosition();
            }

            UpdateInteractableCards();

            UpdateDropZonePosition();
        }

        private void UpdateInteractableCards()
        {
            for (int i = 0; i < CardViews.Count; i++)
            {
                CardView cardView = CardViews[i];

                if (interactionMode == InteractionMode.None)
                {
                    cardView.SetInteractable(false);
                }
                else if (interactionMode == InteractionMode.AllRevealedCards)
                {
                    cardView.SetInteractable(cardView.IsRevealed);
                }
                else if (interactionMode == InteractionMode.OnlyTopRevealedCard)
                {
                    cardView.SetInteractable(cardView.IsRevealed && i == CardViews.Count - 1);
                }
            }
        }

        public void RemoveCardView(CardView cardView)
        {
            CardViews.Remove(cardView);

            if (layoutCount > 0)
            {
                UpdateAllCardsPosition();
            }

            UpdateInteractableCards();

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
            Vector2 offset = CardViews.Last().IsRevealed ? revealedOffset : hiddenOffset;

            offset *= screenScale;

            return offset;
        }

        private void UpdateAllCardsPosition()
        {
            CardView previousCardView = null;
            for (int i = 0; i < CardViews.Count; i++)
            {
                CardView cardView = CardViews[i];
                if (i >= CardViews.Count - layoutCount)
                {
                    Vector2 offset = i > 0 && previousCardView.IsRevealed ? revealedOffset : hiddenOffset;
                    cardView.SetTarget(i > 0 ? previousCardView.transform : transform, offset * screenScale);
                    cardView.transform.SetAsLastSibling();
                }
                else
                {
                    cardView.SetTarget(i > 0 ? previousCardView.transform : transform, Vector2.zero);
                    cardView.transform.SetAsLastSibling();
                }
                previousCardView = cardView;
            }
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

        [Serializable]
        public enum InteractionMode
        {
            None,
            AllRevealedCards,
            OnlyTopRevealedCard
        }
    }
}

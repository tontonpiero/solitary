using Solitary.Core;
using Solitary.Data;
using Solitary.Utils;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Solitary.UI
{
    public class CardView : MonoBehaviour, IPointerClickHandler
    {
        [Header("Sprites")]
        [SerializeField] private CardSpriteData spriteData;
        [SerializeField] private Image rankImage;
        [SerializeField] private Image bigSuitImage;
        [SerializeField] private Image smallSuitImage;

        [Header("Colors")]
        [SerializeField] private Color redColor;
        [SerializeField] private Color blackColor;

        [Header("Faces")]
        [SerializeField] private GameObject frontFace;
        [SerializeField] private GameObject backFace;

        public bool IsRevealed { get; private set; } = true;
        public Card Card { get; private set; }
        public DeckView DeckView { get; private set; }

        public event Action<CardView> OnCardDragStarted;
        public event Action<CardView> OnCardDragComplete;
        public event Action<CardView> OnCardDoubleClicked;

        private Image image;
        private Transform target;
        private Vector2 offset;
        private Vector2 velocity;
        private IDragBehaviour dragBehaviour;

        private void Awake()
        {
            dragBehaviour = GetComponent<IDragBehaviour>();
            image = GetComponent<Image>();
            dragBehaviour.OnDragStarted += DragBehaviour_OnDragStarted;
            dragBehaviour.OnDragComplete += DragBehaviour_OnDragComplete;
        }

        private void Start()
        {
            Hide();
        }

        public void SetCard(Card card)
        {
            this.Card = card;
            DressUp();
        }

        public void SetDeckView(DeckView deckView)
        {
            DeckView = deckView;
        }

        public void SetTarget(Transform target, Vector2 offset)
        {
            this.target = target;
            this.offset = offset;
        }

        private void DressUp()
        {
            rankImage.sprite = spriteData.GetRankSprite(Card.Rank);
            rankImage.color = Card.Color == CardColor.Red ? redColor : blackColor;
            bigSuitImage.sprite = spriteData.GetSuitSprite(Card.Suit);
            smallSuitImage.sprite = spriteData.GetSuitSprite(Card.Suit);
        }

        public void Reveal()
        {
            if (IsRevealed) return;
            IsRevealed = true;
            frontFace.SetActive(true);
            backFace.SetActive(false);
            image.enabled = true;
            dragBehaviour.Enabled = IsDraggable();
        }

        public void Hide()
        {
            if (!IsRevealed) return;
            IsRevealed = false;
            frontFace.SetActive(false);
            backFace.SetActive(true);
            image.enabled = false;
            dragBehaviour.Enabled = IsDraggable();
        }

        private bool IsDraggable()
        {
            return IsRevealed;
        }

        private void DragBehaviour_OnDragStarted()
        {
            OnCardDragStarted?.Invoke(this);
        }

        private void DragBehaviour_OnDragComplete()
        {
            OnCardDragComplete?.Invoke(this);
        }

        private void Update()
        {
            UpdateMoveToTarget();
        }

        private void UpdateMoveToTarget()
        {
            Vector2 targetPosition;
            if (dragBehaviour.IsDragging)
            {
                targetPosition = dragBehaviour.DragPosition;
            }
            else
            {
                if (target == null) return;
                targetPosition = (Vector2)target.position + offset;
            }

            FollowTarget(targetPosition);
        }

        private void FollowTarget(Vector2 targetPosition)
        {
            transform.position = Vector2.SmoothDamp(transform.position, targetPosition, ref velocity, 0.02f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                OnCardDoubleClicked?.Invoke(this);
            }
        }

        private void OnDestroy()
        {
            OnCardDragStarted = null;
            OnCardDragComplete = null;
            OnCardDoubleClicked = null;
        }
    }
}

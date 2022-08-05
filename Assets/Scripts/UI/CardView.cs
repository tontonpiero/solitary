using Solitary.Core;
using Solitary.Data;
using Solitary.Utils;
using System;
using System.Collections;
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

        public bool IsRevealed => Card.IsRevealed;
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

        public void SetCard(Card card)
        {
            this.Card = card;
            card.OnVisibilityChanged += OnVisibilityChanged;
            DressUp();

            OnVisibilityChanged();
        }

        private void OnVisibilityChanged()
        {
            if (Card.IsRevealed) Reveal();
            else Hide();
        }

        public void SetInteractable(bool value)
        {
            dragBehaviour.Enabled = value;
        }

        public void SetDeckView(DeckView deckView)
        {
            DeckView = deckView;
            SetInteractable(IsRevealed);
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
            image.enabled = true;
            SetInteractable(true);
            StartCoroutine(RevealRoutine());
        }

        public IEnumerator RevealRoutine()
        {
            yield return ScaleTo(new Vector3(0f, 1f, 1f), 0.05f);
            frontFace.SetActive(true);
            backFace.SetActive(false);
            yield return ScaleTo(Vector3.one, 0.1f);
        }

        public void Hide()
        {
            image.enabled = false;
            SetInteractable(false);
            StartCoroutine(HideRoutine());
        }

        public IEnumerator HideRoutine()
        {
            yield return ScaleTo(new Vector3(0f, 1f, 1f), 0.05f);
            frontFace.SetActive(false);
            backFace.SetActive(true);
            yield return ScaleTo(Vector3.one, 0.1f);
        }

        private IEnumerator ScaleTo(Vector3 targetScale, float duration)
        {
            float timeleft = duration;
            Vector3 initialScale = transform.localScale;
            while (timeleft > 0f)
            {
                timeleft -= Time.deltaTime;
                transform.localScale = Vector3.Lerp(initialScale, targetScale, 1f - (timeleft / duration));
                yield return null;
            }
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

        private float lastTimeClick = 0f;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!dragBehaviour.Enabled) return;
            float currentTimeClick = eventData.clickTime;
            if (Mathf.Abs(currentTimeClick - lastTimeClick) < 0.5f)
            {
                OnCardDoubleClicked?.Invoke(this);
            }
            lastTimeClick = currentTimeClick;
        }

        private void OnDestroy()
        {
            OnCardDragStarted = null;
            OnCardDragComplete = null;
            OnCardDoubleClicked = null;
        }
    }
}

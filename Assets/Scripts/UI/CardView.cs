using Solitary.Core;
using Solitary.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Solitary.UI
{
    public class CardView : MonoBehaviour
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

        public bool IsRevealed { get; set; }
        public Card Card { get; private set; }

        private Transform target;
        private Vector2 offset;
        private Vector2 velocity;

        private void Start()
        {
            Hide();
        }

        public void SetCard(Card card)
        {
            this.Card = card;
            DressUp();
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
            frontFace.SetActive(true);
            backFace.SetActive(false);
        }

        public void Hide()
        {
            if (!IsRevealed) return;
            frontFace.SetActive(false);
            backFace.SetActive(true);
        }

        private void Update()
        {
            UpdateMoveToTarget();
        }

        private void UpdateMoveToTarget()
        {
            if (target == null) return;

            Vector2 targetPosition = (Vector2)target.position + offset;
            transform.position = Vector2.SmoothDamp(transform.position, targetPosition, ref velocity, 0.1f);
        }
    }
}

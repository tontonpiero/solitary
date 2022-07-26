using Solitary.Core;
using Solitary.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Audio;

namespace Solitary.Manager
{
    public class CardManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private DeckManager deckManager;

        [Header("Cards")]
        [SerializeField] private CardView cardViewPrefab;
        [SerializeField] private Transform cardsContainer;

        private Dictionary<Card, CardView> cardViewByCard = new Dictionary<Card, CardView>();
        private Queue<MoveCardData> moveCardQueue = new Queue<MoveCardData>();
        private const float moveInterval = 0.03f;
        private float nextMoveTimeleft = 0f;

        public CardView GetCardView(Card card) => cardViewByCard[card];

        public void CreateDeckCards(DeckView deckView, DeckView stockDeckView)
        {
            IEnumerable<Card> cards = deckView.Deck.GetCards(deckView.Deck.Count).Reverse();
            foreach (Card card in cards)
            {
                CardView cardView = Instantiate(cardViewPrefab, cardsContainer, false);
                cardView.SetCard(card);
                cardView.OnCardDragStarted += OnCardDragStarted;
                cardView.OnCardDragComplete += OnCardDragComplete;
                cardView.OnCardDoubleClicked += OnCardDoubleClicked;
                cardViewByCard.Add(card, cardView);

                cardView.transform.position = stockDeckView.transform.position;
                stockDeckView.AddCardView(cardView);

                if (deckView != stockDeckView)
                {
                    MoveCard(card, deckView);
                }
            }
        }

        private void OnCardDoubleClicked(CardView cardView)
        {
            deckManager.TryMove(cardView);
        }

        private void OnCardDragStarted(CardView cardView)
        {
            int index = cardView.DeckView.GetCardIndexFromTop(cardView);
            IEnumerable<CardView> draggedCards = cardView.DeckView.GetTopCards(index + 1);
            foreach (CardView draggedCard in draggedCards)
            {
                draggedCard.transform.SetAsLastSibling();
            }
        }

        private void OnCardDragComplete(CardView cardView)
        {
            if (!deckManager.TryDropCardView(cardView))
            {
                AudioManager.PlaySound("move_card_wrong");
            }
        }

        private void Update()
        {
            UpdateMoveCardQueue(Time.deltaTime);
        }

        private void UpdateMoveCardQueue(float deltaTime)
        {
            if (moveCardQueue.Count == 0) return;
            nextMoveTimeleft -= deltaTime;
            if (nextMoveTimeleft < 0f)
            {
                nextMoveTimeleft += moveInterval;
                MoveCardData data = moveCardQueue.Dequeue();
                data.CardView.DeckView.RemoveCardView(data.CardView);
                data.DeckView.AddCardView(data.CardView);
            }
        }

        public void MoveCard(Card card, DeckView deckView)
        {
            CardView cardView = GetCardView(card);
            cardView.SetTarget(null, Vector2.zero);
            moveCardQueue.Enqueue(new MoveCardData()
            {
                CardView = cardView,
                DeckView = deckView
            });
        }

        private struct MoveCardData
        {
            public CardView CardView { get; set; }
            public DeckView DeckView { get; set; }
        }
    }
}

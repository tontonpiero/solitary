using Solitary.Core;
using Solitary.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Solitary.Manager
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private CardView cardViewPrefab;
        [SerializeField] private Transform cardsContainer;

        private Dictionary<Card, CardView> cardViewByCard = new Dictionary<Card, CardView>();
        private Queue<MoveCardData> moveCardQueue = new Queue<MoveCardData>();
        private const float moveInterval = 0.03f;
        private float nextMoveTimeleft = 0f;

        public CardView GetCardView(Card card) => cardViewByCard[card];

        public void CreateDeckCards(DeckView deckView)
        {
            foreach (Card card in deckView.Deck.GetCards(deckView.Deck.Count).Reverse())
            {
                CardView cardView = Instantiate(cardViewPrefab, cardsContainer, false);
                cardView.transform.position = deckView.transform.position;
                cardView.SetCard(card);
                cardViewByCard.Add(card, cardView);
                deckView.AddCardView(cardView);
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
                data.DeckView.AddCardView(data.CardView);
            }
        }

        public void MoveCard(Card card, DeckView deckView)
        {
            CardView cardView = GetCardView(card);
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

using Solitary.Core;
using Solitary.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitary.Manager
{
    public class DeckManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CardManager cardManager;

        [SerializeField] private DeckView stockDeckView;
        [SerializeField] private DeckView wasteDeckView;
        [SerializeField] private DeckView[] foundationDeckViews;
        [SerializeField] private DeckView[] columnDeckViews;

        private Dictionary<Deck, DeckView> deckViewByDeck = new Dictionary<Deck, DeckView>();
        private Queue<MoveCardData> moveCardQueue = new Queue<MoveCardData>();
        private const float moveInterval = 0.05f;
        private float nextMoveTimeleft = 0f;

        public DeckView GetDeckView(Deck deck) => deckViewByDeck[deck];

        public void InitializeDecks(Game game)
        {
            SetupDeckView(stockDeckView, game.StockDeck);
            SetupDeckView(wasteDeckView, game.WasteDeck);
            for (int i = 0; i < Game.FoundationsCount; i++)
            {
                SetupDeckView(foundationDeckViews[i], game.FoundationDecks[i]);
            }
            for (int i = 0; i < Game.ColumnsCount; i++)
            {
                SetupDeckView(columnDeckViews[i], game.ColumnDecks[i]);
            }
        }

        private void SetupDeckView(DeckView deckView, Deck deck)
        {
            deckView.Deck = deck;
            deckViewByDeck[deck] = deckView;
            cardManager.CreateDeckCards(deckView);
            deckView.Deck.OnCardsAdded += OnCardsAddedToDeck;
            deckView.Deck.OnCardsRemoved += OnCardsRemovedFromDeck;
        }

        private void OnCardsAddedToDeck(Deck deck, IEnumerable<Card> cards)
        {
            DeckView deckView = deckViewByDeck[deck];
            foreach (Card card in cards)
            {
                CardView cardView = cardManager.GetCardView(card);
                moveCardQueue.Enqueue(new MoveCardData()
                {
                    CardView = cardView,
                    DeckView = deckView
                });
            }
        }

        private void OnCardsRemovedFromDeck(Deck deck, IEnumerable<Card> cards)
        {
            //DeckView deckView = deckViewByDeck[deck];
            //cardManager.UpdatePositions(cards);
        }

        private void Update()
        {
            UpdateMoveCardQueue(Time.deltaTime);
        }

        private void UpdateMoveCardQueue(float deltaTime)
        {
            if (moveCardQueue.Count == 0) return;
            nextMoveTimeleft -= deltaTime;
            if(nextMoveTimeleft < 0f)
            {
                nextMoveTimeleft += moveInterval;
                MoveCardData data = moveCardQueue.Dequeue();
                data.DeckView.AddCardView(data.CardView);
            }
        }

        private struct MoveCardData
        {
            public CardView CardView { get; set; }
            public DeckView DeckView { get; set; }
        }
    }
}

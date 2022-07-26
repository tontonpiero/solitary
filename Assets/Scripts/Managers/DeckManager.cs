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
        [Header("Managers")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private CardManager cardManager;

        [Header("Decks")]
        [SerializeField] private DeckView stockDeckView;
        [SerializeField] private DeckView ReserveDeckView;
        [SerializeField] private DeckView[] foundationDeckViews;
        [SerializeField] private DeckView[] columnDeckViews;

        private Dictionary<Deck, DeckView> deckViewByDeck = new Dictionary<Deck, DeckView>();

        public DeckView GetDeckView(Deck deck) => deckViewByDeck[deck];

        public void InitializeDecks(Game game)
        {
            SetupDeckView(stockDeckView, game.StockDeck, stockDeckView);
            SetupDeckView(ReserveDeckView, game.ReserveDeck, stockDeckView);
            for (int i = 0; i < Game.FoundationsCount; i++)
            {
                SetupDeckView(foundationDeckViews[i], game.FoundationDecks[i], stockDeckView);
            }
            for (int i = 0; i < Game.ColumnsCount; i++)
            {
                SetupDeckView(columnDeckViews[i], game.ColumnDecks[i], stockDeckView);
            }
        }

        private void SetupDeckView(DeckView deckView, Deck deck, DeckView fromDeckView)
        {
            deckView.Deck = deck;
            deckViewByDeck[deck] = deckView;
            cardManager.CreateDeckCards(deckView, fromDeckView);
            deckView.Deck.OnCardsAdded += OnCardsAddedToDeck;
            deckView.Deck.OnCardsRemoved += OnCardsRemovedFromDeck;
            deckView.OnDoubleClickDeck += OnDoubleClickDeck;
        }

        private void OnCardsAddedToDeck(Deck deck, IEnumerable<Card> cards)
        {
            DeckView deckView = deckViewByDeck[deck];
            foreach (Card card in cards)
            {
                cardManager.MoveCard(card, deckView);
            }
        }

        private void OnDoubleClickDeck(DeckView drawView)
        {
            if (drawView.Deck is StockDeck)
            {
                DrawNextReserve();
            }
        }

        private void DrawNextReserve()
        {
            int stockCount = stockDeckView.Deck.Count;
            int ReserveCount = ReserveDeckView.Deck.Count;
            if (stockCount > 0)
            {
                gameManager.Draw();
            }
            else
            {
                gameManager.Recycle();
            }
        }

        private void OnCardsRemovedFromDeck(Deck deck, IEnumerable<Card> cards)
        {
        }

        public bool TryDropCardView(CardView cardView)
        {
            int amount = cardView.DeckView.GetCardIndexFromTop(cardView) + 1;

            foreach (DeckView deckView in deckViewByDeck.Values)
            {
                if (deckView.IsCardOverDropZone(cardView))
                {
                    if (deckView.Deck.CanPush(cardView.Card))
                    {
                        gameManager.MoveCards(cardView.DeckView.Deck, deckView.Deck, amount);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TryMove(CardView cardView)
        {
            if (cardView.DeckView.Deck is ColumnDeck)
            {
                if (TryMoveToFoundation(cardView)) return true;
                if (TryMoveToColumn(cardView)) return true;
            }
            else
            {
                if (TryMoveToColumn(cardView)) return true;
                if (TryMoveToFoundation(cardView)) return true;
            }

            return false;
        }

        private bool TryMoveToFoundation(CardView cardView)
        {
            int cardIndex = cardView.DeckView.GetCardIndexFromTop(cardView);
            if (cardIndex > 0) return false;
            foreach (DeckView deckView in foundationDeckViews)
            {
                if (deckView.Deck.CanPush(cardView.Card))
                {
                    gameManager.MoveCards(cardView.DeckView.Deck, deckView.Deck, 1);
                    return true;
                }
            }
            return false;
        }

        private bool TryMoveToColumn(CardView cardView)
        {
            int cardIndex = cardView.DeckView.GetCardIndexFromTop(cardView);
            foreach (DeckView deckView in columnDeckViews)
            {
                if (deckView.Deck.CanPush(cardView.Card))
                {
                    gameManager.MoveCards(cardView.DeckView.Deck, deckView.Deck, cardIndex + 1);
                    return true;
                }
            }
            return false;
        }
    }
}

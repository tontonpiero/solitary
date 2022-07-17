using Solitary.Core;
using Solitary.UI;
using System;
using System.Collections.Generic;
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

        public CardView GetCardView(Card card) => cardViewByCard[card];

        public void CreateDeckCards(DeckView deckView)
        {
            foreach (Card card in deckView.Deck.GetCards(deckView.Deck.Count))
            {
                CardView cardView = Instantiate(cardViewPrefab, cardsContainer, false);
                cardView.transform.position = deckView.transform.position;
                cardView.SetCard(card);
                cardViewByCard.Add(card, cardView);
                deckView.AddCardView(cardView);
            }
        }


    }
}

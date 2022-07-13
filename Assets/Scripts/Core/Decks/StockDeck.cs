using System;
using System.Collections.Generic;
using System.Linq;

namespace Solitary.Core
{

    public class StockDeck : Deck
    {
        private ICardFactory cardFactory;

        public StockDeck(ICardFactory cardFactory)
        {
            this.cardFactory = cardFactory;
        }

        public override bool CanMoveCardsTo(Deck destination, int amount = 1)
        {
            if (!(destination is WasteDeck)) return false;
            return base.CanMoveCardsTo(destination, amount);
        }

        virtual public void Fill()
        {
            IEnumerable<CardSuit> suits = Enum.GetValues(typeof(CardSuit)).Cast<CardSuit>();
            IEnumerable<CardRank> ranks = Enum.GetValues(typeof(CardRank)).Cast<CardRank>();

            List<Card> cards = new List<Card>();
            foreach (CardSuit suit in suits)
            {
                foreach (CardRank rank in ranks)
                {
                    cards.Add(cardFactory.Create(rank, suit));
                }
            }
            Push(cards);
        }

        virtual public void Shuffle()
        {
            cards.Shuffle();
        }
    }

}
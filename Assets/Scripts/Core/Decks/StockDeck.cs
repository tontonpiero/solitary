using System;
using System.Collections.Generic;
using System.Linq;

namespace Solitary.Core
{

    public class StockDeck : Deck
    {
        private ICardFactory cardFactory;

        public StockDeck(ICardFactory cardFactory) : base(0)
        {
            this.cardFactory = cardFactory;
        }

        protected override void OnChanged()
        {
            foreach (Card card in cards)
            {
                card.Hide();
            }
        }

        public override bool CanMoveCardTo(Deck destination, Card card)
        {
            if (!(destination is ReserveDeck)) return false;
            return base.CanMoveCardTo(destination, card);
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

        virtual public void Shuffle(int seed)
        {
            Random rnd = new Random(seed);
            var values = cards.ToArray();
            cards.Clear();
            foreach (var value in values.OrderBy(x => rnd.Next()))
                cards.Push(value);
        }
    }

}
using System.Collections.Generic;
using System.Linq;

namespace Solitary.Core
{

    public class FoundationDeck : Deck
    {
        public CardSuit Suit { get; private set; }

        public FoundationDeck(CardSuit suit)
        {
            Suit = suit;
        }

        public override bool CanPush(Card card)
        {
            if (card == null) return false;
            if (card.Suit != Suit) return false;
            if (IsEmpty) return card.Rank == Card.MinRank;
            return TopCard.Value == card.Value - 1;
        }

        public override bool CanMoveCardTo(Deck destination, Card card)
        {
            if (!(destination is ColumnDeck)) return false;
            return base.CanMoveCardTo(destination, card);
        }
    }

}
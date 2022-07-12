using System.Collections.Generic;

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

        public override bool CanPush(IEnumerable<Card> newCards) => false;
    }

}
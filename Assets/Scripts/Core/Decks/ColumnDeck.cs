using System.Collections.Generic;

namespace Solitary.Core
{

    public class ColumnDeck : Deck
    {
        public override bool CanPush(Card card) => CanStack(TopCard, card);

        public override bool CanPush(IEnumerable<Card> newCards)
        {
            if (newCards == null) return false;
            Card previous = TopCard;
            foreach (Card next in newCards)
            {
                if (!CanStack(previous, next)) return false;
                previous = next;
            }
            return true;
        }

        private bool CanStack(Card previous, Card next)
        {
            if (next == null) return false;
            if (previous == null) return next.Rank == Card.MaxRank;
            return previous.Value == next.Value + 1 && previous.Color != next.Color;
        }
    }

}
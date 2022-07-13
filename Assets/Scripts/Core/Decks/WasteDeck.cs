using System.Collections.Generic;

namespace Solitary.Core
{

    public class WasteDeck : Deck
    {
        public override bool CanPush(Card card) => true;

        public override bool CanPush(IEnumerable<Card> newCards) => true;

        public override bool CanMoveCardsTo(Deck destination, int amount = 1)
        {
            if (destination is StockDeck) return false;
            return base.CanMoveCardsTo(destination, amount);
        }
    }

}

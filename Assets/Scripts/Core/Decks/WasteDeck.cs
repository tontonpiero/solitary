using System.Collections.Generic;

namespace Solitary.Core
{

    public class WasteDeck : Deck
    {
        public override bool CanPush(Card card) => true;

        public override bool CanMoveCardTo(Deck destination, Card card)
        {
            if (destination is StockDeck) return false;
            return base.CanMoveCardTo(destination, card);
        }
    }

}

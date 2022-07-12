using System.Collections.Generic;

namespace Solitary.Core
{

    public class WasteDeck : Deck
    {
        public override bool CanPush(Card card) => true;

        public override bool CanPush(IEnumerable<Card> newCards) => true;
    }

}

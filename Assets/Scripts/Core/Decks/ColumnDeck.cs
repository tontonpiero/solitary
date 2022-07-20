using System.Collections.Generic;
using System.Linq;

namespace Solitary.Core
{

    public class ColumnDeck : Deck
    {
        int previousCount = 0;

        protected override void OnChanged()
        {
            TopCard?.Reveal();

            int diffAmount = Count - previousCount;

            if (diffAmount > 0) // card wad added
            {
                // hack to know if we have to hide previous card in case of undo
                // this scenario could probably be managed better

                Card firstNewCard = GetCard(diffAmount - 1);
                Card previousCard = GetCard(diffAmount);

                if (previousCard != null)
                {
                    bool legitMove = CanStack(previousCard, firstNewCard);

                    if (!legitMove)
                    {
                        previousCard.Reveal();
                        previousCard.Hide();
                    }
                }
            }

            previousCount = Count;
        }

        public override bool CanPush(Card card) => CanStack(TopCard, card);

        public bool CanStack(Card previous, Card next)
        {
            if (next == null) return false;
            if (previous == null) return next.Rank == Card.MaxRank;
            return previous.Value == next.Value + 1 && previous.Color != next.Color;
        }

        public override bool CanMoveCardTo(Deck destination, Card card)
        {
            if (destination is StockDeck) return false;
            if (destination is ReserveDeck) return false;
            return base.CanMoveCardTo(destination, card);
        }
    }

}
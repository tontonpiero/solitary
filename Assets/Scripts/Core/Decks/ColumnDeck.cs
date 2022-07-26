namespace Solitary.Core
{

    public class ColumnDeck : Deck
    {
        public ColumnDeck(int index) : base(index) { }

        protected override void OnChanged()
        {
            TopCard?.Reveal();
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
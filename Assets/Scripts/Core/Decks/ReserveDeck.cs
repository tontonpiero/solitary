namespace Solitary.Core
{

    public class ReserveDeck : Deck
    {
        protected override void OnChanged()
        {
            foreach (Card card in cards)
            {
                card.Reveal();
            }
        }

        public override bool CanPush(Card card) => true;

        public override bool CanMoveCardTo(Deck destination, Card card)
        {
            if (destination is StockDeck) return false;
            return base.CanMoveCardTo(destination, card);
        }
    }

}

namespace Solitary.Core
{
    public class CardFactory : ICardFactory
    {
        public Card Create(CardRank rank, CardSuit suit) => new Card(rank, suit);
    }
}

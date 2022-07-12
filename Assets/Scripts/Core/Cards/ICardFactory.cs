namespace Solitary.Core
{
    public interface ICardFactory
    {
        Card Create(CardRank rank, CardSuit suit);
    }
}

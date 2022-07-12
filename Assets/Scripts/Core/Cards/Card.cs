namespace Solitary.Core
{

    public class Card
    {
        public const CardRank MinRank = CardRank.Ace;
        public const CardRank MaxRank = CardRank.King;
        public const int MinValue = (int)CardRank.Ace;
        public const int MaxValue = (int)CardRank.King;

        public Card(CardRank rank, CardSuit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public CardRank Rank { get; private set; }

        public CardSuit Suit { get; private set; }

        public int Value => (int)Rank;

        public CardColor Color => (Suit == CardSuit.Hearts || Suit == CardSuit.Diamonds) ? CardColor.Red : CardColor.Black;

        public static bool operator ==(Card lhs, Card rhs)
        {
            if (lhs is null || rhs is null) return lhs is null && rhs is null;
            return lhs.Rank == rhs.Rank && lhs.Suit == rhs.Suit;
        }

        public static bool operator !=(Card lhs, Card rhs)
        {
            if ((lhs is null && !(rhs is null)) || (!(lhs is null) && rhs is null)) return true;
            return lhs.Rank != rhs.Rank || lhs.Suit != rhs.Suit;
        }

        public override string ToString() => $"Card: {Rank} of {Suit}";
    }

}

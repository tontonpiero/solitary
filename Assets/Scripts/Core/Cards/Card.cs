using System;

namespace Solitary.Core
{

    public class Card
    {
        public const CardRank MinRank = CardRank.Ace;
        public const CardRank MaxRank = CardRank.King;
        public const int MinValue = (int)CardRank.Ace;
        public const int MaxValue = (int)CardRank.King;

        public event Action OnVisibilityChanged;

        private Card(CardRank rank, CardSuit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public bool IsRevealed { get; private set; } = false;

        public CardRank Rank { get; private set; }

        public CardSuit Suit { get; private set; }

        public int Value => (int)Rank;

        public CardColor Color => (Suit == CardSuit.Hearts || Suit == CardSuit.Diamonds) ? CardColor.Red : CardColor.Black;

        public void Reveal()
        {
            if (IsRevealed == false)
            {
                IsRevealed = true;
                OnVisibilityChanged?.Invoke();
            }
        }

        public void Hide()
        {
            if (IsRevealed == true)
            {
                IsRevealed = false;
                OnVisibilityChanged?.Invoke();
            }
        }

        public static bool operator ==(Card c1, Card c2)
        {
            if (c1 is null) return c2 is null;
            return c1.Equals(c2);
        }

        public static bool operator !=(Card c1, Card c2) => !(c1 == c2);

        public override int GetHashCode() => Rank.GetHashCode() ^ Suit.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Card card = obj as Card;
            return Rank == card.Rank && Suit == card.Suit;
        }

        public override string ToString() => $"Card: {Rank} of {Suit}";

        public class Factory : ICardFactory
        {
            public Card Create(CardRank rank, CardSuit suit) => new Card(rank, suit);
        }
    }

}

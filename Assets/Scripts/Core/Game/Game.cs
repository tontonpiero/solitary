using System.Collections.Generic;

namespace Solitary.Core
{

    public class Game
    {
        public const int ColumnsCount = 7;
        public const int FoundationsCount = 4;

        public GameState State { get; private set; } = GameState.NotStarted;

        // Decks
        public StockDeck StockDeck { get; private set; }
        public WasteDeck WasteDeck { get; private set; }
        public ColumnDeck[] ColumnDecks { get; private set; }
        public FoundationDeck[] FoundationDecks { get; private set; }

        public void Start()
        {
            if (State != GameState.NotStarted) return;

            InitializeDecks();

            State = GameState.Started;
        }

        private void InitializeDecks()
        {
            StockDeck = new StockDeck(new Card.Factory());
            StockDeck.Fill();
            StockDeck.Shuffle();

            WasteDeck = new WasteDeck();

            ColumnDecks = new ColumnDeck[ColumnsCount];
            for (int i = 0; i < ColumnDecks.Length; i++)
            {
                ColumnDecks[i] = new ColumnDeck();
                ColumnDecks[i].Push(StockDeck.Pick(i + 1));
            }

            FoundationDecks = new FoundationDeck[FoundationsCount];
            for (int i = 0; i < FoundationDecks.Length; i++)
            {
                FoundationDecks[i] = new FoundationDeck((CardSuit)i);
            }
        }

        public bool CanMoveCards(Deck source, Deck destination, int amount = 1)
        {
            if( source == null|| destination == null) return false;
            if( !source.CanPick(amount)) return false;
            IEnumerable<Card> cards = source.GetCards(amount);
            return destination.CanPush(cards);
        }

        public void MoveCards(Deck source, Deck destination, int amount = 1)
        {

        }
    }

}
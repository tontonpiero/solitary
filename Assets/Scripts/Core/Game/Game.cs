using System;
using System.Collections.Generic;

namespace Solitary.Core
{

    public class Game : IDisposable
    {
        public const int ColumnsCount = 7;
        public const int FoundationsCount = 4;

        public GameState State { get; private set; } = GameState.NotStarted;
        public int Moves { get; private set; } = 0;
        public int Score { get; private set; } = 0;

        // Decks
        public StockDeck StockDeck { get; private set; }
        public WasteDeck WasteDeck { get; private set; }
        public ColumnDeck[] ColumnDecks { get; private set; }
        public FoundationDeck[] FoundationDecks { get; private set; }

        public event Action OnScoreChanged;
        public event Action OnMovesChanged;

        private ICommandInvoker commandInvoker;
        private IDeckFactory deckFactory;
        private IMoveSolver moveSolver;

        private Game(ICommandInvoker commandInvoker, IDeckFactory deckFactory, IMoveSolver moveSolver)
        {
            this.commandInvoker = commandInvoker;
            this.deckFactory = deckFactory;
            this.moveSolver = moveSolver;
        }

        public void Start()
        {
            if (State != GameState.NotStarted) return;

            CreateDecks();

            State = GameState.Started;
        }

        private void CreateDecks()
        {
            StockDeck = deckFactory.CreateStockDeck();
            StockDeck.Fill();
            StockDeck.Shuffle();

            WasteDeck = deckFactory.CreateWasteDeck();

            ColumnDecks = new ColumnDeck[ColumnsCount];
            for (int i = 0; i < ColumnDecks.Length; i++)
            {
                ColumnDecks[i] = deckFactory.CreateColumnDeck();
            }

            FoundationDecks = new FoundationDeck[FoundationsCount];
            for (int i = 0; i < FoundationDecks.Length; i++)
            {
                FoundationDecks[i] = deckFactory.CreateFoundationDeck((CardSuit)i);
            }
        }

        public void InitializeColumns()
        {
            for (int i = 0; i < ColumnDecks.Length; i++)
            {
                ColumnDecks[i].Push(StockDeck.Pick(i + 1));
            }
        }

        public bool CanMoveCard(Deck source, Deck destination, Card card) => source != null && source.CanMoveCardTo(destination, card);

        public void MoveCards(Deck source, Deck destination, int amount = 1)
        {
            if (source == null || destination == null || amount < 1) return;

            ICommand command = new MoveCommand(this, source, destination, amount);
            commandInvoker.AddCommand(command);
            IncrementMoves();
        }

        public void UndoLastMove()
        {
            if (commandInvoker.Count > 0)
            {
                commandInvoker.UndoCommand();
                IncrementMoves();
            }
        }

        public void SetScore(int score)
        {
            if (score < 0) score = 0;
            Score = score;
            OnScoreChanged?.Invoke();
        }

        public void IncrementMoves()
        {
            Moves++;
            OnMovesChanged?.Invoke();
        }

        public void ResolveNextMove()
        {
            if (moveSolver.TrySolve(this, out Deck source, out Deck destination, out int amount))
            {
                MoveCards(source, destination, amount);
            }
        }

        public void Dispose()
        {
            OnMovesChanged = null;
            OnScoreChanged = null;
        }

        public class Builder
        {
            private ICommandInvoker commandInvoker;
            private IDeckFactory deckFactory;
            private IMoveSolver moveSolver;

            public Builder WithCommandInvoker(ICommandInvoker commandInvoker)
            {
                this.commandInvoker = commandInvoker;
                return this;
            }

            public Builder WithDeckFactory(IDeckFactory deckFactory)
            {
                this.deckFactory = deckFactory;
                return this;
            }

            public Builder WithMoveSolver(IMoveSolver moveSolver)
            {
                this.moveSolver = moveSolver;
                return this;
            }

            public Game Build()
            {
                return new Game(
                    commandInvoker ?? new CommandInvoker(),
                    deckFactory ?? new Deck.Factory(),
                    moveSolver ?? new MoveSolver()
                );
            }
        }
    }

}
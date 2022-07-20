using System;

namespace Solitary.Core
{

    public partial class Game : IDisposable
    {
        public const int ColumnsCount = 7;
        public const int FoundationsCount = 4;

        // Game State
        public GameState State { get; private set; } = GameState.NotStarted;
        public int Moves { get; private set; } = 0;
        public int Score { get; private set; } = 0;
        public float TotalTime { get; private set; } = 0f;
        public bool IsNew { get; private set; } = true;

        // Decks
        public StockDeck StockDeck { get; private set; }
        public WasteDeck WasteDeck { get; private set; }
        public ColumnDeck[] ColumnDecks { get; private set; }
        public FoundationDeck[] FoundationDecks { get; private set; }

        // Events
        public event Action OnScoreChanged;
        public event Action OnMovesChanged;
        public event Action<GameState> OnStateChanged;

        // Systems
        private readonly ICommandInvoker commandInvoker;
        private readonly IDeckFactory deckFactory;
        private readonly IMoveSolver moveSolver;
        private readonly IGameSaver gameSaver;

        private Game(ICommandInvoker commandInvoker, IDeckFactory deckFactory, IMoveSolver moveSolver, IGameSaver gameSaver)
        {
            this.commandInvoker = commandInvoker;
            this.deckFactory = deckFactory;
            this.moveSolver = moveSolver;
            this.gameSaver = gameSaver;
        }

        public void Start()
        {
            if (State != GameState.NotStarted) return;

            CreateDecks();

            if (gameSaver.HasData())
            {
                gameSaver.Load(this);
            }
            else
            {
                StockDeck.Fill();
                StockDeck.Shuffle();
            }

            SetState(GameState.Started);
        }

        private void SetState(GameState newState)
        {
            if (newState != State)
            {
                State = newState;
                OnStateChanged?.Invoke(newState);
            }
        }

        private void CreateDecks()
        {
            StockDeck = deckFactory.CreateStockDeck();

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

        public void Update(float deltaTime)
        {
            if (State != GameState.Started) return;

            TotalTime += deltaTime;
        }

        public void Pause()
        {
            if (State != GameState.Started) return;

            SetState(GameState.Paused);
        }

        public void Resume()
        {
            if (State != GameState.Paused) return;

            SetState(GameState.Started);
        }

        public bool CanMoveCard(Deck source, Deck destination, Card card) => source != null && source.CanMoveCardTo(destination, card);

        public void MoveCards(Deck source, Deck destination, int amount = 1)
        {
            if (State != GameState.Started) return;

            if (source == null || destination == null || amount < 1) return;

            ICommand command = new MoveCommand(this, source, destination, amount);
            commandInvoker.AddCommand(command);

            SetMoves(Moves + 1);

            gameSaver.Save(this);

            CheckGameOver();
        }

        private void CheckGameOver()
        {
            for (int i = 0; i < FoundationsCount; i++)
            {
                if (FoundationDecks[i].Count < 13) return;
            }
            SetState(GameState.Over);
        }

        public bool UndoLastMove()
        {
            if (State != GameState.Started) return false;

            if (commandInvoker.Count > 0)
            {
                commandInvoker.UndoCommand();

                SetMoves(Moves + 1);

                gameSaver.Save(this);

                return true;
            }
            return false;
        }

        public void SetScore(int score)
        {
            if (State != GameState.Started) return;

            if (score < 0) score = 0;
            Score = score;
            OnScoreChanged?.Invoke();
        }

        public void SetMoves(int moves)
        {
            if (State != GameState.Started) return;

            Moves = moves;
            OnMovesChanged?.Invoke();
        }

        public bool ResolveNextMove()
        {
            if (State != GameState.Started) return false;

            if (moveSolver.TrySolve(this, out Deck source, out Deck destination, out int amount))
            {
                MoveCards(source, destination, amount);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            OnMovesChanged = null;
            OnScoreChanged = null;
        }
    }

}
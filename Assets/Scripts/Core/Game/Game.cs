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
        public int Id { get; private set; }
        public IGameSettings Settings => settings;

        // Decks
        public StockDeck StockDeck { get; private set; }
        public ReserveDeck ReserveDeck { get; private set; }
        public ColumnDeck[] ColumnDecks { get; private set; }
        public FoundationDeck[] FoundationDecks { get; private set; }

        // Events
        public event Action OnScoreChanged;
        public event Action OnMovesChanged;
        public event Action<GameState> OnStateChanged;

        // Systems
        private readonly ICommandInvoker commandInvoker;
        private readonly IDeckFactory deckFactory;
        private readonly IGameSolver gameSolver;
        private readonly IGameSaver gameSaver;
        private readonly IGameSettings settings;

        private Game(ICommandInvoker commandInvoker, IDeckFactory deckFactory, IGameSolver gameSolver, IGameSaver gameSaver, IGameSettings settings)
        {
            this.commandInvoker = commandInvoker;
            this.deckFactory = deckFactory;
            this.gameSolver = gameSolver;
            this.gameSaver = gameSaver;
            this.settings = settings;
        }

        public void Start(int id = 0)
        {
            if (State != GameState.NotStarted) return;

            if (id == 0)
            {
                Random rnd = new Random();
                Id = rnd.Next(1, int.MaxValue);
            }
            else
            {
                Id = id;
            }

            SetState(GameState.Started);

            CreateDecks();

            if (gameSaver.HasData())
            {
                gameSaver.Load(this);
            }
            else
            {
                StockDeck.Fill();
                StockDeck.Shuffle(Id);
            }
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

            ReserveDeck = deckFactory.CreateReserveDeck();

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

        public void Deal()
        {
            for (int i = 0; i < ColumnDecks.Length; i++)
            {
                ColumnDecks[i].Push(StockDeck.Pick(i + 1));
            }
        }

        public void Update(float deltaTime)
        {
            if (State != GameState.Started) return;
            if (Moves == 0) return;

            TotalTime += deltaTime;
        }

        public void Pause()
        {
            if (State != GameState.Started) return;

            SetState(GameState.Paused);

            Save();
        }

        public void Resume()
        {
            if (State != GameState.Paused) return;

            SetState(GameState.Started);
        }

        public void Save()
        {
            if (State != GameState.Started && State != GameState.Paused) return;
            if (Moves == 0) return;

            gameSaver.Save(this);
        }

        public bool CanMoveCard(Deck source, Deck destination, Card card) => source != null && source.CanMoveCardTo(destination, card);

        public void MoveCards(Deck source, Deck destination, int amount = 1, bool reverse = false)
        {
            if (State != GameState.Started) return;

            if (source == null || destination == null || amount < 1) return;

            ICommand command = new MoveCommand(this, source, destination, amount, reverse);
            commandInvoker.AddCommand(command);

            SetMoves(Moves + 1);

            Save();

            CheckGameOver();
        }

        private void CheckGameOver()
        {
            for (int i = 0; i < FoundationsCount; i++)
            {
                if (FoundationDecks[i].Count < 13) return;
            }
            SetState(GameState.Over);

            gameSaver.ClearData();
        }

        public void Draw()
        {
            if (State != GameState.Started) return;

            int amount = settings.ThreeCardsMode ? 3 : 1;
            amount = Math.Min(StockDeck.Count, amount);

            MoveCards(StockDeck, ReserveDeck, amount, true);
        }

        public void Recycle()
        {
            if (State != GameState.Started) return;

            MoveCards(ReserveDeck, StockDeck, ReserveDeck.Count, true);
        }

        public bool UndoLastMove()
        {
            if (State != GameState.Started) return false;

            if (commandInvoker.Count > 0)
            {
                commandInvoker.UndoCommand();

                SetMoves(Moves + 1);

                Save();

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

            if (gameSolver.TrySolve(this, out Deck source, out Deck destination, out int amount))
            {
                if (source is StockDeck && destination is ReserveDeck)
                {
                    Draw();
                }
                else if (source is ReserveDeck && destination is StockDeck)
                {
                    Recycle();
                }
                else
                {
                    MoveCards(source, destination, amount);
                }
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            OnMovesChanged = null;
            OnScoreChanged = null;
            OnStateChanged = null;
        }
    }

}
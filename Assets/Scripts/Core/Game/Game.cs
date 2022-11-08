using System;
using System.Linq;

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
        private readonly IMoveCommandInvoker moveCommandInvoker;
        private readonly IDeckFactory deckFactory;
        private readonly IGameSolver gameSolver;
        private readonly IGameSaver gameSaver;
        private readonly IGameSettings settings;

        private Game(IMoveCommandInvoker moveCommandInvoker, IDeckFactory deckFactory, IGameSolver gameSolver, IGameSaver gameSaver, IGameSettings settings, Game originalGame)
        {
            this.moveCommandInvoker = moveCommandInvoker;
            this.deckFactory = deckFactory;
            this.gameSolver = gameSolver;
            this.gameSaver = gameSaver;
            this.settings = settings;

            this.moveCommandInvoker.Game = this;

            if (originalGame != null)
            {
                CloneFromGame(originalGame);
            }
        }

        private void CloneFromGame(Game originalGame)
        {
            SetState(GameState.Started);
            CreateDecks();

            StockDeck.Load(originalGame.StockDeck.Save());
            ReserveDeck.Load(originalGame.ReserveDeck.Save());
            for (int i = 0; i < ColumnsCount; i++)
            {
                ColumnDecks[i].Load(originalGame.ColumnDecks[i].Save());
            }
            for (int i = 0; i < FoundationsCount; i++)
            {
                FoundationDecks[i].Load(originalGame.FoundationDecks[i].Save());
            }
        }

        public void Start(int id = 0)
        {
            if (State != GameState.NotStarted) return;

            bool isReplay = id != 0;
            if (isReplay)
            {
                Id = id;
            }
            else
            {
                Random rnd = new Random();
                Id = rnd.Next(1, int.MaxValue);
            }

            SetState(GameState.Started);

            if (gameSaver.HasData())
            {
                CreateDecks();
                gameSaver.Load(this);
            }
            else
            {
                bool isReady = false;
                int tries = 0;
                while (!isReady)
                {
                    CreateDecks();
                    StockDeck.Fill();
                    StockDeck.Shuffle(Id);
                    Deal();
                    isReady = isReplay || tries >= 50 || !settings.EnsureSolvable || IsSolvable();
                    if (!isReady)
                    {
                        tries++;
                        Random rnd = new Random();
                        Id = rnd.Next(1, int.MaxValue);
                    }
                }
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
                ColumnDecks[i] = deckFactory.CreateColumnDeck(i);
            }

            FoundationDecks = new FoundationDeck[FoundationsCount];
            for (int i = 0; i < FoundationDecks.Length; i++)
            {
                FoundationDecks[i] = deckFactory.CreateFoundationDeck((CardSuit)i);
            }
        }

        private void Deal()
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

        public void MoveCards(Deck source, Deck destination, int amount = 1)
        {
            if (State != GameState.Started) return;

            if (source == null || destination == null || amount < 1) return;

            bool reverse = (source is StockDeck && destination is ReserveDeck) || (source is ReserveDeck && destination is StockDeck);

            IMoveCommand command = new MoveCommand(this, source, destination, amount, reverse);
            moveCommandInvoker.AddCommand(command);

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

            MoveCards(StockDeck, ReserveDeck, amount);
        }

        public void Recycle()
        {
            if (State != GameState.Started) return;

            MoveCards(ReserveDeck, StockDeck, ReserveDeck.Count);
        }

        public bool UndoLastMove()
        {
            if (State != GameState.Started) return false;

            if (moveCommandInvoker.Count > 0)
            {
                moveCommandInvoker.UndoCommand();

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

        public bool IsSolvable()
        {
            if (State != GameState.Started) return false;

            return gameSolver.IsSolvable(this);
        }

        public void Dispose()
        {
            OnMovesChanged = null;
            OnScoreChanged = null;
            OnStateChanged = null;
        }
    }

}
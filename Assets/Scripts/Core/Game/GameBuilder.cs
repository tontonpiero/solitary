namespace Solitary.Core
{
    public partial class Game
    {
        public class Builder
        {
            private IMoveCommandInvoker moveCommandInvoker;
            private IDeckFactory deckFactory;
            private IGameSolver gameSolver;
            private IGameSaver gameSaver;
            private IGameSettings settings;
            private Game originalGame;

            public Builder WithMoveCommandInvoker(IMoveCommandInvoker moveCommandInvoker)
            {
                this.moveCommandInvoker = moveCommandInvoker;
                return this;
            }

            public Builder WithDeckFactory(IDeckFactory deckFactory)
            {
                this.deckFactory = deckFactory;
                return this;
            }

            public Builder WithGameSolver(IGameSolver gameSolver)
            {
                this.gameSolver = gameSolver;
                return this;
            }

            public Builder WithGameSaver(IGameSaver gameSaver)
            {
                this.gameSaver = gameSaver;
                return this;
            }

            public Builder WithGameSettings(IGameSettings settings)
            {
                this.settings = settings;
                return this;
            }

            public Builder HasOriginalGame(Game originalGame)
            {
                this.originalGame = originalGame;
                return this;
            }

            public Game Build()
            {
                if (settings == null)
                {
                    settings = new GameSettings();
                    settings.Load();
                }

                return new Game(
                    moveCommandInvoker ?? new MoveCommandInvoker(),
                    deckFactory ?? new Deck.Factory(),
                    gameSolver ?? new GameSolver(),
                    gameSaver ?? new Game.Saver(),
                    settings,
                    originalGame
                );
            }
        }
    }
}

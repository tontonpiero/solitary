namespace Solitary.Core
{
    public partial class Game
    {
        public class Builder
        {
            private ICommandInvoker commandInvoker;
            private IDeckFactory deckFactory;
            private IGameSolver gameSolver;
            private IGameSaver gameSaver;

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

            public Game Build()
            {
                return new Game(
                    commandInvoker ?? new CommandInvoker(),
                    deckFactory ?? new Deck.Factory(),
                    gameSolver ?? new GameSolver(),
                    gameSaver ?? new Game.Saver()
                );
            }
        }
    }
}

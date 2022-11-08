using System.Collections.Generic;

namespace Solitary.Core
{
    public class GameSolver : IGameSolver
    {

        public bool TrySolve(Game game, out Deck source, out Deck destination, out int amount)
        {
            List<SolverMove> possibleMoves = GetPossibleMoves(game);
            if (possibleMoves.Count > 0)
            {
                source = possibleMoves[0].Source;
                destination = possibleMoves[0].Destination;
                amount = possibleMoves[0].Amount;
                return true;
            }

            amount = 0;
            source = null;
            destination = null;
            return false;
        }

        private List<SolverMove> GetPossibleMoves(Game game)
        {
            List<SolverMove> moves = new List<SolverMove>();

            // try columns to other columns
            for (int columnSourceIndex = 0; columnSourceIndex < Game.ColumnsCount; columnSourceIndex++)
            {
                ColumnDeck columnSourceDeck = game.ColumnDecks[columnSourceIndex];
                for (int columnDestIndex = 0; columnDestIndex < Game.ColumnsCount; columnDestIndex++)
                {
                    ColumnDeck columnDestDeck = game.ColumnDecks[columnDestIndex];
                    if (CanSolveColumn(columnSourceDeck, columnDestDeck, out int amount))
                    {
                        moves.Add(new SolverMove() { Source = columnSourceDeck, Destination = columnDestDeck, Amount = amount });
                    }
                }
            }

            // try columns to foundations
            for (int columnIndex = 0; columnIndex < Game.ColumnsCount; columnIndex++)
            {
                ColumnDeck columnDeck = game.ColumnDecks[columnIndex];
                for (int foundationIndex = 0; foundationIndex < 4; foundationIndex++)
                {
                    FoundationDeck foundationDeck = game.FoundationDecks[foundationIndex];
                    if (CanSolve(columnDeck, foundationDeck))
                    {
                        moves.Add(new SolverMove() { Source = columnDeck, Destination = foundationDeck, Amount = 1 });
                    }
                }
            }

            // try Reserve to columns
            for (int columnDestIndex = 0; columnDestIndex < Game.ColumnsCount; columnDestIndex++)
            {
                ColumnDeck columnDestDeck = game.ColumnDecks[columnDestIndex];
                if (CanSolve(game.ReserveDeck, columnDestDeck))
                {
                    moves.Add(new SolverMove() { Source = game.ReserveDeck, Destination = columnDestDeck, Amount = 1 });
                }
            }

            // try Reserve to foundations
            for (int foundationIndex = 0; foundationIndex < Game.FoundationsCount; foundationIndex++)
            {
                FoundationDeck foundationDeck = game.FoundationDecks[foundationIndex];
                if (CanSolve(game.ReserveDeck, foundationDeck))
                {
                    moves.Add(new SolverMove() { Source = game.ReserveDeck, Destination = foundationDeck, Amount = 1 });
                }
            }

            if (game.StockDeck.Count > 0)
            {
                moves.Add(new SolverMove() { Source = game.StockDeck, Destination = game.ReserveDeck, Amount = 1 });
            }
            else if (game.ReserveDeck.Count > 0)
            {
                moves.Add(new SolverMove() { Source = game.ReserveDeck, Destination = game.StockDeck, Amount = game.ReserveDeck.Count });
            }

            return moves;
        }

        private bool CanSolve(Deck source, Deck destination)
        {
            return destination.CanPush(source.TopCard);
        }

        private bool CanSolveColumn(ColumnDeck source, ColumnDeck destination, out int amount)
        {
            amount = 1;
            Card currentCard = source.TopCard;
            while (amount < source.Count)
            {
                Card nextCard = currentCard;
                currentCard = source.GetCard(amount);
                if (!source.CanStack(currentCard, nextCard) || !currentCard.IsRevealed)
                {
                    currentCard = nextCard;
                    break;
                }
                amount++;
            }
            if (destination.CanPush(currentCard))
            {
                Card previousCard = source.GetCard(amount);
                if (destination.Count == 0 && previousCard == null) return false;
                if (destination.Count == 0 && previousCard != null) return true;
                if (previousCard == null) return true;
                if (previousCard.IsRevealed)
                {
                    if (destination.TopCard.Color == previousCard.Color && destination.TopCard.Value == previousCard.Value) return false;
                }
                return true;
            }
            return false;
        }

        public bool IsSolvable(Game game)
        {
            Game gameClone = new Game.Builder()
                .WithGameSaver(new DummyGameSaver())
                .HasOriginalGame(game)
                .Build();

            solveIterations = 0;
            bool isSolvable = TrySolveRecursive(gameClone);

            UnityEngine.Debug.Log("isSolvable=" + isSolvable + " iterations=" + solveIterations);

            return isSolvable;
        }

        private const int maxIterations = 200;
        private int solveIterations = 0;
        private bool TrySolveRecursive(Game game)
        {
            List<SolverMove> possibleMoves = GetPossibleMoves(game);
            solveIterations++;
            if (game.State == GameState.Over) return true;
            if (solveIterations == maxIterations) return false;
            if (possibleMoves.Count == 0) return false;
            foreach (SolverMove move in possibleMoves)
            {
                game.MoveCards(move.Source, move.Destination, move.Amount);

                return TrySolveRecursive(game);
            }
            return false;
        }

        private class DummyGameSaver : IGameSaver
        {
            public void ClearData() { }
            public bool HasData() => false;
            public void Load(Game game) { }
            public void Save(Game game) { }
        }

        private struct SolverMove
        {
            public Deck Source;
            public Deck Destination;
            public int Amount;
        }
    }
}

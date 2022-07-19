namespace Solitary.Core
{
    public class MoveSolver : IMoveSolver
    {

        public bool TrySolve(Game game, out Deck source, out Deck destination, out int amount)
        {
            // try columns to other columns
            for (int columnSourceIndex = 0; columnSourceIndex < Game.ColumnsCount; columnSourceIndex++)
            {
                ColumnDeck columnSourceDeck = game.ColumnDecks[columnSourceIndex];
                for (int columnDestIndex = 0; columnDestIndex < Game.ColumnsCount; columnDestIndex++)
                {
                    ColumnDeck columnDestDeck = game.ColumnDecks[columnDestIndex];
                    if (CanSolveColumn(columnSourceDeck, columnDestDeck, out amount))
                    {
                        source = columnSourceDeck;
                        destination = columnDestDeck;
                        return true;
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
                        source = columnDeck;
                        destination = foundationDeck;
                        amount = 1;
                        return true;
                    }
                }
            }

            // try waste to columns
            for (int columnDestIndex = 0; columnDestIndex < Game.ColumnsCount; columnDestIndex++)
            {
                ColumnDeck columnDestDeck = game.ColumnDecks[columnDestIndex];
                if (CanSolve(game.WasteDeck, columnDestDeck))
                {
                    source = game.WasteDeck;
                    destination = columnDestDeck;
                    amount = 1;
                    return true;
                }
            }

            // try waste to foundations
            for (int foundationIndex = 0; foundationIndex < Game.FoundationsCount; foundationIndex++)
            {
                FoundationDeck foundationDeck = game.FoundationDecks[foundationIndex];
                if (CanSolve(game.WasteDeck, foundationDeck))
                {
                    source = game.WasteDeck;
                    destination = foundationDeck;
                    amount = 1;
                    return true;
                }
            }

            if (game.StockDeck.Count > 0)
            {
                source = game.StockDeck;
                destination = game.WasteDeck;
                amount = 1;
                return true;
            }
            else if (game.WasteDeck.Count > 0)
            {
                source = game.WasteDeck;
                destination = game.StockDeck;
                amount = game.WasteDeck.Count;
                return true;
            }

            amount = 0;
            source = null;
            destination = null;
            return false;
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
                if (!source.CanStack(currentCard, nextCard))
                {
                    currentCard = nextCard;
                    break;
                }
                amount++;
            }
            if (destination.CanPush(currentCard))
            {
                Card previousCard = source.GetCard(amount + 1);
                if (destination.Count == 0 && previousCard == null) return false;
                if (destination.Count == 0 && previousCard != null) return true;
                if (previousCard == null) return true;
                if (destination.TopCard.Color == previousCard.Color && destination.TopCard.Value == previousCard.Value) return false;
                return true;
            }
            return false;
        }
    }
}

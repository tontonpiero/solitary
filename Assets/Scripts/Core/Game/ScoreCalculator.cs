namespace Solitary.Core
{
    public class ScoreCalculator
    {
        public const int WasteToFoundationPoints = 10;
        public const int WasteToColumnPoints = 5;
        public const int ColumnToFoundationPoints = 10;
        public const int FoundationToColumnPoints = -15;
        public const int TurnOverColumnCardPoints = 5;
        public const int RecycleWaste1Points = -100;
        public const int RecycleWaste3Points = -20;

        static public int GetMovePoints(Deck source, Deck destination)
        {
            int points = 0;

            if (source is WasteDeck)
            {
                if (destination is FoundationDeck) points = WasteToFoundationPoints;
                else if (destination is ColumnDeck) points = WasteToColumnPoints;
                else if (destination is StockDeck) points = RecycleWaste1Points;
            }
            else if (source is ColumnDeck)
            {
                if (destination is FoundationDeck) points = ColumnToFoundationPoints;
            }
            else if (source is FoundationDeck)
            {
                if (destination is ColumnDeck) points = FoundationToColumnPoints;
            }

            return points;
        }
    }
}

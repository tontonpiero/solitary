namespace Solitary.Core
{
    public class ScoreCalculator
    {
        public const int ReserveToFoundationPoints = 10;
        public const int ReserveToColumnPoints = 5;
        public const int ColumnToFoundationPoints = 10;
        public const int FoundationToColumnPoints = -15;
        public const int TurnOverColumnCardPoints = 5;
        public const int RecycleReserve1Points = -100;
        public const int RecycleReserve3Points = -20;

        static public int GetMovePoints(Deck source, Deck destination)
        {
            int points = 0;

            if (source is ReserveDeck)
            {
                if (destination is FoundationDeck) points = ReserveToFoundationPoints;
                else if (destination is ColumnDeck) points = ReserveToColumnPoints;
                else if (destination is StockDeck) points = RecycleReserve1Points;
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

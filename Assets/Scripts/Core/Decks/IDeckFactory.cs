namespace Solitary.Core
{
    public interface IDeckFactory
    {
        StockDeck CreateStockDeck();
        ReserveDeck CreateReserveDeck();
        ColumnDeck CreateColumnDeck();
        FoundationDeck CreateFoundationDeck(CardSuit suit);
    }
}
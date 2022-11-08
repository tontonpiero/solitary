namespace Solitary.Core
{
    public interface IDeckFactory
    {
        StockDeck CreateStockDeck();
        ReserveDeck CreateReserveDeck();
        ColumnDeck CreateColumnDeck(int index);
        FoundationDeck CreateFoundationDeck(CardSuit suit);
    }
}
namespace Solitary.Core
{
    public interface IDeckFactory
    {
        StockDeck CreateStockDeck();
        WasteDeck CreateWasteDeck();
        ColumnDeck CreateColumnDeck();
        FoundationDeck CreateFoundationDeck(CardSuit suit);
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Solitary.Core
{
    public class MoveCommand : IMoveCommand
    {
        private Deck source;
        private Deck destination;
        private int amount;
        private Game game;
        private int previousScore;
        private bool reverse;
        private bool cardWasRevealed;

        public MoveCommand(Game game, Deck source, Deck destination, int amount, bool reverse)
        {
            this.source = source;
            this.destination = destination;
            this.amount = amount;
            this.game = game;
            this.reverse = reverse;
        }

        public void Execute()
        {
            int points = ScoreCalculator.GetMovePoints(source, destination, amount, game.Settings);

            cardWasRevealed = false;
            if (source is ColumnDeck && source.Count > amount && source.GetCard(amount).IsRevealed == false)
            {
                cardWasRevealed = true;
            }

            IEnumerable<Card> cards = source.Pick(amount);
            if (reverse) cards = cards.Reverse();
            destination.Push(cards);
            previousScore = game.Score;

            game.SetScore(game.Score + points);
        }

        public void Undo()
        {
            IEnumerable<Card> cards = destination.Pick(amount);
            if (reverse) cards = cards.Reverse();
            source.Push(cards);
            game.SetScore(previousScore);

            if (cardWasRevealed) source.GetCard(amount).Hide();
        }

        public MoveCommandData Save()
        {
            MoveCommandData data = new MoveCommandData()
            {
                Amount = amount,
                CarRev = cardWasRevealed,
                Reverse = reverse,
                PrevScore = previousScore,
                Src = source.GetType().Name,
                SrcIdx = source.Index,
                Dest = destination.GetType().Name,
                DestIdx = destination.Index
            };
            return data;
        }

        public void Load(MoveCommandData data)
        {
            amount = data.Amount;
            cardWasRevealed = data.CarRev;
            reverse = data.Reverse;
            previousScore = data.PrevScore;
            source = GetDeck(data.Src, data.SrcIdx);
            destination = GetDeck(data.Dest, data.DestIdx);
        }

        private Deck GetDeck(string typeName, int index)
        {
            if (typeName == game.StockDeck.GetType().Name) return game.StockDeck;
            if (typeName == game.ReserveDeck.GetType().Name) return game.ReserveDeck;
            if (typeName == game.ColumnDecks[0].GetType().Name) return game.ColumnDecks[index];
            if (typeName == game.FoundationDecks[0].GetType().Name) return game.FoundationDecks[index];
            return null;
        }
    }
}

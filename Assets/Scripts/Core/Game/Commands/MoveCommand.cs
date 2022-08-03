using System.Collections.Generic;
using System.Linq;

namespace Solitary.Core
{
    public class MoveCommand : ICommand
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
            int points = ScoreCalculator.GetMovePoints(source, destination);

            cardWasRevealed = false;
            if (source is ColumnDeck && source.Count > 1 && source.GetCard(1).IsVisible == false)
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

            if (cardWasRevealed) source.GetCard(1).Hide();
        }
    }
}

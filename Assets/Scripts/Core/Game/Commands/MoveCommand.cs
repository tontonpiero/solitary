using System.Collections.Generic;

namespace Solitary.Core
{
    public class MoveCommand : ICommand
    {
        private Deck source;
        private Deck destination;
        private int amount;
        private Game game;
        private int previousScore;

        public MoveCommand(Game game, Deck source, Deck destination, int amount)
        {
            this.source = source;
            this.destination = destination;
            this.amount = amount;
            this.game = game;
        }

        public void Execute()
        {
            IEnumerable<Card> cards = source.Pick(amount);
            destination.Push(cards);
            previousScore = game.Score;

            int points = ScoreCalculator.GetMovePoints(source, destination);
            game.SetScore(game.Score + points);
        }

        public void Undo()
        {
            IEnumerable<Card> cards = destination.Pick(amount);
            source.Push(cards);
            game.SetScore(previousScore);
        }
    }
}

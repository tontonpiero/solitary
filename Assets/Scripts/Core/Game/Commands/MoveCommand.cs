using System.Collections.Generic;

namespace Solitary.Core
{
    public class MoveCommand : ICommand
    {
        private Deck source;
        private Deck destination;
        private int amount;

        public MoveCommand(Deck source, Deck destination, int amount = 1)
        {
            this.source = source;
            this.destination = destination;
            this.amount = amount;
        }

        public void Execute()
        {
            IEnumerable<Card> cards = source.Pick(amount);
            destination.Push(cards);
        }

        public void Undo()
        {
            IEnumerable<Card> cards = destination.Pick(amount);
            source.Push(cards);
        }
    }
}

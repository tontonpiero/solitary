namespace Solitary.Core
{
    public interface IMoveSolver
    {
        bool TrySolve(Game game, out Deck source, out Deck destination, out int amount);
    }
}
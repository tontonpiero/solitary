namespace Solitary.Core
{
    public interface IGameSolver
    {
        bool TrySolve(Game game, out Deck source, out Deck destination, out int amount);
        bool IsSolvable(Game game);
    }
}
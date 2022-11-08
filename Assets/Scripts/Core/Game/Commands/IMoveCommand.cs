namespace Solitary.Core
{
    public interface IMoveCommand : ISavable<MoveCommandData>
    {
        void Execute();
        void Undo();
    }
}

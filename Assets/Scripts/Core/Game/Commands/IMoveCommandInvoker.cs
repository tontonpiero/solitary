namespace Solitary.Core
{
    public interface IMoveCommandInvoker : ISavable<MoveCommandData[]>
    {
        int Count { get; }
        Game Game { get; set; }

        void AddCommand(IMoveCommand command);
        void UndoCommand();
    }
}
namespace Solitary.Core
{
    public interface ICommandInvoker
    {
        int Count { get; }
        void AddCommand(ICommand command);
        void UndoCommand();
    }
}
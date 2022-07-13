namespace Solitary.Core
{
    public interface ICommandInvoker
    {
        void AddCommand(ICommand command);
        void UndoCommand();
    }
}
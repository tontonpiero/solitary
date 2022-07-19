using System.Collections.Generic;

namespace Solitary.Core
{
    public class CommandInvoker : ICommandInvoker
    {
        public int Count => commands.Count;

        protected Stack<ICommand> commands;

        public CommandInvoker()
        {
            commands = new Stack<ICommand>();
        }

        public void AddCommand(ICommand command)
        {
            command.Execute();
            commands.Push(command);
        }

        public void UndoCommand()
        {
            if (commands.Count > 0)
            {
                ICommand command = commands.Pop();
                command.Undo();
            }
        }
    }
}

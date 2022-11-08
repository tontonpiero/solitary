using System.Collections.Generic;

namespace Solitary.Core
{
    public class MoveCommandInvoker : IMoveCommandInvoker
    {
        public int Count => commands.Count;

        public Game Game { get; set; }

        protected Stack<IMoveCommand> commands;

        public MoveCommandInvoker()
        {
            commands = new Stack<IMoveCommand>();
        }

        public void AddCommand(IMoveCommand command)
        {
            command.Execute();
            commands.Push(command);
        }

        public void UndoCommand()
        {
            if (commands.Count > 0)
            {
                IMoveCommand command = commands.Pop();
                command.Undo();
            }
        }

        public MoveCommandData[] Save()
        {
            MoveCommandData[] data = new MoveCommandData[commands.Count];
            int i = commands.Count - 1;
            foreach (IMoveCommand command in commands)
            {
                data[i] = command.Save();
                i--;
            }

            return data;
        }

        public void Load(MoveCommandData[] data)
        {
            foreach (MoveCommandData commandData in data)
            {
                MoveCommand command = new MoveCommand(Game, null, null, 0, false);
                command.Load(commandData);
                commands.Push(command);
            }
        }
    }
}

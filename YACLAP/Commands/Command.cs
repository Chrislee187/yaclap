namespace YACLAP.Commands
{
    public abstract class Command
    {
        protected Command(string commandName)
        {
            CommandName = commandName;
        }

        protected string CommandName { get; }

        public abstract void Execute();
    }
}
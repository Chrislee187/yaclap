namespace YACLAP.Commands
{
    public abstract class Command
    {
        protected Command(string name)
        {
            Name = name;
        }
        public string Name { get; }

        public abstract void Execute();
    }
}
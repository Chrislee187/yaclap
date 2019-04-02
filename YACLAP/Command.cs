namespace YACLAP
{
    public class Command
    {
        public string Name { get; protected set; }

        public Command SubCommand { get; private set; }

        public Command(SimpleParser parsed)
        {
            
        }
    }
}
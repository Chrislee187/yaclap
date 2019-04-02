using System;

namespace YACLAP
{
    public class ParserBuilder
    {
        public OptionBuilder NoCommands { get; private set; }

        public ParserBuilder AddCommand(string command, Action<CommandBuilder> p1)
        {
            throw new NotImplementedException();
        }

    }
}
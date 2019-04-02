using System;

namespace YACLAP
{
    public class CommandBuilder
    {
        public void AddSubCommand(string subcmd1, Action<OptionBuilder> p1)
        {
            throw new NotImplementedException();
        }

        public OptionBuilder NoSubCommand { get; set; }
    }
}
namespace YACLAP
{
    public class ReflectionParser
    {
        public string Command { get; private set; }
        public string SubCommand { get; private set; }
        public virtual object Data { get; set; }

        public ReflectionParser(string command, string subCommand, object data)
        {
            Command = command;
            SubCommand = subCommand;
            Data = data;
        }
    }

    public class ReflectionParser<T> : ReflectionParser
    {
        public ReflectionParser(string command, string subCommand, object data) : base(command, subCommand, data)
        {
        }

        public new T Data { get; set; }
    }
}
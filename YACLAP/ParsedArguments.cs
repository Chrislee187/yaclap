using System.Collections.Generic;
using System.Linq;
using YACLAP.Extensions;

namespace YACLAP
{
    public class ParsedArguments
    {
        private IDictionary<string, string> _options = new Dictionary<string, string>();

        public ParsedArguments(string[] args)
        {
            if (!args.Any()) return;
            var argQueue = new Queue<string>(args);

            CheckForCommand(argQueue);
            CheckForSubCommand(argQueue);
            CheckForFlagsAndOptions(argQueue);
        }
        public string Command { get; private set; } = "";
        public bool HasCommand => Command.Any();
        public string SubCommand { get; private set; } = "";
        public bool HasSubCommand => SubCommand.Any();
        public bool Error { get; private set; }

        public bool Flags(string flag)
        {
            return _options.ContainsKey(flag) && _options[flag].ToBool();
        }

        public bool HasOption(string option)
        {
            return _options.ContainsKey(option) && !_options[option].ToBool();
        }

        public string Option(string option)
        {
            if (!HasOption(option)) return null;

            return _options[option];
        }

        private void CheckForFlagsAndOptions(Queue<string> argQueue)
        {
            while (argQueue.Any())
            {
                if (NextTokenIsOption(argQueue))
                {
                    var name = argQueue.Dequeue().Remove(0, 2);
                    var value = "true";
                    if (argQueue.Any())
                    {
                        if (!NextTokenIsOption(argQueue))
                        {
                            value = argQueue.Dequeue();
                        }
                    }
                    _options.Add(name, value);
                }
            }
        }

        private void CheckForSubCommand(Queue<string> argQueue)
        {
            if (NextTokenIsCommand(argQueue))
            {
                SubCommand = argQueue.Dequeue();
            }
        }

        private void CheckForCommand(Queue<string> argQueue)
        {
            if (NextTokenIsCommand(argQueue))
            {
                Command = argQueue.Dequeue();
            }
        }

        private bool NextTokenIsCommand(Queue<string> queue) => queue.Any() && !IsOption(queue.Peek());
        private bool NextTokenIsOption(Queue<string> queue) => queue.Any() && IsOption(queue.Peek());

        private bool IsOption(string arg) => arg.Any() && arg.StartsWith("--");

    }

}

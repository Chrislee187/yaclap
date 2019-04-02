using System.Collections.Generic;
using System.Linq;
using YACLAP.Extensions;

namespace YACLAP
{
    public interface ISimpleParser
    {
        string Command { get; }
        bool HasCommand { get; }
        string SubCommand { get; }
        bool HasSubCommand { get; }
        bool Flags(string flag);
        bool HasOption(string option);
        string Option(string option);
    }

    public class SimpleParser : ISimpleParser
    {
        public static ISimpleParser CreateParser(string[] args) => new SimpleParser(args);

        private readonly IDictionary<string, string> _options = new Dictionary<string, string>();

        public SimpleParser(string[] args)
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

        public bool Flags(string flag)
        {
            var lower = flag.ToLower();
            return _options.ContainsKey(lower) && _options[lower].ToBool();
        }

        public bool HasOption(string option)
        {
            var lower = option.ToLower();
            return _options.ContainsKey(lower) && !_options[lower].ToBool();
        }

        public string Option(string option) => HasOption(option) ? _options[option.ToLower()] : null;

        private void CheckForFlagsAndOptions(Queue<string> argQueue)
        {
            while (argQueue.Any())
            {
                if (NextTokenIsOption(argQueue))
                {
                    var option = argQueue.Dequeue().Remove(0, 2).ToLower();
                    var value = "true";
                    if (argQueue.Any())
                    {
                        if (!NextTokenIsOption(argQueue))
                        {
                            value = argQueue.Dequeue();
                        }
                    }
                    _options.Add(option, value);
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

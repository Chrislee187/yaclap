using YACLAP.Commands;

namespace YACLAP
{
    public static class Yaclap
    {
        public static ISimpleParser Parse(string[] args) 
            => new SimpleParser(args);

        public static Command Parse(string[] args, params ICommandMapper[] commands) 
            => CommandParser.Parse(args, commands);
    }
}
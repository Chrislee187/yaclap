using System;

namespace YACLAP.Commands
{
    public class CommandMapper<T> : ICommandMapper where T:class
    {
        public string CommandName { get; }
        public Type CommandType { get; }
        public Action<string[], T> Mapper { get; }

        public CommandMapper(string commandName, Action<string[], T> argumentMapper)
        {
            Mapper = argumentMapper;
            CommandName = commandName;
            CommandType = typeof(T);
        }

        public void MapArguments(string[] arguments, object command)
        {
            Mapper(arguments, command as T);
        }

    }
}
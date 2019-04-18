using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YACLAP.Commands
{
    public class CommandParser
    {
        static readonly Func<Type, Command> DefaultResolver = type => ((Command) Activator.CreateInstance(type));
        public static Command Parse(string[] args, IList<ICommandMapper> commands, Func<Type, Command> commandTypeResolver = null)
        {
            var resolver = commandTypeResolver ?? DefaultResolver;

            var parser = new SimpleParser(args, true);
            var cmd = parser.Arguments[0].ToLower();
            var mapper = commands.FirstOrDefault(commandMapper => commandMapper.CommandName.ToLower() == cmd);
            if (mapper == null)
            {
                throw new ArgumentException($"No CommandMapper found for command: '{cmd}'.", nameof(args));
            }
            var commandType = mapper.CommandType;

            var command = resolver(commandType);

            mapper.MapArguments(parser.Arguments, command);
            parser.SetOptionAndFlagValues(command);
            Validator.ValidateObject(command, new ValidationContext(command));
            return command;
        }

    }
}
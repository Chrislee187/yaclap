using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YACLAP.Commands
{
    public class CommandParser
    {
        public static Command Parse(string[] args, IList<ICommandMapper> commands)
        {
            var parser = new SimpleParser(args, true);
            var cmd = parser.Arguments[0].ToLower();
            var mapper = commands.First(kvp => kvp.Name.ToLower() == cmd);
            var commandType = mapper.CommandType;

            var command = Activator.CreateInstance(commandType) as Command;

            mapper.MapArguments(parser.Arguments, command);
            parser.SetOptionAndFlagValues(command);
            Validator.ValidateObject(command, new ValidationContext(command));
            return command;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using YACLAP.Commands;
using YACLAP.Extensions;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace YACLAP.Tests
{

    public class CommandParserTests
    {
        private readonly ICommandMapper[] _commandMappers = new List<ICommandMapper>
        {
            AddCommandMapper,
            UpdateCommandMapper,
            ReplaceCommandMapper,
            DeleteCommandMapper,
        }.ToArray();


        [TestCase("add|filename|--key|a-value", typeof(AddCommand))]
        [TestCase("update|filename|--key|a-value", typeof(UpdateCommand))]
        [TestCase("replace|filename1|filename2|--archive", typeof(ReplaceCommand))]
        [TestCase("delete|filename1|--confirm", typeof(DeleteCommand))]
        public void Parse_should_Parse_command(string testArgs, Type expectedCommand)
        {
            var command = CommandParser.Parse(testArgs.ToArgsArray(), _commandMappers);
            Assert.That(command, Is.TypeOf(expectedCommand));
            command.Execute();
        }

        [TestCase("add|filename|--key|a-value", typeof(AddCommand))]
        [TestCase("update|filename|--key|a-value", typeof(UpdateCommand))]
        [TestCase("replace|filename1|filename2|--archive", typeof(ReplaceCommand))]
        [TestCase("delete|filename1|--confirm", typeof(DeleteCommand))]
        public void Parse_should_Parse_command_using_custom_resolver(string testArgs, Type expectedCommand)
        {

            var usedCustom = false;
            var command = CommandParser.Parse(testArgs.ToArgsArray(), _commandMappers, type =>
            {
                usedCustom = true;
                return (Command) Activator.CreateInstance(type);
            });
            Assert.That(command, Is.TypeOf(expectedCommand));
            Assert.True(usedCustom);
            command.Execute();
        }

        [Test]
        public void Parse_should_use_argument_mappings()
        {
            var testArgs = "replace|filename1|filename2|--archive";
            var cmd = (ReplaceCommand) CommandParser.Parse(testArgs.ToArgsArray(), _commandMappers);

            Assert.That(cmd.SourceFilename, Is.EqualTo("filename1"));
            Assert.That(cmd.DestinationFilename, Is.EqualTo("filename2"));
            Assert.False(cmd.Debug);
            Assert.True(cmd.Archive);
        }

        [Test]
        public void Parse_performs_standard_data_annotation_validations()
        {
            var testArgs = "delete";

            Assert.Throws<ValidationException>( ()=>
            {
                ICommandMapper[] commands = {DeleteCommandMapper};
                CommandParser.Parse(testArgs.ToArgsArray(), commands);
            });
        }

        [Test]
        public void Parse_populates_flags_and_options()
        {
            var expectedFilename = "filename.tst";
            var testArgs = $"delete|{expectedFilename}|--confirm";

            ICommandMapper[] commands = { DeleteCommandMapper };
            var command = (DeleteCommand)CommandParser.Parse(testArgs.ToArgsArray(), commands);

            Assert.That(command.SourceFilename, Is.EqualTo(expectedFilename));
            Assert.True(command.Confirm);
            Assert.False(command.Debug);
        }


        [Test]
        public void Parse_throws_for_unsupported_command()
        {
            var testArgs = $"unsupported|filename|--confirm";

            ICommandMapper[] commands = { DeleteCommandMapper };
            Assert.Throws<ArgumentException>(() => CommandParser.Parse(testArgs.ToArgsArray(), commands));
        }

        [Test]
        public void Parse_non_string_options_are_converted()
        {
            var testArgs = $"add|filename|--number|27";

            var cmd = (AddCommand) CommandParser.Parse(testArgs.ToArgsArray(), _commandMappers);

            Assert.That(cmd.Number, Is.EqualTo(27));
        }
        #region Test Command's

        class CommonOptionsAndFlags : Command
        {
            public bool Debug { get; set; }
            public string Log { get; set; }
            public int Number { get; set; }

            public CommonOptionsAndFlags(string commandName) : base(commandName)
            {

            }

            public override void Execute()
            {
                Console.WriteLine($"Executing {CommandName} command.");
                Console.WriteLine(JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }

        private class AddCommand : CommonOptionsAndFlags
        {
            public string Filename { get; set; }
            public string Key { get; set; }

            public AddCommand() : base("add")
            {
            }
        }

        private class UpdateCommand : CommonOptionsAndFlags
        {
            public string Filename { get; set; }

            public string Key { get; set; }

            public UpdateCommand() : base("update")
            {
            }

        }

        private class ReplaceCommand : CommonOptionsAndFlags
        {
            public string SourceFilename { get; set; }
            public string DestinationFilename { get; set; }

            public bool Archive { get; set; }
            public ReplaceCommand() : base("replace")
            {
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class DeleteCommand : CommonOptionsAndFlags
        {
            [Required]
            public string SourceFilename { get; set; }

            public bool Confirm { get; set; }

            public DeleteCommand() : base("delete")
            {
            }
        }
        #endregion

        #region Test Command Mappers
        private static readonly CommandMapper<AddCommand> AddCommandMapper = new CommandMapper<AddCommand>(
            "add",
            (arguments, d) =>
            {
                d.Filename = arguments.DefaultingIndex(1);
            });

        private static readonly CommandMapper<UpdateCommand> UpdateCommandMapper = new CommandMapper<UpdateCommand>(
            "update",
            (arguments, d) =>
            {
                d.Filename = arguments.DefaultingIndex(1);
            });

        private static readonly CommandMapper<DeleteCommand> DeleteCommandMapper = new CommandMapper<DeleteCommand>(
            "delete",
            (arguments, d) =>
            {
                d.SourceFilename = arguments.DefaultingIndex(1);
            });

        private static readonly CommandMapper<ReplaceCommand> ReplaceCommandMapper = new CommandMapper<ReplaceCommand>(
            "replace",
            (arguments, d) =>
            {
                d.SourceFilename = arguments.DefaultingIndex(1);
                d.DestinationFilename = arguments.DefaultingIndex(2);
            });
        #endregion

    }
}
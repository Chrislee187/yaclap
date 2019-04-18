using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using YACLAP.Commands;
using YACLAP.Extensions;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace YACLAP.Tests
{

    public class CommandParserTests
    {
        private readonly ICommandMapper[] _commands = new List<ICommandMapper>
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
        public void Create_should_map_command(string testArgs, Type expectedCommand)
        {
            Assert.That(CommandParser.Parse(testArgs.ToArgsArray(), _commands), Is.TypeOf(expectedCommand));
        }

        [Test]
        public void Create_should_use_argument_mappings()
        {
            var testArgs = "replace|filename1|filename2|--archive";
            var cmd = (ReplaceCommand) CommandParser.Parse(testArgs.ToArgsArray(), _commands);

            Assert.That(cmd.SourceFilename, Is.EqualTo("filename1"));
            Assert.That(cmd.DestinationFilename, Is.EqualTo("filename2"));
            Assert.False(cmd.Debug);
            Assert.True(cmd.Archive);
        }

        [Test]
        public void Create_performs_standard_data_annotation_validations()
        {
            var testArgs = "delete";

            Assert.Throws<ValidationException>( ()=>
            {
                ICommandMapper[] commands = {DeleteCommandMapper};
                CommandParser.Parse(testArgs.ToArgsArray(), commands);
            });
        }

        #region Test Command's

        abstract class CommonOptionsAndFlags : Command
        {
            public bool Debug { get; set; }
            public string Log { get; set; }
            public abstract override void Execute();

            public CommonOptionsAndFlags(string name) : base(name)
            {

            }
        }

        private class AddCommand : CommonOptionsAndFlags
        {
            public string Filename { get; set; }
            public string Key { get; set; }

            public AddCommand() : base("add")
            {
            }

            public override void Execute()
            {
                throw new NotImplementedException();
            }
        }

        private class UpdateCommand : CommonOptionsAndFlags
        {
            public string Filename { get; set; }

            public string Key { get; set; }

            public UpdateCommand() : base("edit")
            {
            }

            public override void Execute()
            {
                throw new NotImplementedException();
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

            public override void Execute()
            {
                throw new NotImplementedException();
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

            public override void Execute()
            {
                throw new NotImplementedException();
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
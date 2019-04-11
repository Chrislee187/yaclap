using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable PossibleNullReferenceException

namespace YACLAP.Tests
{
    public class ReflectionParserTests
    {
        public class FlagArgument
        {
            public bool MyFlag { get; set; }
        }

        [Test]
        public void Should_handle_no_arguments()
        {
            string[] args = { "" };

            var parser = ReflectionParser.CreateParser<FlagArgument>(args);

            Assert.False(parser.HasCommand);
            Assert.False(parser.HasSubCommand);
        }
        [Test]
        public void Should_handle_single_flag()
        {
            string[] args = { "--myflag" };

            var parser = ReflectionParser.CreateParser<FlagArgument>(args);

            Assert.False(parser.HasCommand);
            Assert.False(parser.HasSubCommand);
            Assert.NotNull(parser.Data);
            Assert.True((parser.Data as FlagArgument).MyFlag);
        }


        public class Command1
        {
            public bool Command1Flag { get; set; }
            public string Command1Option { get; set; }
        }
        public class Command2
        {
            public bool Command2Flag { get; set; }
            public string Command2Option { get; set; }
        }
        [Test]
        public void Should_find_correct_command_object()
        {
            string[] args = { "command2" };

            var parser = ReflectionParser.CreateParser(args, typeof(Command1), typeof(Command2));

            Assert.True(parser.HasCommand);
            Assert.That(parser.Command, Is.EqualTo("command2"));
            Assert.False(parser.HasSubCommand);
            Assert.NotNull(parser.Data);
        }

        public class Command1SubCommand1
        {
            public bool Command1SubCommand1Flag { get; set; }
            public string Command1SubCommand1Option { get; set; }
        }
        [Test]
        public void Should_handle_find_correect_subcommand_object()
        {
            string[] args = { "command1", "subcommand1" };

            var parser = ReflectionParser.CreateParser(args, typeof(Command1), typeof(Command2), typeof(Command1SubCommand1));

            Assert.True(parser.HasCommand);
            Assert.That(parser.Command, Is.EqualTo("command1"));
            Assert.True(parser.HasSubCommand);
            Assert.That(parser.SubCommand, Is.EqualTo("subcommand1"));
            Assert.NotNull(parser.Data);
            Assert.That(parser.Data, Is.TypeOf<Command1SubCommand1>());
        }

        [Test]
        public void Should_handle_single_option()
        {
            string[] args = { "--command1option", "value" };

            var parser = ReflectionParser.CreateParser<Command1>(args);

            Assert.False(parser.HasCommand);
            Assert.False(parser.HasSubCommand);
            Assert.NotNull(parser.Data);
            Assert.That((parser.Data as Command1).Command1Option, Is.EqualTo("value"));
        }

        [Test]
        public void Should_handle_flags_and_options()
        {
            string[] args = { "--command1option", "value", "--command1flag" };

            var parser = ReflectionParser.CreateParser<Command1>(args);

            Assert.NotNull(parser.Data);
            Assert.True((parser.Data as Command1).Command1Flag);
            Assert.That((parser.Data as Command1).Command1Option, Is.EqualTo("value"));
        }

        [Test]
        public void Should_ignore_non_existant_flags_and_options()
        {
            string[] args = { "--flag1", "--option1" };

            var parser = ReflectionParser.CreateParser<Command1>(args);

            Assert.False(parser.Error);
        }

        [Test]
        public void Should_handle_option_with_spaces_in_value()
        {
            string[] args = { "--command1option", "two values" };
            var parser = ReflectionParser.CreateParser<Command1>(args);

            Assert.NotNull(parser.Data);
            Assert.That((parser.Data as Command1).Command1Option, Is.EqualTo("two values"));
        }

        [Test]
        public void Should_handle_subcommand_flags_and_options()
        {
            string[] args = { "command1", "subcommand1", "--command1subcommand1option", "value", "--command1subcommand1flag" };

            var parser = ReflectionParser.CreateParser(args, typeof(Command1), typeof(Command2), typeof(Command1SubCommand1));

            Assert.NotNull(parser.Data);
            Assert.That(parser.Data, Is.TypeOf<Command1SubCommand1>());
            Assert.True((parser.Data as Command1SubCommand1).Command1SubCommand1Flag);
            Assert.That((parser.Data as Command1SubCommand1).Command1SubCommand1Option, Is.EqualTo("value"));
        }

        class RequiredOption

        {
            [Required(ErrorMessage = "Name option is required")]
            public string Name { get; set; }
        }
        [Test]
        public void Should_support_standard_annotation_based_validations()
        {
            string[] args = { "--wrongoption", "value" };

            var parser = ReflectionParser.CreateParser<RequiredOption>(args);

            AssertValidationError(parser, $"Name option is required");
        }

        private static void AssertValidationError(ReflectionParser parser, string expected)
        {
            Assert.True(parser.Error);
            Assert.That(parser.Errors, Contains.Substring(expected));
        }
    }
}
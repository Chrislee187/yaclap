using NUnit.Framework;

namespace YACLAP
{
    public class ParsedArgumentsTests
    {
        [Test]
        public void Should_handle_no_arguments()
        {
            string[] args = {""};

            var parser = new ParsedArguments(args);

            Assert.False(parser.HasCommand);
            Assert.False(parser.HasSubCommand);
            Assert.False(parser.Error);
        }

        [Test]
        public void Should_handle_commands()
        {
            string[] args = { "command" };

            var parser = new ParsedArguments(args);

            Assert.True(parser.HasCommand);
            Assert.That(parser.Command, Is.EqualTo("command"));
            Assert.False(parser.HasSubCommand);
            Assert.False(parser.Error);
        }

        [Test]
        public void Should_handle_subcommands()
        {
            string[] args = { "command","sub" };

            var parser = new ParsedArguments(args);

            Assert.True(parser.HasCommand);
            Assert.That(parser.Command, Is.EqualTo("command"));
            Assert.True(parser.HasSubCommand);
            Assert.That(parser.SubCommand, Is.EqualTo("sub"));
            Assert.False(parser.Error);
        }

        [Test]
        public void Should_handle_single_flag()
        {
            string[] args = { "--flag1" };

            var parser = new ParsedArguments(args);

            Assert.False(parser.HasCommand);
            Assert.False(parser.HasSubCommand);
            Assert.True(parser.Flags("flag1"));
            Assert.False(parser.HasOption("flag1"));
            Assert.False(parser.Error);
        }

        [Test]
        public void Should_handle_single_option()
        {
            string[] args = { "--option", "value" };

            var parser = new ParsedArguments(args);

            Assert.False(parser.HasCommand);
            Assert.False(parser.HasSubCommand);
            Assert.That(parser.Option("option"), Is.EqualTo("value"));
            Assert.False(parser.Error);
        }

        [Test]
        public void Should_handle_flags_and_options()
        {
            string[] args = { "--flag1", "--option1", "option1value", "--flag2", "--flag3" };

            var parser = new ParsedArguments(args);

            Assert.False(parser.HasCommand);
            Assert.False(parser.HasSubCommand);
            Assert.True(parser.Flags("flag1"));
            Assert.True(parser.Flags("flag2"));
            Assert.True(parser.Flags("flag3"));
            Assert.That(parser.Option("option1"), Is.EqualTo("option1value"));
            Assert.False(parser.Error);
        }
        [Test]
        public void Should_handle_non_existant_flags_and_options()
        {
            string[] args = { "--flag1", "--option1", "option1value", "--flag2", "--flag3" };

            var parser = new ParsedArguments(args);

            Assert.False(parser.HasCommand);
            Assert.False(parser.HasSubCommand);
            Assert.False(parser.Flags("flagx"));
            Assert.That(parser.Option("option2"), Is.Null);
            Assert.False(parser.Error);
        }

    }
}
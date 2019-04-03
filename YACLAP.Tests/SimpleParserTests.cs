using NUnit.Framework;

namespace YACLAP.Tests
{
    public class SimpleParserTests
    {
        [Test]
        public void Should_handle_no_arguments()
        {
            string[] args = {""};

            var parser = new SimpleParser(args);

            Assert.False(parser.HasCommand);
            Assert.False(parser.HasSubCommand);
        }

        [Test]
        public void Should_handle_commands()
        {
            string[] args = { "command" };

            var parser = new SimpleParser(args);

            Assert.True(parser.HasCommand);
            Assert.That(parser.Command, Is.EqualTo("command"));
            Assert.False(parser.HasSubCommand);
        }

        [Test]
        public void Should_handle_subcommands()
        {
            string[] args = { "command","sub" };

            var parser = new SimpleParser(args);

            Assert.True(parser.HasCommand);
            Assert.That(parser.Command, Is.EqualTo("command"));
            Assert.True(parser.HasSubCommand);
            Assert.That(parser.SubCommand, Is.EqualTo("sub"));
        }

        [Test]
        public void Should_handle_single_flag()
        {
            string[] args = { "--flag1" };

            var parser = new SimpleParser(args);

            Assert.False(parser.HasCommand);
            Assert.False(parser.HasSubCommand);
            Assert.True(parser.Flags("flag1"));
            Assert.False(parser.HasOption("flag1"));
        }

        [Test]
        public void Should_handle_single_option()
        {
            string[] args = { "--option", "value" };

            var parser = new SimpleParser(args);

            Assert.False(parser.HasCommand);
            Assert.False(parser.HasSubCommand);
            Assert.That(parser.Option("option"), Is.EqualTo("value"));
        }

        [Test]
        public void Should_handle_flags_and_options()
        {
            string[] args = { "--flag1", "--option1", "option1value", "--flag2", "--flag3" };

            var parser = new SimpleParser(args);

            Assert.False(parser.HasCommand);
            Assert.False(parser.HasSubCommand);
            Assert.True(parser.Flags("flag1"));
            Assert.True(parser.Flags("flag2"));
            Assert.True(parser.Flags("flag3"));
            Assert.That(parser.Option("option1"), Is.EqualTo("option1value"));
        }

        [Test]
        public void Should_handle_non_existant_flags_and_options()
        {
            string[] args = { "--flag1", "--option1", "option1value", "--flag2", "--flag3" };

            var parser = new SimpleParser(args);

            Assert.False(parser.HasCommand);
            Assert.False(parser.HasSubCommand);
            Assert.False(parser.Flags("flagx"));
            Assert.That(parser.Option("option2"), Is.Null);
        }

        [Test]
        public void Should_handle_option_with_spaces_in_value()
        {
            string[] args = { "--flag1", "--option1", "two words"};

            var parser = new SimpleParser(args);

            Assert.That(parser.Option("option1"), Is.EqualTo("two words"));
        }
        [Test]
        public void Should_handle_command_subcommand_flags_and_options()
        {
            string[] args = { "command1", "subcommand1", "--command1subcommand1option", "value", "--command1subcommand1flag" };

            var parser = new SimpleParser(args);

            Assert.True(parser.HasCommand);
            Assert.That(parser.Command, Is.EqualTo("command1"));
            Assert.True(parser.HasSubCommand);
            Assert.That(parser.SubCommand, Is.EqualTo("subcommand1"));
            Assert.True(parser.Flags("command1subcommand1flag"));
            Assert.That(parser.Option("command1subcommand1option"), Is.EqualTo("value"));


        }

    }
}
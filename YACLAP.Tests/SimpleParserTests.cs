using NUnit.Framework;
using YACLAP.Extensions;

namespace YACLAP.Tests
{
    public class SimpleParserTests
    {

        [Test]
        public void Should_parse_flags()
        {
            var testArgs = "--debug|--flag".ToArgsArray();

            var parser = new SimpleParser(testArgs);

            Assert.True(parser.Flag("debug"));
            Assert.True(parser.Flag("flag"));
            Assert.That(parser.Arguments.Length, Is.EqualTo(0));
        }

        [Test]
        public void Should_parse_options()
        {
            var testArgs = "--option|value|--option2|multi word value".ToArgsArray();

            var parser = new SimpleParser(testArgs);
            Assert.That(parser.Option("option"), Is.EqualTo("value"));
            Assert.That(parser.Option("option2"), Is.EqualTo("multi word value"));
            Assert.That(parser.Arguments.Length, Is.EqualTo(0));
        }

        [Test]
        public void Should_parse_arguments()
        {
            var testArgs = "arg1|arg2|arg three".ToArgsArray();

            var parser = new SimpleParser(testArgs);
            Assert.That(parser.Arguments.Length, Is.EqualTo(3));
            Assert.That(parser.Arguments[0], Is.EqualTo("arg1"));
            Assert.That(parser.Arguments[1], Is.EqualTo("arg2"));
            Assert.That(parser.Arguments[2], Is.EqualTo("arg three"));
        }

        [Test]
        public void Should_parse_combination()
        {
            var testArgs = "argument1|--flag|--option|a value".ToArgsArray();

            var parser = new SimpleParser(testArgs);
            Assert.True(parser.Flag("flag"));
            Assert.That(parser.Option("option"), Is.EqualTo("a value"));
            Assert.That(parser.Arguments.Length, Is.EqualTo(1));
            Assert.That(parser.Arguments[0], Is.EqualTo("argument1"));
        }


        [Test]
        public void Should_parse_arguments_from_anywhere()
        {
            var testArgs = "argument1|--flag|--option|a value|argument2".ToArgsArray();

            var parser = new SimpleParser(testArgs);
            Assert.True(parser.Flag("flag"));
            Assert.That(parser.Option("option"), Is.EqualTo("a value"));
            Assert.That(parser.Arguments.Length, Is.EqualTo(2));
            Assert.That(parser.Arguments[0], Is.EqualTo("argument1"));
            Assert.That(parser.Arguments[1], Is.EqualTo("argument2"));
        }

        [Test]
        public void Should_throw_in_arguments_first_mode_when_argument_found_after_flag_or_option()
        {
            var testArgs = "argument1|--flag|--option|a value|argument2".ToArgsArray();

            Assert.That(() => new SimpleParser(testArgs, true), Throws.Exception);
        }

    }

}
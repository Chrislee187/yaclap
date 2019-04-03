using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;

namespace YACLAP.Tests
{
    public class Tests
    {

        class CommonOptions
        {
            public bool Flag1 { get; set; }
            public string Option1 { get; set; }
            public int Option2 { get; set; }

            public string Option3 { get; set; }
            public string Option4 { get; set; }
        }

        readonly string[] _argsCmd1SubCmd1 = { "cmd1", "subcmd1", "--customFlag1", "--flag1", "--option1", "value1", "--option2", "42", "--option3", "abc,def,ghi", "--option4", "two words" };
        class Cmd1SubCmd1 : CommonOptions
        {
            public bool CustomFlag1 { get; set; }
        }

        readonly string[] _argsCmd1SubCmd2 = { "cmd1", "subcmd2", "--customFlag2", "--flag1", "--option1", "value1", "--option2", "42", "--option3", "abc, def, ghi", "--option4", "two words" };
        class Cmd1SubCmd2 : CommonOptions
        {
            public bool CustomFlag2 { get; set; }
        }

        readonly string[] _argsCmd2SubCmd1 = { "cmd2", "subcmd1", "--customFlag3", "--flag1", "--option1", "value1", "--option2", "42", "--option3", "abc, def, ghi", "--option4", "two words" };
        class Cmd2SubCmd1 : CommonOptions
        {
            public bool CustomFlag3 { get; set; }
        }

        readonly string[] _argsCmd2SubCmd2 = { "cmd2", "subcmd2", "--customFlag4", "--flag1", "--option1", "value1", "--option2", "42", "--option3", "abc, def, ghi", "--option4", "two words" };
        class Cmd2SubCmd2 : CommonOptions
        {
            public bool CustomFlag4 { get; set; }
        }


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SimpleParserStructuralSpike()
        {
            var pargs = Yaclap.Parser(_argsCmd1SubCmd1);

            Assert.That(pargs.Command, Is.EqualTo("cmd1"));
            Assert.True(pargs.HasCommand);
            Assert.That(pargs.SubCommand, Is.EqualTo("subcmd1"));
            Assert.True(pargs.HasSubCommand);
            Assert.True(pargs.Flags("customFlag1"));
            Assert.True(pargs.Flags("flag1"));
            Assert.False(pargs.HasOption("flag1"));
            Assert.True(pargs.HasOption("option1"));
            Assert.False(pargs.Flags("option1"));
            Assert.That(pargs.Option("option1"), Is.EqualTo("value1"));
        }


        [Test]
        public void ReflectiveParserStructuralSpike()
        {

            Type[] argTypes =
            {
                typeof(Cmd1SubCmd1),
                typeof(Cmd1SubCmd2),
                typeof(Cmd2SubCmd1),
                typeof(Cmd2SubCmd2),
            };

            var parsed1 = Yaclap.Parser(_argsCmd1SubCmd1, argTypes);
            AssertNoErrors(parsed1);


            Assert.That(parsed1.Command, Is.EqualTo("cmd1"));
            Assert.That(parsed1.SubCommand, Is.EqualTo("subcmd1"));
            Assert.That(parsed1.Data, Is.TypeOf<Cmd1SubCmd1>());
            var cmd1SubCmd1 = parsed1.Data as Cmd1SubCmd1;
            Assert.That(cmd1SubCmd1.CustomFlag1, Is.True);
            Assert.That(cmd1SubCmd1.Option1, Is.EqualTo("value1"));
            Assert.That(cmd1SubCmd1.Option2, Is.EqualTo(42));
//            Assert.That(cmd1SubCmd1.Option3, Is.EqualTo("abc,def,ghi"));
            Assert.That(cmd1SubCmd1.Option4, Is.EqualTo("two words"));

            var parsed2 = Yaclap.Parser(_argsCmd1SubCmd2, argTypes);
            AssertNoErrors(parsed2);
            Assert.That(parsed2.Command, Is.EqualTo("cmd1"));
            Assert.That(parsed2.SubCommand, Is.EqualTo("subcmd2"));
            Assert.That(parsed2.Data, Is.TypeOf<Cmd1SubCmd2>());

            var parsed3 = Yaclap.Parser(_argsCmd2SubCmd1, argTypes);
            AssertNoErrors(parsed3);
            Assert.That(parsed3.Command, Is.EqualTo("cmd2"));
            Assert.That(parsed3.SubCommand, Is.EqualTo("subcmd1"));
            Assert.That(parsed3.Data, Is.TypeOf<Cmd2SubCmd1>());

            var parsed4 = Yaclap.Parser(_argsCmd2SubCmd2, argTypes);
            AssertNoErrors(parsed4);
            Assert.That(parsed4.Command, Is.EqualTo("cmd2"));
            Assert.That(parsed4.SubCommand, Is.EqualTo("subcmd2"));
            Assert.That(parsed4.Data, Is.TypeOf<Cmd2SubCmd2>());

            var results = new List<ValidationResult>();
            var context = new ValidationContext(parsed1.Data, null, null);

        }

        private void AssertNoErrors(ReflectionParser parser)
        {
            Assert.False(parser.Error, parser.Errors);
        }
        [Test, Ignore("WIP")]
        public void DeclarativeParserStructuralSpike()
        {
//            string[] args = { "command1", "cmd1subcmd1", "--flag1 --option1 value1 --option2 42 --option3 abc,def,ghi --option4 'two words'" };
//
//            var flags = "--flag1";
//            var options = "--option1 value1 --option2 42 --option3 abc,def,ghi --option4 'two words'";
//
//            string[] noCommandsArgs = { $"{flags}{options}" };
//            var noCommandsParser = YACLAP.CreateParser((builder) => BuildTestOptions(builder.NoCommands));
//
//            string[] commandNoSubCommandArgs = { $"cmd1 {flags}{options}" };
//            var commandNoSubCommandParser = YACLAP.CreateParser((builder) =>
//            {
//                builder
//                    .AddCommand("cmd1", commandBuilder => BuildTestOptions(commandBuilder.NoSubCommand));
//            });
//
//            string[] commandAndSubCommandArgs = { $"cmd1 subcmd1 {flags}{options}" };
//            var commandAndSubCommandParser = YACLAP.CreateParser((builder) =>
//            {
//                builder
//                    .AddCommand("cmd1", cmdBuilder =>
//                    {
//                        cmdBuilder.AddSubCommand("subcmd1", BuildTestOptions);
//                    });
//;
//            });
//
//            string[] complexParserArg1 = { $"cmd1 subcmd1 {flags}{options}" };
//            string[] complexParserArg2 = { $"cmd1 subcmd2 {flags}{options}" };
//            string[] complexParserArg3 = { $"cmd2 subcmd1 {flags}{options}" };
//            string[] complexParserArg4 = { $"cmd3 subcmd2 {flags}{options}" };
//            var complexParser = YACLAP.CreateParser((builder) =>
//            {
//                builder
//                    .AddCommand("cmd1", cmdBuilder =>
//                    {
//                        cmdBuilder.AddSubCommand("subcmd1", BuildTestOptions);
//                    })
//                    .AddCommand("cmd2", cmdBuilder =>
//                    {
//                        cmdBuilder.AddSubCommand("subcmd2", BuildTestOptions);
//                    })
//                    .AddCommand("cmd1", cmdBuilder =>
//                    {
//                        cmdBuilder.AddSubCommand("subcmd1", BuildTestOptions);
//                    })
//                    .AddCommand("cmd2", cmdBuilder =>
//                    {
//                        cmdBuilder.AddSubCommand("subcmd2", BuildTestOptions);
//                    })
//                    ;
//                ;
//            });
//
//
//
//            Assert.That(parsed.Command, Is.EqualTo("command1"));
//            Assert.True(parsed.HasCommand);
//            Assert.That(parsed.SubCommand, Is.EqualTo("cmd1sub1"));
//            Assert.True(parsed.HasSubCommand);
//            Assert.True(parsed.Flags("flag1"));
//            Assert.True(parsed.HasOption("option1"));
//            Assert.That(parsed.Option<string>("option1"), Is.EqualTo("value1"));
//
//            var cmd1 = new Command1Command(parsed);

//            Assert.That(cmd1.Name, Is.EqualTo("command1"));


        }

        private static void BuildTestOptions(OptionBuilder subcmdBuilder)
        {
            subcmdBuilder.AddFlag("flag1")
                .AddOption<string>("option1")
                .AddOption<int>("option2")
                .AddOption<string[]>("option3")
                .AddOption<string>("option4");
        }
    }

    public class Command1Command : Command
    {
        public Command1Command(SimpleParser parsed) : base(parsed)
        {
            Name = parsed.Command;
        }
    }
    class NoCommandArgs
    {
        public bool Flag1;
        public string Option1;
        public int Option2;
        public string[] Option3;
        public string Option4;
    }

    class Command1NoSubCommandArgs
    {
        class Cmd1
        {
            public bool Flag1;
            public string Option1;
            public int Option2;
            public string[] Option3;
            public string Option4;
        }
    }

    class Command1AndSubCommand1Args
    {
        class Cmd1
        {
            class SubCmd1
            {
                public bool Flag1;
                public string Option1;
                public int Option2;
                public string[] Option3;
                public string Option4;
            }
        }
    }

    class ComplexCommandsAndArgs
    {
        class Cmd1
        {
            class SubCmd1
            {
                public bool Flag1;
                public string Option1;
                public int Option2;
                public string[] Option3;
                public string Option4;
            }
            class SubCmd2
            {
                public bool Flag1;
                public string Option1;
                public int Option2;
                public string[] Option3;
                public string Option4;
            }
        }
        class Cmd2
        {
            class SubCmd1
            {
                public bool Flag1;
                public string Option1;
                public int Option2;
                public string[] Option3;
                public string Option4;
            }
            class SubCmd2
            {
                public bool Flag1;
                public string Option1;
                public int Option2;
                public string[] Option3;
                public string Option4;
            }
        }
    }



}
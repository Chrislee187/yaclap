# Yet Another Command Line Argument Parser (YACLAP)
Looked at a few of the multitude of parser libraries for command line arguments. I initially looked at the [CommandLineProvider](link required) in .NET Core and whilst this is great for override configuration values, it doesn't quite work for typical command line argument usage, namely it doesn't really support flags, you would need to use --flag=true/--flag=false which didn't feel quite right.

So after spending some time coming up with a good acronym ;) I thought about my requirements.

# Usage requirements
These are the requirements around how we want to use command line arguments;

* Support for "commands" - a keyword, without '--' etc, as the first argument)
* Support for "sub-commands"  - a keyword, without '--' etc, as the second argument that passed to the first "command")
* Support for "flags" - a keyword, with '--' etc. but with no value associated so is a boolean
* Support for "arguments" - a keyword, with '--' etc. and with a value (quote delimited if contains spaces)

**Extras**

* Support for JSON arguments - arguments whose value is a JSON string
* Support for JSON file arguments - arguments whose value begins with '@' and represents a file containing JSON
* Support for system annotations for validation

# Development Requirements
Requirements around how we want to code our support for command line arguments.

## Simple/NoSchema
```
string[] args = { "command1", "cmd1subcmd1", "--flag1 --option1 value1 --option2 42 --option3 abc,def,ghi --option4 'two words'" };

ParsedArguments pargs = YACLAP.SimpleParser(args);

Assert.That(pargs.Command, Is.EqualTo("command1"));
Assert.True(pargs.HasCommand);
Assert.That(pargs.SubCommand, Is.EqualTo("cmd1sub1"));
Assert.True(pargs.HasSubCommand);
Assert.True(pargs.Flags("flag1"));
Assert.False(pargs.Flags("flag2"));
Assert.True(pargs.HasOption("option1"));
Assert.That(pargs.Option<string>("option1"), Is.EqualTo("value1"));
Assert.False(pargs.HasOption("optionX"));
```

## Reflective schema
YACLAP support simple/schema-less argument patterns but it also supports a more strongly typed approach through the use of reflection across simple C# objects.

```
class CommonOptions
{
    public bool Flag1;
    public string Option1;
    public int Option2;
    public string[] Option3;
    public string Option4;
}
class Cmd1SubCmd1 : CommonOptions
{
    public bool CustomFlag1 { get; set; }
}
class Cmd1SubCmd2: CommonOptions
{
    public bool CustomFlag2 { get; set; }
}
string[] args1 = { "cmd1", "subcmd1", "--customFlag1 --flag1 --option1 value1 --option2 42 --option3 abc,def,ghi --option4 'two words'" };
string[] args2 = { "cmd1", "subcmd2", "--customFlag2 --flag1 --option1 value1 --option2 42 --option3 abc,def,ghi --option4 'two words'" };
Type[] argTypes = {typeof(Cmd1SubCmd1), typeof(Cmd1SubCmd2)};

var parsed1 = YACLAP.Parser(args1, argTypes);
Assert.That(parsed1.Command, Is.EqualTo("cmd1"));
Assert.That(parsed1.SubCommand, Is.EqualTo("subcmd1"));
Assert.That(parsed1.Data, Is.TypeOf<Cmd1SubCmd1>());

var parsed2 = YACLAP.Parser(args2, argTypes);
Assert.That(parsed2.Command, Is.EqualTo("cmd1"));
Assert.That(parsed2.SubCommand, Is.EqualTo("subcmd2"));
Assert.That(parsed2.Data, Is.TypeOf<Cmd1SubCmd2>());

```


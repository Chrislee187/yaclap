# Yet Another Command Line Argument Parser (YACLAP)
Over the years I've written serveral (and tried several more 3rd-party implementations) of approaches to parsing command line arguments.

I was hoping .NET Standard would offer... well... a standard for this but it doesn't, and please note, I have investigated the [Commandline Configuration Provider](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.2#command-line-configuration-provider) and whilst this is great for overriding your appSettings properties, but it's not that useful for more typical console applications that may not even have an appSettings file but may still need some complex command line options.

So what sort of parsing do we need, let's look at some examples;

```
dotnet new
dotnet new sln
dotnet new sln --dry-run
dotnet new sln --dry-run --output testSln
dotnet sln xyz.sln list
etc.
```

From these few examples we can determine some simple rules that will allow us to define a simple parser;
* Some arguments may act as commands
* Some arguments are simply strings
* Some arguments represent boolean values (i.e. --dry-run), typical ones that default to false
* Some arguments represent addition data to be used (i.e. --output testSln). 

Using these rules we can develop a simple parser to represent the above as;
* "Flags" - arguments that have the option prefix ('--') but no value in the following index (or the following value also has an option prefix)
* "Options" - arguments that have the option prefix ('--') and a value in the next index
* "Arguments" - these are the remain indexes in the args array, positional Arguments are not supported, an all non-option/flag arguments will be placed in the Arguments array in the original order found on the command line

This gives us a `SimpleParser` interface;

```
    public interface ISimpleParser
    {
        string[] Arguments { get; }
        string Option(string name);
        bool Flag(string name);
        bool HasOption(string name);
        bool HasFlag(string optionName);
    }
```

For simple console applications that only have a few arguments, flags and/or options this interface may be sufficient, however more complex console applications often require more complex command-lines, often to support multiple "Commands" within an application. For example, `dotnet new`, `dotnet build`, `dotnet sln`. All act on dotnet projects/solutions but all require different sets values to be configurable on a run-by-run basis.

Although obviously related to the core functionality of the tool these commands may be unrelated to each other and each require differing sets of arguments/flags and options.

Whilst we could easily use the ISimpleParser interface to work out which command is being requested and therefore which options/flags are need. It would be far nicer to abstract this in to a cleaner, reusable implementation. To this end we have the `CommandParser`, which utilizes the SimpleParser described above;

Commands are simply classes that inherit from; 
```
    public abstract class Command
    {
        protected Command(string name)
        {
            CommandName = name;
        }
        public string CommandName { get; }

        public abstract void Execute();
    }
```

and have simple properties on them that match the Flag/Option names you use, i.e.
```
        private class AddCommand : Command
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
```

The CommandName field will be used to the match the command found (case-insensitive comparison) as the first argument in the SimpleParser.Arguments array.

The CommandParser can then determine from a collection of "Commands" which one much the current `args` array and generate an instance of that command, with arguments, flags and options, mapped and populated and return it for execution.

# Validation
The CommandParser supports default validation via the `System.ComponentModel.DataAnnotations` namespace and the [`Validator.ValidateObject()`](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validator?view=netcore-2.2) method.



**Extras**
* No support for dependencies when creating the Command instances

* Has no real error handling per se. No way of generating help nor showing any.
* Support for JSON arguments - arguments whose value is a JSON string
* Support for JSON file arguments - arguments whose value begins with '@' and represents a file containing JSON
* Formalised and tested support for setting values of different datatypes on Command instances (i.e. not big if/else construct in SetOptionAndFlagValues()



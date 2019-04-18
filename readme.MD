# Yet Another Command Line Argument Parser (YACLAP)
Looked at a few of the multitude of parser libraries for command line arguments. I initially looked at the [CommandLineProvider](link required) in .NET Core and whilst this is great for override configuration values, it doesn't quite work for typical command line argument usage, namely it doesn't really support flags, you would need to use --flag=true/--flag=false which didn't feel quite right.

A command line could be considered as containing three types of information (excluding the .EXE file itself)

* Flags/Switches - These represent boolean values and are represented on the command line by a single token prefixed by '--'
i.e.
`program.exe --debug`

* Options - These represent more complex values of any simple data-type and are represented by a singlet oken prefixed by '--' followed by a space and then a value. Values containing spaces must be quoted
i.e.
`program.exe --message "My message"

* Arguments - Any values that do not have a prefix of '--' and are not an option value
i.e.
`program.exe abc.txt def.txt`

# 


**Extras**

* Support for JSON arguments - arguments whose value is a JSON string
* Support for JSON file arguments - arguments whose value begins with '@' and represents a file containing JSON
* Support for system annotations for validation


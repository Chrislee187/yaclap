using System;
using System.Collections.Generic;
using System.Linq;

namespace YACLAP
{
    public static class YACLAP
    {
        public static ParsedArguments SimpleParser(string[] args)
        {
            return new ParsedArguments(args);
        }

        // Single type/No commands
        public static ReflectionParser Parser<T>(string[] args)
        {
            var reflectionParser = CreateParser(typeof(T));
            return reflectionParser;
        }

        // Multiple command support with multiple types
        public static ReflectionParser Parser(string[] args, params Type[] argObjects)
        {
            if (!argObjects.Any()) throw new ArgumentOutOfRangeException("No argument types supplied!");
            if (!args.Any()) { 
                // TODO: This should create default arguments return
                throw new ArgumentOutOfRangeException("No argument supplied!");
            }

            var command = "";
            var subCommand = "";

            var argQueue = new Queue<string>(args);



            if (!argQueue.Peek().StartsWith("--"))
            {
                command = argQueue.Dequeue();
            }

            if (!argQueue.Peek().StartsWith("--"))
            {
                subCommand = argQueue.Dequeue();
            }

            var commandTypeName = $"{command}{subCommand}";

            var argObject = FindType(argObjects, commandTypeName);

            if (argObject == null) throw new ArgumentNullException($"Type for command '{commandTypeName}' not found.");
            
            var reflectionParser = CreateParser(argObject, command, subCommand);

            return reflectionParser as ReflectionParser;
        }

        public static ReflectionParser CreateParser(Type argObject, string command = null, string subCommand = null)
        {
            var instance = Activator.CreateInstance(argObject);

            Type myGeneric = typeof(ReflectionParser<>);
            Type constructedClass = myGeneric.MakeGenericType(argObject);

            ReflectionParser reflectionParser;
            if (command == null)
            {
                reflectionParser = (ReflectionParser) Activator.CreateInstance(constructedClass, new object[] { instance });
            }
            else
            {
                reflectionParser = (ReflectionParser)Activator.CreateInstance(constructedClass, new object[] { command, subCommand, instance });
            }

            return reflectionParser;
        }

        private static Type FindType(Type[] types, string name) => types.FirstOrDefault(t => t.Name.ToLower().Contains(name.ToLower()));
    }
}
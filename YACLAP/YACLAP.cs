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

        public static object CreateParser(Action<ParserBuilder> command1)
        {
            throw new NotImplementedException();
        }

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
            
            var instance = Activator.CreateInstance(argObject);

            Type myGeneric = typeof(ReflectionParser<>);
            Type constructedClass = myGeneric.MakeGenericType(argObject);
            var reflectionParser = Activator.CreateInstance(constructedClass, new object[]{ command, subCommand, instance});

            return reflectionParser as ReflectionParser;
        }

        private static Type FindType(Type[] types, string name) => types.FirstOrDefault(t => t.Name.ToLower().Contains(name.ToLower()));
    }
}
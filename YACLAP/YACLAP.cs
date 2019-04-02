using System;
using System.Collections.Generic;
using System.Linq;
using YACLAP.Extensions;
using System.ComponentModel.DataAnnotations;
namespace YACLAP
{
    public static class Yaclap
    {
        public static ParsedArguments SimpleParser(string[] args)
        {
            return new ParsedArguments(args);
        }

        // Single type/No commands
        public static IReflectionParser Parser<T>(string[] args)
        {
            var reflectionParser = CreateParser(typeof(T), new ParsedArguments(args));
            return reflectionParser;
        }

        // Multiple command support with multiple types
        public static IReflectionParser Parser(string[] args, params Type[] argObjects)
        {
            if (!argObjects.Any()) throw new ArgumentOutOfRangeException(nameof(argObjects),"No argument types supplied!");
            if (!args.Any()) { 
                // TODO: This should create default arguments return
                throw new ArgumentOutOfRangeException(nameof(args),"No argument supplied!");
            }

            var parser = new ParsedArguments(args);

            var commandTypeName = $"{parser.Command}{parser.SubCommand}";

            var argObject = FindType(argObjects, commandTypeName);

            if (argObject == null) throw new ArgumentNullException($"Type for command '{commandTypeName}' not found.");
            
            var reflectionParser = CreateParser(argObject, parser);

            return reflectionParser;
        }

        public static IReflectionParser CreateParser(Type argObject, ParsedArguments args)
        {
            var instance = Activator.CreateInstance(argObject);

            // TODO: Use reflection to determine flag and option names from the argObject, then use ParsedArguments to set values on the instance
            SetValues(instance, args, argObject);

            Type myGeneric = typeof(ReflectionParser<>);
            Type constructedClass = myGeneric.MakeGenericType(argObject);

            IReflectionParser reflectionParser;
            if (args.Command == null)
            {
                reflectionParser = (IReflectionParser) Activator.CreateInstance(constructedClass, new object[] { instance });
            }
            else
            {
                reflectionParser = (IReflectionParser)Activator.CreateInstance(constructedClass, new object[] { args.Command, args.SubCommand, instance });
            }

            return reflectionParser;
        }

        private static void SetValues(object instance, ParsedArguments args, Type argObject)
        {
            var memberInfos = argObject.GetProperties();
            foreach (var memberInfo in memberInfos)
            {
                if (memberInfo.PropertyType.IsArray)
                {
                    throw new NotImplementedException();
                }
                /* if member is boolean check args.Flag(), else args.Option() */
                var optionName = memberInfo.Name.ToLower();
                if (memberInfo.PropertyType == typeof(bool))
                {
                    var flag = args.Flags(optionName);
                    memberInfo.SetValue(instance, flag);
                }
                else 
                {
                    var value = args.Option(optionName);
                    if (memberInfo.PropertyType == typeof(int))
                    {
                        memberInfo.SetValue(instance, value.ToInt());
                    }
                    else if (memberInfo.PropertyType == typeof(double))
                    {
                        memberInfo.SetValue(instance, value.ToDouble());
                    }
                    else if (memberInfo.PropertyType == typeof(decimal))
                    {
                        memberInfo.SetValue(instance, value.ToDecimal());
                    }
                    else if (memberInfo.PropertyType == typeof(DateTime))
                    {
                        memberInfo.SetValue(instance, value.ToDateTime());
                    }
                    else
                    {
                        memberInfo.SetValue(instance, value);
                    }
                }
            }
        }

        private static Type FindType(Type[] types, string name) => types.FirstOrDefault(t => t.Name.ToLower().Contains(name.ToLower()));
    }
}
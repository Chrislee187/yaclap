using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using YACLAP.Extensions;

namespace YACLAP
{
    public interface IReflectionParser
    {
        string Command { get; }
        string SubCommand { get; }
        object Data { get; set; }
        bool Error { get; set; }
        string Errors { get; set; }
    }

    public class ReflectionParser : IReflectionParser
    {
        public string Command { get; }
        public string SubCommand { get; }
        public object Data { get; set; }
        public bool Error { get; set; }
        public string Errors { get; set; }

        public ReflectionParser(string command, string subCommand, object data) : this(data)
        {
            Command = command;
            SubCommand = subCommand;
        }
        public ReflectionParser(object data)
        {
            Validate(data);

            Data = data;
        }

        private void Validate(object data)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(data, null, null);
            if (!Validator.TryValidateObject(data, validationContext, validationResults))
            {
                // Validation failed, show errors
                Error = true;

                Errors = string.Join("\n", validationResults);
            }
        }


        // Multiple command support with multiple types
        public static IReflectionParser CreateParser(string[] args, Type argObject)
        {
            var parsedArgs = new SimpleParser(args);

            var instance = Activator.CreateInstance(argObject);

            SetValues(instance, parsedArgs, argObject);

            var myGeneric = typeof(ReflectionParser<>);
            var constructedClass = myGeneric.MakeGenericType(argObject);

            var reflectionParser = CreateReflectionParser(parsedArgs, constructedClass, instance);

            return reflectionParser;
        }

        private static IReflectionParser CreateReflectionParser(SimpleParser parsedArgs, Type constructedClass, object instance)
        {
            IReflectionParser reflectionParser;
            if (parsedArgs.Command == null)
            {
                reflectionParser = (IReflectionParser) Activator.CreateInstance(constructedClass, new object[] {instance});
            }
            else
            {
                reflectionParser = (IReflectionParser) Activator.CreateInstance(constructedClass,
                    new object[] {parsedArgs.Command, parsedArgs.SubCommand, instance});
            }

            return reflectionParser;
        }

        public static IReflectionParser CreateParser(string[] args, params Type[] argObjects)
        {
            if (!argObjects.Any()) throw new ArgumentOutOfRangeException(nameof(argObjects), "No argument types supplied!");
            if (!args.Any())
            {
                // TODO: This should create default arguments return
                throw new ArgumentOutOfRangeException(nameof(args), "No argument supplied!");
            }

            var parser = new SimpleParser(args);

            var commandTypeName = $"{parser.Command}{parser.SubCommand}";

            var argObject = FindType(argObjects, commandTypeName);

            if (argObject == null) throw new ArgumentNullException($"Type for command '{commandTypeName}' not found.");

            var reflectionParser = CreateParser(args, argObject);

            return reflectionParser;
        }

        private static void SetValues(object instance, SimpleParser args, Type argObject)
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

    public class ReflectionParser<T> : ReflectionParser
    {
        public ReflectionParser(string command, string subCommand, object data) : base(command, subCommand, data)
        {
        }

        public new T Data { get; set; }
    }
}
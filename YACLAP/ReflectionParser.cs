using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using YACLAP.Extensions;

namespace YACLAP
{ 
    public sealed class ReflectionParser<T> : ReflectionParser
    {
        public ReflectionParser(object data)
        {
            Data = (T) data;
            Validate();
        }

        public void BuildParser(string[] args, params Type[] argObjects)
        {
            if (!argObjects.Any()) throw new ArgumentOutOfRangeException(nameof(argObjects), "No argument types supplied!");
            if (!args.Any())
            {
                // TODO: This should create default arguments return
                throw new ArgumentOutOfRangeException(nameof(args), "No argument supplied!");
            }

            var parser = new SimpleParser(args);
            Command = parser.Command;
            SubCommand = parser.SubCommand;

            var commandTypeName = $"{parser.Command}{parser.SubCommand}";

            var argObject = FindType(argObjects, commandTypeName);

            if (argObject == null) throw new ArgumentNullException($"Type for command '{commandTypeName}' not found.");

            CreateArgumentObject(args, argObject);
        }

        private void CreateArgumentObject(string[] args, Type argObject)
        {
            var parsedArgs = new SimpleParser(args);

            var instance = Activator.CreateInstance(argObject);

            SetValues(instance, parsedArgs, argObject);

            Data = (T) instance;
        }

        private void SetValues(object instance, SimpleParser args, Type argObject)
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
        private Type FindType(Type[] types, string name) => types.FirstOrDefault(t => t.Name.ToLower().Contains(name.ToLower()));

        private void Validate()
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(Data, null, null);
            if (!Validator.TryValidateObject(Data, validationContext, validationResults))
            {
                // Validation failed, show errors
                Error = true;

                Errors = string.Join("\n", validationResults);
            }
        }
    }

    public class ReflectionParser 
    {
        public virtual object Data { get; set; }

        public string Command { get; set; } = "";
        public bool HasCommand => Command.Any();
        public string SubCommand { get; set; } = "";
        public bool HasSubCommand => SubCommand.Any();
        public bool Error { get; set; }
        public string Errors { get; set; }

        public static ReflectionParser CreateParser(string[] args, string argClassName, Assembly assembly = null) =>
            CreateParser(args, assembly == null 
                ? Type.GetType(argClassName) 
                : assembly.GetTypes().Single(t => t.FullName.EndsWith(argClassName)));


        public static ReflectionParser CreateParser(string[] args, object argsInstance, string command = null, string subcommand=null) => CreateReflectionParser(argsInstance);

        public static ReflectionParser CreateParser(string[] args, params Type[] argTypes)
        {
            if (!argTypes.Any()) throw new ArgumentOutOfRangeException(nameof(argTypes), "No argument types supplied!");
            if (!args.Any())
            {
                if(argTypes.Length != 1) throw new ArgumentException("A single argument class type must be supplied when no arguments are present", nameof(argTypes));

                return CreateReflectionParser(Activator.CreateInstance(argTypes[0]));
            }

            var pargs = new SimpleParser(args);
            var commandTypeName = $"{pargs.Command}{(pargs.HasSubCommand ? pargs.SubCommand : "")}";

            var argType = FindType(argTypes, commandTypeName);
            if (argType == null)
            {
                
                throw new ArgumentNullException($"Type for command '{commandTypeName}' not found in {string.Join(",", argTypes.Select(t => t.Name))}.");
            }

            var dataInstance = CreateArgumentObject(pargs, argType);

            var reflectionParser = CreateReflectionParser(dataInstance, pargs.Command, pargs.SubCommand);

            return reflectionParser;
        }

        private static ReflectionParser CreateReflectionParser(object data, string command = null, string subCommand = null)
        {
            var myGeneric = typeof(ReflectionParser<>);
            var constructedClass = myGeneric.MakeGenericType(data.GetType());


            var parser = (ReflectionParser) Activator.CreateInstance(constructedClass, Convert.ChangeType(data, data.GetType()));
            parser.Command = command;
            parser.SubCommand = subCommand;
            return parser;
        }

        private static object CreateArgumentObject(SimpleParser pargs, Type argObject)
        {
            var instance = Activator.CreateInstance(argObject);

            SetValues(instance, pargs);

            return instance;
        }

        private static void SetValues(object instance, SimpleParser args)
        {
            var argObject = instance.GetType();
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

        private static Type FindType(Type[] types, string name) 
            => types.FirstOrDefault(t => t.Name.ToLower().Contains(name.ToLower()));

    }
}
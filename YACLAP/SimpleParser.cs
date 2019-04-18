using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YACLAP.Extensions;

namespace YACLAP
{
    public interface ISimpleParser
    {
        string[] Arguments { get; }
        string Option(string name);
        bool Flag(string name);
        bool HasOption(string name);
        bool HasFlag(string optionName);
    }

    public class SimpleParser : ISimpleParser
    {
        private readonly IReadOnlyDictionary<string, string> _options;

        public SimpleParser(string[] args, bool argumentsFirst = false, string optionsPrefix = "--")
        {
            bool IsOptionOrFlag(string value) => value.StartsWith(optionsPrefix);
            bool NextTokenIsOptionOrFlag(Queue<string> queue1) => !queue1.Any() || IsOptionOrFlag(queue1.Peek());

            var queue = new Queue<string>(args);
            var arguments = new List<string>();
            var options = new Dictionary<string, string>();

            while (queue.Count > 0)
            {
                var arg = queue.Dequeue();

                if (IsOptionOrFlag(arg))
                {
                    var argName = arg.Substring(optionsPrefix.Length);
                    var argValue = NextTokenIsOptionOrFlag(queue) ? "true" : queue.Dequeue();

                    options.Add(argName, argValue);
                }
                else
                {
                    if (options.Any() && argumentsFirst)
                    {
                        throw new ArgumentException("Arguments found after an option or flag");
                    }
                    arguments.Add(arg);
                }
            }

            Arguments = arguments.ToArray();
            _options = options;
        }

        public string[] Arguments { get; }
        
        public string Option(string name) => _options[name];

        public bool Flag(string name) => _options[name].ToBool();
        public bool HasOption(string name) => _options.ContainsKey(name);
        public bool HasFlag(string name) => _options.ContainsKey(name);


        public void SetOptionAndFlagValues(object instance)
        {
            var args = this;
            var argObject = instance.GetType();
            var memberInfos = argObject.GetProperties(BindingFlags.Public | BindingFlags.Instance);
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
                    if (args.HasFlag(optionName))
                    {
                        var flag = args.Flag(optionName);
                        memberInfo.SetValue(instance, flag);
                    }
                }
                else
                {
                    if (args.HasOption(optionName))
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
        }

    }
}
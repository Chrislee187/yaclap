using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
    }

    public class ReflectionParser<T> : ReflectionParser
    {
        public ReflectionParser(string command, string subCommand, object data) : base(command, subCommand, data)
        {
        }

        public new T Data { get; set; }
    }
}
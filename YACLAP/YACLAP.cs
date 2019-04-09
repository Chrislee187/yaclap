using System;
using System.Reflection;

namespace YACLAP
{
    public static class Yaclap
    {
        public static IParser Parser(string[] args) 
            => SimpleParser.CreateParser(args);

        public static ReflectionParser Parser<T>(string[] args) 
            => ReflectionParser.CreateParser(args, typeof(T));

        public static ReflectionParser Parser(string[] args, params Type[] argTypes)
            => ReflectionParser.CreateParser(args, argTypes);
        public static ReflectionParser Parser(string[] args, string argTypeName, Assembly assembly = null)
            => ReflectionParser.CreateParser(args, argTypeName, assembly);
        public static ReflectionParser Parser(string[] args, object instance, string command = null, string subCommand = null)
            => ReflectionParser.CreateParser(args, instance);
    }
}
using System;

namespace YACLAP
{
    public static class Yaclap
    {
        public static ISimpleParser Parser(string[] args) => SimpleParser.CreateParser(args);

        public static IReflectionParser Parser<T>(string[] args) => ReflectionParser.CreateParser(args, typeof(T));

        public static IReflectionParser Parser(string[] args, params Type[] argTypes) => ReflectionParser.CreateParser(args, argTypes);
    }
}
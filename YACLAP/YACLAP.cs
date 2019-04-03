using System;

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
    }
}
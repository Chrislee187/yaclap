using System;

namespace YACLAP.Commands
{
    public interface ICommandMapper
    {
        string Name { get; }
        Type CommandType { get; }
        void MapArguments(string[] arguments, object command);
    }
}
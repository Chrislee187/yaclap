using System;

namespace YACLAP.Commands
{
    public interface ICommandMapper
    {
        string CommandName { get; }
        Type CommandType { get; }
        void MapArguments(string[] arguments, object command);
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{
    internal abstract class CommandBase
    {
        public string Help { get; set; }
        public int MinimumParameters { get; set; }
        public Type Type { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public string Identifier { get; set; }
        private List<ParameterInfo> _parameters;
        public List<ParameterInfo> Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value ?? new List<ParameterInfo>();
                int minimumParameters = 0;
                foreach (ParameterInfo parameterInfo in Parameters)
                {
                    if (!parameterInfo.IsOptional &&
                        !parameterInfo.ParameterType.IsNullable() && !parameterInfo.ParameterType.IsIList())
                    {
                        minimumParameters++;
                    }
                }
                MinimumParameters = minimumParameters;
            }
        }

        public MatchState Match(List<string> arguments)
        {
            string currentSearch = string.Empty;
            foreach (string argument in arguments)
            {
                string firstSpace = string.IsNullOrEmpty(currentSearch) ? "" : " ";
                currentSearch += $"{firstSpace}{argument.ToLowerInvariant()}";
                if (currentSearch == Identifier)
                {
                    var argumentsWithoutIdentifiers = ArgumentsWithoutIdentifier(arguments);
                    
                    if (MinimumParameters > argumentsWithoutIdentifiers.Count)
                    {
                        return MatchState.MissingParameter;
                    }

                    if (Parameters.Any() &&
                        !Parameters.LastOrDefault()
                            .ParameterType.IsIList() && Parameters.Count < argumentsWithoutIdentifiers.Count)
                    {
                        return MatchState.TooManyArguments;
                    }
                    return MatchState.Matched;
                }
            }
            return MatchState.Miss;
        }

        public List<string> ArgumentsWithoutIdentifier(List<string> arguments)
        {
            var possibleArguments = arguments.Skip(Identifier.Split(' ').Length).ToList();
            return possibleArguments;
        }

        public bool Invoke(object instance, object[] parameters)
        {
            if (MethodInfo != null)
            {
                try
                {
                    MethodInfo.Invoke(instance, parameters);
                    return true;
                }
                catch (TargetInvocationException exception) when(exception.InnerException != null)
                {
                    ConsoleWrite.WriteErrorLine(
                        $"Exception occured while executing the command '{Identifier}'");
                    ConsoleWrite.WriteErrorLine($"{exception.InnerException.GetType()}: {exception.InnerException.Message}");
                    ConsoleWrite.WriteErrorLine(exception.StackTrace);
                    return false;
                }
                catch (Exception exception)
                {
                    ConsoleWrite.WriteErrorLine(
                        $"Exception occured while executing the command {Identifier}: {exception.GetType()}: {exception.Message}");
                    ConsoleWrite.WriteErrorLine(exception.StackTrace);
                    return false;
                }
            }
            return false;
        }
        public override string ToString()
        {
            return Identifier;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandRunner
{
    internal abstract class BaseRunner
    {
        internal readonly State State;

        internal BaseRunner(State state)
        {
            State = state;
        }

        internal Tuple<ICommand, MatchState> Match(List<string> arguments)
        {
            var matches = State.ActiveMenu.Select(x => new { Key = x, Value = x.Match(arguments) })
                .Where(x => x.Value != MatchState.Miss)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            if (!matches.Any())
            {
                ConsoleWrite.WriteErrorLine(ErrorMessages.NoMatch);
                return null;
            }
            if (matches.Count > 1)
            {
                ConsoleWrite.WriteErrorLine(ErrorMessages.TooManyMatches);
                return null;
            }
            var match = matches.Single();
            if (match.Value == MatchState.MissingParameter)
            {
                ConsoleWrite.WriteErrorLine(ErrorMessages.MissingArgument);
                match.Key.WriteToConsole();
            }
            else if (match.Value == MatchState.WrongTypes)
            {
                ConsoleWrite.WriteErrorLine(ErrorMessages.WrongTypes);
            }
            return new Tuple<ICommand, MatchState>(match.Key, match.Value);
        }

        internal void ProcessMatches(Dictionary<ICommand, MatchState> matches, List<string> arguments, Action<ICommand, List<string>> functionOnMatch)
        {
            foreach (KeyValuePair<ICommand, MatchState> match in matches)
            {
                if (match.Value == MatchState.MissingParameter)
                {
                    ConsoleWrite.WriteErrorLine(ErrorMessages.MissingArgument);
                    match.Key.WriteToConsole();
                }
                else if (match.Value == MatchState.TooManyArguments)
                {
                    ConsoleWrite.WriteErrorLine(ErrorMessages.TooManyArguments);
                    match.Key.WriteToConsole();
                }
                else if (match.Value == MatchState.WrongTypes)
                {
                    ConsoleWrite.WriteErrorLine(ErrorMessages.WrongTypes);
                }
                else if (match.Value == MatchState.Matched)
                {
                    functionOnMatch(match.Key, arguments);
                }
            }
        }
        internal bool ExecuteCommand(ICommand command, List<string> arguments)
        {
            try
            {
                var commandInstance = State.StatefullCommandActivator(command.Type);
                Console.ForegroundColor = State.CommandColor;
                var navigatableCommand = command as NavigatableCommand;
                if (command.Parameters.Count > 0)
                {
                    var typedParameters =
                        TypedParameterExecution.CreateTypedParameters(command.Parameters.ToArray(),
                            command.ArgumentsWithoutIdentifier(arguments));
                    if (typedParameters.Length != command.Parameters.Count)
                    {
                        return false;
                    }
                    command.MethodInfo.Invoke(commandInstance, typedParameters);

                }
                else
                {
                    //Navigation commands don't always have an initialize method
                    if (navigatableCommand != null)
                    {
                        navigatableCommand.MethodInfo?.Invoke(commandInstance, null);
                    }
                    else
                    {
                        command.MethodInfo.Invoke(commandInstance, null);
                    }
                }

                if (navigatableCommand != null)
                {
                    SetMenu(navigatableCommand, commandInstance);
                }
                return true;
            }
            catch (Exception exception)
            {
                ConsoleWrite.WriteErrorLine($"We couldn't setup your command parameters. Exception: {exception.Message}");
                return false;
            }
            finally
            {
                Console.ForegroundColor = State.StartupColor;
            }
        }

        internal abstract void SetMenu(NavigatableCommand command, object commandInstance);
    }
}

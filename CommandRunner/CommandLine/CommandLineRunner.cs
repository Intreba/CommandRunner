using System;
using System.Linq;

namespace CommandRunner.CommandLine
{
    internal class CommandLineRunner : BaseRunner, IStartableRunner {
        private readonly CommandLineState _state;
        internal CommandLineRunner(CommandLineState state) : base(state)
        {
            _state = state;
        }
        public void Start()
        {
            if (!_state.Arguments.Any())
            {
                ConsoleWrite.WriteErrorLine(ErrorMessages.NoArgumentsProvided);
                return;
            }
            if (_state.InHelpMode)
            {
                Console.ForegroundColor = _state.TerminalColor;
                HelpPrinter.PrintHelp(_state.Title, _state.FullMenu);
                Console.ForegroundColor = _state.StartupColor;
                return;
            }
            FindAndExecuteCommand();
        }

        private void FindAndExecuteCommand()
        {
            var match = Match(_state.Arguments);
            if (match == null) return;
            var navigatableCommand = match.Item1 as NavigatableCommand;
            if (navigatableCommand != null)
            {
                ExecuteCommand(match.Item1, _state.Arguments);
                _state.Arguments = _state.Arguments.Skip(navigatableCommand.Identifier.Split(' ').Length)
                    .Skip(navigatableCommand.MinimumParameters).ToList();
                FindAndExecuteCommand();
            }
            else
            {
                if (match.Item2 == MatchState.TooManyArguments)
                {
                    ConsoleWrite.WriteErrorLine(ErrorMessages.TooManyArguments);
                    match.Item1.WriteToConsole();
                }
                else if (match.Item2 == MatchState.Matched)
                {
                    ExecuteCommand(match.Item1, _state.Arguments);
                }
            }
        }

        internal override void SetMenu(NavigatableCommand command)
        {
            State.ActiveMenu = command.SubItems;
        }
    }
}
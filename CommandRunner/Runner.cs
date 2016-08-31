﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandRunner
{    
    public class Runner
    {
        private readonly RunnerSettings _settings;
        public Runner()
        {
            _settings = new RunnerSettings
            {
                Title = "Command Runner",
                ScanAllAssemblies = true,
                ReflectionActivator = true
            };
        }
        public Runner(Action<ICustomizableRunnerConfiguration> configure)
        {
            if(configure == null) throw new ArgumentNullException(nameof(configure));

            _settings = new RunnerSettings();
            configure(_settings);
        }
        public void Run()
        {
            SetupConsoleTitle();
            SetupMenu();
            DeciceMode();
            Execute();
        }
        private void SetupConsoleTitle()
        {
            if (!string.IsNullOrWhiteSpace(_settings.Title))
            {
                Console.Title = _settings.Title;
            }
        }
        private void SetupMenu()
        {
            if (_settings.Menu.Any())
            {
                return;
            }
            var commandMethods = CommandMethodReflector.GetCommandMethods(_settings)?.ToList();
            var menu = new List<IMenuItem>();
            if (commandMethods == null)
            {
                Console.WriteLine("Please make sure to setup command scanning or provide your own commands.");
            }
            else if (commandMethods.Count == 0)
            {
                Console.WriteLine("No commands found.");
                return;
            }
            else
            {
                menu.AddRange(MenuCreator.CreateMenuItems(commandMethods, _settings));
            }
            _settings.Menu = menu;
        }
        private void DeciceMode()
        {
            if (_settings.ForceCommandLine)
            {
                _settings.Mode = RunMode.CommandLine;
            }
            else if (_settings.ForceTerminal)
            {
                _settings.Mode = RunMode.Terminal;
            }
            else
            {
                var args = Environment.GetCommandLineArgs();
                if (args.Length == 0 || (args.FirstOrDefault() == Assembly.GetEntryAssembly().Location))
                {
                    _settings.Mode = RunMode.Terminal;
                }
                else
                {
                    _settings.Mode = RunMode.CommandLine;
                }
            }
        }
        private void Execute()
        {
            if (_settings.Mode == RunMode.CommandLine)
            {
                RunCommandLineMode();
            }
            else if (_settings.Mode == RunMode.Terminal)
            {
                RunTerminalMode();
            }
        }
        private void RunCommandLineMode()
        {
            var arguments = Environment.GetCommandLineArgs().ToList();
            var commandWithArgs = InputParser.FindCommand(_settings.Menu.OfType<ICommand>(), arguments);
            commandWithArgs.Item1.Execute(commandWithArgs.Item2.ToList());
            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
        }
        private void RunTerminalMode()
        {
            var menuItemList = _settings.Menu;
            if (!menuItemList.Any())
            {
                Console.WriteLine("Please add commands to add functionality.");
                return;
            }
            string input;
            do
            {
                Console.WriteLine($"Available commands:");
                menuItemList = menuItemList.OrderBy(x => x.Title).ToList();
                var groupedMenuItems = menuItemList.GroupBy(x => x.Title.Split(' ')[0]);
                foreach (IGrouping<string, IMenuItem> groupedMenuItem in groupedMenuItems)
                {
                    Console.WriteLine();
                    foreach (IMenuItem menuItem in groupedMenuItem)
                    {
                        if (menuItem is ContainerCommand)
                        {
                            Console.WriteLine($"  {menuItem.Title.ToLowerInvariant()} {menuItem.Help}");
                        }
                        else
                        {
                            Console.WriteLine($"  {menuItem.Title.ToLowerInvariant()}: {menuItem.Help}");
                        }
                    }
                }

                Console.Write($"{Environment.NewLine}Command> ");
                input = Console.ReadLine() ?? string.Empty;
                var commandWithArgs = InputParser.FindCommand(menuItemList.OfType<ICommand>(), InputParser.ParseInputToArguments(input));
                if (commandWithArgs != null)
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        commandWithArgs.Item1.Execute(commandWithArgs.Item2.ToList());
                        Console.ResetColor();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.ToString());
                    }
                    Console.WriteLine();
                }
            } while (string.IsNullOrEmpty(input) || !input.Equals("EXIT", StringComparison.OrdinalIgnoreCase));
        }
    }
}
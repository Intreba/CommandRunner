﻿using System.Collections.Generic;

namespace CommandRunner
{
    /// <summary>
    /// Implement this interface to enable your commands.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Method called when command should be executed.
        /// </summary>
        /// <param name="args">The arguments a user provied, minus the Command itself</param>
        void Execute(IEnumerable<string> args);
        /// <summary>
        /// A simple help to document all different sub-commands so users can easily see which parameters should be used and how.
        /// </summary>
        IEnumerable<string> Help { get; }
        /// <summary>
        /// The command identifier.
        /// </summary>
        string Command { get; }
    }
}
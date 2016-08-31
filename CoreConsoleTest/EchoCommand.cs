﻿using System;
using System.Collections.Generic;

namespace CommandRunner.CoreConsoleTest
{
    public class EchoCommand
    {

        public Injectable Injectable { get; set; }

        [Command("echo", "Echo back anything following the command.")]
        public void Execute(List<string> args)
        {
            foreach (var arg in args) Console.WriteLine(arg);
        }
    }
}

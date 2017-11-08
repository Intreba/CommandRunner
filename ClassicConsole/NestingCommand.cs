﻿using System;

namespace CommandRunner.ClassicConsole
{
    [Command("nest", "Commands can nest too.")]
    public class NestingCommand
    {
        [Command("hello", "Say hello")]
        public void Hello()
        {
            Console.WriteLine("Hello");
        }

        [Command("timer")]
        public void Timer(TimeSpan span)
        {
            Console.WriteLine(span);
        }
    }
}
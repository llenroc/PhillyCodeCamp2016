﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoundRobinWordCounter.Readers
{
    public class ConsoleReader : IReadStuff
    {
        public String ReadLine()
        {
            return Console.ReadLine();
        }
    }
}

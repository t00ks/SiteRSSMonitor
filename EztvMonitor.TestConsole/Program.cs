using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EztvMonitor.Core;

namespace EztvMonitor.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var processor = new Processor();
            processor.Start();

            Console.ReadLine();

            processor.Stop();
        }
    }
}

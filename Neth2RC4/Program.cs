using System;
using System.Diagnostics;
using System.IO;

namespace Neth2RC4
{
    class Program
    {
        static void Main(string[] args)
        {
            MultiplyTest test = new MultiplyTest();
            test.Run().Wait();
        }

    }
}
using System;

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
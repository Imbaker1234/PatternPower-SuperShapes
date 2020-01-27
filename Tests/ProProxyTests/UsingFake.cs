using System;

namespace ProProxyTests
{
    public class UsingFake : IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("Dispose called");
        }
    }
}
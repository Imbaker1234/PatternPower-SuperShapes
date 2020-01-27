using System;

namespace ProProxyTests
{
    public class Fake : IFake
    {
        public void Speak(string words)
        {
            Console.WriteLine($"Speak: {words}");
        }

        public string Listen(string words)
        {
            Console.WriteLine($"Listen: {words}");
            return words.ToUpper();
        }

        public int Calculate(int a, int b, int c)
        {
            var calculate = a * b * c;
            Console.WriteLine($"Calculate: {calculate}");
            return calculate;
        }
    }
}
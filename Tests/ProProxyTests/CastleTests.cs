using System;
using System.Diagnostics;
using FluentAssertions;
using ImpromptuInterface;
using NUnit.Framework;
using ProProxy.Castle;
using ProProxy.Proxies;
using ProProxy.Shells;

namespace ProProxyTests
{
    [TestFixture]
    public class CastleTests
    {
        [Test]
        public void CastleProxyWillPairNicelyWithImpromptuInterface()
        {
            var proxy = new ProxyGenerator()
                .CreateInterfaceProxyWithTarget(typeof(IJimJam), new JimJam(), new Interceptor()).ActLike<IJimJam>();

            proxy.Calculate(2, 4, 10);

            proxy.Listen("KingKrab");
        }

        [Test]
        public void CastleVsProProxy()
        {
            var castleProxy = new ProxyGenerator()
                .CreateInterfaceProxyWithTarget(typeof(IJimJam), new JimJam(), new Interceptor()).ActLike<IJimJam>();

            var proProxy = LogProxy<JimJam>.As<IJimJam>((s, b, args) => 
                Console.WriteLine($"Before target call {b.Name}"), 
                (s, b, args) => Console.WriteLine($"After target call {b.Name}"),
                null);

            var swCastle = new Stopwatch();

            swCastle.Start();
            for (int i = 0; i < 10000; i++)
            {
                castleProxy.Calculate(5, 10, 3);
            }
            swCastle.Stop();

            var swPro = new Stopwatch();

            swPro.Start();
            for (int i = 0; i < 10000; i++)
            {
                proProxy.Calculate(5, 10, 3);
            }
            swPro.Stop();
            Console.WriteLine($"ProProxy {swPro.ElapsedTicks}");
            Console.WriteLine($"CastleProxy {swCastle.ElapsedTicks}");
            swPro.ElapsedTicks.Should().BeLessThan(swCastle.ElapsedTicks);
        }

        [Test]
        public void MultipleProxyBehavior()
        {
            var ds = new DecoratorShell(() => Console.WriteLine("Entering A"), () => Console.WriteLine("Exiting A"), null);
            
            var jj = DecoratorProxy<JimJam>.As<IJimJam>(ds, null);
            var kk = DecoratorProxy<DecoratorProxy<JimJam>>.As<IJimJam>(ds, jj);
        }
    }


    public interface IJimJam
    {
        void Speak(string words);
        string Listen(string words);
        int Calculate(int a, int b, int c);
    }

    public class JimJam : IJimJam
    {
        public void Speak(string words)
        {
            Console.WriteLine(words);
        }

        public string Listen(string words)
        {
            Console.WriteLine($"I hear {words}");
            return words;
        }

        public int Calculate(int a, int b, int c)
        {
            var result = a * b * c;
            Console.WriteLine(result);
            return result;
        }
    }
}
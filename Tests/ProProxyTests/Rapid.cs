using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Dynamitey;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ProProxy.Decorator;
using ProProxy.Proxies;
using ProProxy.Shells;
using ProProxy.Using;

namespace ProProxyTests
{
    [TestFixture]
    public class Rapid
    {
        [Test]
        public void UsingProxyDemo()
        {
            var shell = new UsingShell<UsingFake>(() => new UsingFake());
            var demo = UsingProxy<Fake, UsingFake>.As<IFake>(shell, new Fake());

            Console.WriteLine(demo.Calculate(3, 4, 10));
            Console.WriteLine(demo.Listen("KingKrab"));
            demo.Speak("Frozen");
        }

        [Test]
        public void DecoratorProxyDemo()
        {
            var shell = new DecoratorShell(
                () => Console.WriteLine("PreAction"),
                () => Console.WriteLine("PostAction"),
                e => Console.WriteLine($"Failure: {e.Message}"));

            var demo = DecoratorProxy<Fake>.As<IFake>(shell, new Fake());

            Console.WriteLine(demo.Calculate(5, 21, 32));
            Console.WriteLine(demo.Listen("Listening"));
            demo.Speak("Olaf");
        }

        [Test]
        public void LogProxyDemo()
        {
            var
        }
    }
}
using System;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using ProProxy;
using ProProxy.Proxies;
using ProProxy.Shells;
using static System.Console;

namespace ProProxyTests
{
    [TestFixture]
    public class DecoratorProxyTests
    {
        [Test]
        public void DecoratorProxyWillPerformActionsBeforeAndAfter()
        {
            var ds = new DecoratorShell(() => WriteLine("About to Write Something"),
                () => WriteLine($"Hey look at what I wrote! {Environment.NewLine}"),
                e => WriteLine(e));

            int tracker = 0;

            var writer =
                DecoratorProxy<Writer>.As<IWriter>(ds);

            var bigWriter =
                DecoratorProxy<BigWriter>.As<IBigWriter>(ds);


            writer.Write("LogWriter: KingKrab");

            bigWriter.BigWrite("BigWriter: Goldfox");

            writer.Write("LogWriter: Has MonsterClaws");

            bigWriter.BigWrite("BigWriter: Has a fiery gaze");
        }
    }

    public class Writer : IWriter
    {
        public void Write(string text) => WriteLine(text);
    }

    public interface IWriter
    {
        void Write(string text);
    }

    public class BigWriter
    {
        public void BigWrite(string text) => WriteLine(text.ToUpper());
    }

    public interface IBigWriter
    {
        void BigWrite(string text);
    }
}
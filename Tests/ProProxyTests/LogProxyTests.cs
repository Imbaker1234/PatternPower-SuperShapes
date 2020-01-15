using System;
using NUnit.Framework;
using ProProxy;
using static System.Console;

namespace ProProxyTests
{
    [TestFixture]
    public class LogProxyTests
    {
        [Test]
        public void LogProxyWillLogBeforeAndAfterTests()
        {
            var loggedWriter =
                LogProxy<LogWriter>.As<ILogWriter>(new LogWriter(),
                    (b, args) => WriteLine($"{DateTime.Now} -- Entering {b.Name}({string.Join(",", args)})"),
                    (b, args) => WriteLine($"{DateTime.Now} -- Exiting LogWriter.{b.Name}"),
                    (b, args, e) =>
                        WriteLine(
                            $"{DateTime.Now} -- EXCEPTION: LogWriter.{b.Name}({string.Join(",", args)}: {e.Message}"));

            loggedWriter.Write("Writing to file");

            WriteLine();

            loggedWriter.Write("Writing to another file");
        }

        public class LogWriter
        {
            public void Write(string text) => WriteLine(text);
        }

        public interface ILogWriter
        {
            void Write(string text);
        }
    }
}

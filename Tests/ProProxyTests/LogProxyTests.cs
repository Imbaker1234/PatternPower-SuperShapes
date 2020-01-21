using System;
using System.Text;
using NUnit.Framework;
using ProProxy;
using ProProxy.Proxies;
using ProProxy.Shells;
using static System.Console;

namespace ProProxyTests
{
    [TestFixture]
    public class LogProxyTests
    {
        [Test]
        public void LogProxyWillLogBeforeAndAfterTests()
        {
            var ls = new LogShell(
                (type, b, args) => WriteLine($"{DateTime.Now} -- Entering {type}{b.Name}({string.Join(",", args)})"),
                (type, b, args) => WriteLine($"{DateTime.Now} -- Exiting {type}.{b.Name}"),
                (type, b, args, e) =>
                    WriteLine(
                        $"{DateTime.Now} -- EXCEPTION: {type}.{b.Name}({string.Join(",", args)}: {e.Message}"));

            var loggedWriter =
                ReflectiveLogProxy<LogWriter>.As<ILogWriter>(ls);

            loggedWriter.Write("Writing to file");

            WriteLine();

            loggedWriter.Write("Writing to another file");
        }

        [Test]
        public void Demo()
        {
            var ls = new LogShell(
                (type, b, args) => WriteLine($"{DateTime.Now} -- Entering {type}{b.Name}({string.Join(",", args)})"),
                (type, b, args) => WriteLine($"{DateTime.Now} -- Exiting {type}.{b.Name}"),
                (type, b, args, e) =>
                    WriteLine(
                        $"{DateTime.Now} -- EXCEPTION: {type}.{b.Name}({string.Join(",", args)}: {e.Message}"));

            var regularWriter = new LogWriter();
            var loggedWriter = LogProxy<LogWriter>.As<ILogWriter>(ls);

            regularWriter.WriteMultiple("Regular Writer", 32, DateTime.Now, 'a');
            loggedWriter.WriteMultiple("Logged Writer", 21, DateTime.Now.AddDays(-1), 'b');
        }
    }

    public class LogWriter
    {
        public void Write(string text) => WriteLine(text);

        public string WriteMultiple(string text, int num, DateTime today, char grade, params string[] additionalStrings)
        {
            var sb = new StringBuilder("\n");
            sb.Append($"text: {text} \n");
            sb.Append($"num: {num} \n");
            sb.Append($"today: {DateTime.Now} \n");
            sb.Append($"grade: '{grade}' \n");

            for (int i = 0; i < additionalStrings.Length; i++)
            {
                sb.Append($"Additional String {i}: {additionalStrings[i]}\n");
            }

            Console.WriteLine(sb);
            return sb.ToString();
        }
    }

    public interface ILogWriter
    {
        void Write(string text);
        string WriteMultiple(string text, int num, DateTime today, char grade, params string[] additionalStrings);
    }
}
using System;
using System.Text;
using Dynamitey;
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

            var loggedWriter = LogProxy<LogWriter>
                .As<ILogWriter>(ls);

            regularWriter.Calculate(10, 4,1,3,5,6);
            loggedWriter.Write(loggedWriter.Calculate(5, 4, 3, 2,6,8).ToString());
            loggedWriter.Write(loggedWriter.Calculate(5, 2,7,8,6,4).ToString());

            regularWriter.WriteMultiple("Krab", 32, DateTime.Now, 'a', "1wq", "23w", "45t");
            loggedWriter.WriteMultiple("King", 21, DateTime.Now.AddDays(-1), 'b', "asd", "qaz", "123");
        }
    }

    public class LogWriter
    {
        public void Write(string text) => WriteLine(text);
        public int Calculate(int a, int b, int c, int d, int e, int f) => a * b * c + d + e + f;
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

            WriteLine(sb);
            return sb.ToString();
        }
    }

    public interface ILogWriter
    {
        void Write(string text);
        int Calculate(int a, int b, int c, int d, int e, int f);
        string WriteMultiple(string text, int num, DateTime today, char grade, params string[] additionalStrings);
    }
}
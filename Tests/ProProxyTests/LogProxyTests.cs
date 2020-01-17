using System;
using System.Data;
using FastMember;
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
            var ls = new LogShell((type, b, args) => WriteLine($"{DateTime.Now} -- Entering {type}{b.Name}({string.Join(",", args)})"),
                    (type, b, args) => WriteLine($"{DateTime.Now} -- Exiting {type}.{b.Name}"),
                    (type, b, args, e) =>
                        WriteLine(
                            $"{DateTime.Now} -- EXCEPTION: {type}.{b.Name}({string.Join(",", args)}: {e.Message}"));

            // var loggedWriter =
            //     LogProxy<LogWriter>.As<ILogWriter>(new LogWriter(),
            //         loggedWriter.Write("Writing to file");

            WriteLine();

            // loggedWriter.Write("Writing to another file");
        }

        [Test]
        public void FastMember()
        {
            var receipt = new Receipt
            {
                Amount = 108,
                DateOfPurchase = DateTime.Now,
                Name = "KingKrab"
            };

            var accessor = TypeAccessor.Create(receipt.GetType());

            WriteLine(accessor[receipt, "Amount"]);

            accessor[receipt, "Amount"] = 200;

            WriteLine(accessor[receipt, "Amount"]);
        }


        [Test]
        public void PrintMembers()
        {
            var receipt = new Receipt
            {
                Amount = 108,
                DateOfPurchase = DateTime.Now,
                Name = "KingKrab"
            };

            var accessor = TypeAccessor.Create(receipt.GetType());

            foreach (var member in accessor.GetMembers())
            {
                Console.WriteLine(member.Name);
            }

        }
    }

    public class Receipt
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public DateTime DateOfPurchase { get; set; }
        
        public void UpdateDateOfPurchase(int days)
        {
            DateOfPurchase.AddDays(days);
        }

        public void UpdateName(string newName)
        {
            Name = newName;
        }

        public void UpdateAmount(int newAmount)
        {
            Amount = newAmount;
        }
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
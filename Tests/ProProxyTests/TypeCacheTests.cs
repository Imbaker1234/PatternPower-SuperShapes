using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using ProProxy;
using FluentAssertions;
using static System.Console;

namespace ProProxyTests
{
    [TestFixture]
    public class TypeCacheTests
    {
        [Test]
        public void TypeCache_Actions_Will_Be_More_Performant_Than_Reflection()
        {
            var ls = new LogShell(null, null, null);
            var nonProxy = new Dummy();
            var proProxy = LogProxy<Dummy>.As<IDummy>(ls);
            var reflectiveProxy = ReflectiveLogProxy<Dummy>.As<IDummy>(ls);

            var nonSw = new Stopwatch();

            proProxy.Speak("Warmup");
            reflectiveProxy.Speak("Warmup");

            int testCount = 1000;

            nonSw.Start();
            for (int i = 0; i < testCount; i++)
            {
                nonProxy.Speak("One");
                nonProxy.Speak("Two");
                nonProxy.Speak("Three");
            }

            nonSw.Stop();

            var nonProxyTime = nonSw.Elapsed;

            var proSw = new Stopwatch();

            proSw.Start();
            for (int i = 0; i < testCount; i++)
            {
                proProxy.Speak("One");
                proProxy.Speak("Two");
                proProxy.Speak("Three");
            }

            proSw.Stop();
            var proproxyTime = proSw.Elapsed;

            var refSw = new Stopwatch();

            refSw.Start();
            for (int i = 0; i < testCount; i++)
            {
                reflectiveProxy.Speak("One");
                reflectiveProxy.Speak("Two");
                reflectiveProxy.Speak("Three");
            }

            refSw.Stop();
            var reflectiveProxyTime = refSw.Elapsed;

            Console.WriteLine($"NonProxy: {nonProxyTime}");
            Console.WriteLine($"ProProxy: {proproxyTime}");
            Console.WriteLine($"ReflectiveProxy: {reflectiveProxyTime}");

            nonProxyTime.Should().BeLessThan(proproxyTime).And.BeLessThan(reflectiveProxyTime);
            proproxyTime.Should().BeGreaterThan(nonProxyTime).And.BeLessThan(reflectiveProxyTime);
            reflectiveProxyTime.Should().BeGreaterThan(nonProxyTime).And.BeGreaterThan(proproxyTime);
        }

        [Test]
        public void TypeCache_Funcs_With_Randomized_Returns_Will_Be_More_Performant_Than_Reflection()
        {
            var nonProxyTimes = new List<double>();
            var proProxyTimes = new List<double>();
            var reflectiveProxyTimes = new List<double>();

            for (int j = 0; j < 10; j++)
            {
//                var ls = new LogShell(
//                    (type, b, args) =>
//                        WriteLine($"{DateTime.Now} -- Entering {type}{b.Name}({string.Join(",", args)})"),
//                    (type, b, args) => WriteLine($"{DateTime.Now} -- Exiting {type}.{b.Name}"),
//                    (type, b, args, e) =>
//                        WriteLine(
//                            $"{DateTime.Now} -- EXCEPTION: {type}.{b.Name}({string.Join(",", args)}: {e.Message}"));

                var ls = new LogShell(null, null, null);
                var nonProxy = new Dummy();
                var proProxy = LogProxy<Dummy>.As<IDummy>(ls);
                var reflectiveProxy = ReflectiveLogProxy<Dummy>.As<IDummy>(ls);

                var nonSw = new Stopwatch();

                nonProxy.Listen("Warmup");
                proProxy.Listen("Warmup");
                reflectiveProxy.Listen("Warmup");

                Random r = new Random();
                int testCount = 1000;

                nonSw.Start();
                for (int i = 0; i < testCount; i++)
                {
                    nonProxy.Listen(r.NextDouble().ToString());
                    nonProxy.Listen(r.NextDouble().ToString());
                    nonProxy.Listen(r.NextDouble().ToString());
                }

                nonSw.Stop();

                var nonProxyTime = nonSw.ElapsedMilliseconds;

                var proSw = new Stopwatch();

                proSw.Start();
                for (int i = 0; i < testCount; i++)
                {
                    proProxy.Listen(r.NextDouble().ToString());
                    proProxy.Listen(r.NextDouble().ToString());
                    proProxy.Listen(r.NextDouble().ToString());
                }

                proSw.Stop();
                var proproxyTime = proSw.ElapsedMilliseconds;

                var refSw = new Stopwatch();

                refSw.Start();
                for (int i = 0; i < testCount; i++)
                {
                    reflectiveProxy.Listen(r.NextDouble().ToString());
                    reflectiveProxy.Listen(r.NextDouble().ToString());
                    reflectiveProxy.Listen(r.NextDouble().ToString());
                }

                refSw.Stop();
                var reflectiveProxyTime = refSw.ElapsedMilliseconds;

                nonProxyTimes.Add(nonProxyTime);
                proProxyTimes.Add(proproxyTime);
                reflectiveProxyTimes.Add(reflectiveProxyTime);
            }

            WriteLine($"NonProxy {nonProxyTimes.Average()}");
            WriteLine($"ProProxy {proProxyTimes.Average()}");
            WriteLine($"ReflectiveProxy {reflectiveProxyTimes.Average()}");

            proProxyTimes.Average().Should().BeGreaterThan(nonProxyTimes.Average()).And
                .BeLessThan(reflectiveProxyTimes.Average());
        }

        [Test]
        public void Properties_Will_Be_Gettable()
        {
            var ls = new LogShell(
                (type, b, args) =>
                    WriteLine($"{DateTime.Now} -- Entering {type}{b.Name}({string.Join(",", args)})"),
                (type, b, args) => WriteLine($"{DateTime.Now} -- Exiting {type}.{b.Name}"),
                (type, b, args, e) =>
                    WriteLine(
                        $"{DateTime.Now} -- EXCEPTION: {type}.{b.Name}({string.Join(",", args)}: {e.Message}"));

            var proProxy = LogProxy<Dummy>.As<IDummy>(ls);

            var sw = new Stopwatch();

            sw.Start();
            proProxy.Name = "KingKrab";

            Console.WriteLine(proProxy.Name);
            Console.WriteLine(sw.ElapsedTicks);
        }

        [Test]
        public void Perception()
        {
            var d = new Dummy();

            var sw = new Stopwatch();

            sw.Start();
            d.Name = "KingKrab";

            Console.WriteLine(d.Name);
            Console.WriteLine(sw.ElapsedTicks);
        }
    }

    public class Dummy
    {
        public string Name { get; set; }
        public string FavoriteFood { get; set; }
        public int Age { get; set; }

        public void Speak(string text)
        {
            WriteLine(text);
        }

        public string Listen(string text)
        {
            return text.ToUpper();
        }
    }

    public interface IDummy
    {
        string Name { get; set; }
        string FavoriteFood { get; set; }
        int Age { get; set; }
        void Speak(string text);
        string Listen(string text);
    }
}
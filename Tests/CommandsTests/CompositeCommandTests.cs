using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Commands;
using FluentAssertions;
using NUnit.Framework;
using Prototype;

namespace CommandsTests
{
    [TestFixture]
    public class CompositeCommandTests
    {

        /// <summary>
        /// Checks the status of the array to determine which ones have been flagged vs which ones
        /// have not. Allowing us to ensure that commands are firing in the order in which they were
        /// added to the CompositeCommand.
        /// </summary>
        private static readonly Action<Dictionary<int, bool>, int> Call = (b, i) => { b[i] = true; };

        private static readonly Action<Dictionary<int, bool>, int> Undo = (b, i) => { b[i] = false; };

        private static readonly Action<Exception> WriteLineOnFailure = e => Console.WriteLine(e.Message);

        [Test]
        public void All_Commands_Will_Fire_In_Sequence()
        {
            var flags = GetNewFlags();

            var commands = new List<ICommand>();
            commands.Add(new Command(() => Call(flags, 0), () => Undo(flags, 0), WriteLineOnFailure));
            commands.Add(new Command(() => Call(flags, 1), () => Undo(flags, 1), WriteLineOnFailure));
            commands.Add(new Command(() => Call(flags, 2), () => Undo(flags, 2), WriteLineOnFailure));
            commands.Add(new Command(() => Call(flags, 3), () => Undo(flags, 3), WriteLineOnFailure));

            var sut = new CompositeCommand(commands.ToArray());

            sut.Success.Should().BeFalse();
            sut.Call();
            sut.Success.Should().BeTrue();
        }

        [Test]
        public void Commands_Will_Halt_Execution_On_Failure_With_Static_Defaults()
        {
            var flags = GetNewFlags();
            var commands = new List<ICommand>();
            commands.Add(new Command(() => Call(flags, 0), () => Undo(flags, 0), WriteLineOnFailure));
            commands.Add(new Command(() => Call(flags, 1), () => Undo(flags, 1), WriteLineOnFailure));
            commands.Add(new Command(
                () => throw new Exception("This is an expected exception"),
                () => Console.WriteLine("Nothing to undo"),
                e => WriteLineOnFailure(e)));
            commands.Add(new Command(() => Call(flags, 3), () => Undo(flags, 3), WriteLineOnFailure));

            var sut = new CompositeCommand(commands.ToArray());

            flags.Should().BeEquivalentTo(new Dictionary<int, bool>()
            {
                {0, false},
                {1, false},
                {2, false},
                {3, false}
            }, "We haven't invoked Call() yet.");
            sut.Success.Should().BeFalse("We haven't invoked Call() yet.");

            sut.Call();

            flags.Should().BeEquivalentTo(new Dictionary<int, bool>()
            {
                {0, true},
                {1, true},
                {2, false},
                {3, false}
            }, "We should have made it as far as the second command before the failure");
        }

        [Test]
        public void Commands_Will_Continue_After_Failure()
        {
            var exceptionsThrown = new List<Exception>();

            var faultyCommand = new Command(
                () => throw new Exception($"Throwing exception #{exceptionsThrown.Count + 1}"),
                () => Console.WriteLine("Nothing to undo"),
                e =>
                {
                    exceptionsThrown.Add(e);
                    Console.WriteLine(e.Message);
                });


            var flags = GetNewFlags();

            var commands = new List<ICommand>();
            commands.Add(new Command(() => Call(flags, 0), () => Undo(flags, 0), WriteLineOnFailure));
            commands.Add(faultyCommand);
            commands.Add(new Command(() => Call(flags, 2), () => Undo(flags, 2), WriteLineOnFailure));
            commands.Add(faultyCommand);

            var sut = new CompositeCommand(commands.ToArray());

            sut.ContinueAfterFailure = true;

            sut.UndoOnFailure.Should().Be(false, "Setting Continue after failure should set this to false since " +
                                                        "they can both be false but neither can be true at the same time.");

            flags.Should().BeEquivalentTo(new Dictionary<int, bool>()
            {
                {0, false},
                {1, false},
                {2, false},
                {3, false}
            }, "We haven't invoked Call() yet");

            sut.Success.Should().BeFalse("We haven't invoked Call() yet.");

            sut.Call();

            flags.Should().BeEquivalentTo(new Dictionary<int, bool>()
            {
                {0, true},
                {1, false},
                {2, true},
                {3, false}
            }, "Commands 0 and 2 use a faulty command which should fail to set these flags to true.");

            exceptionsThrown.Count.Should().Be(2, "Commands 0 and 2 are faulty and should each throw an exception.");
        }

        private Dictionary<int, bool> GetNewFlags()
        {
            return new Dictionary<int, bool>()
            {
                {0, false },
                {1, false },
                {2, false },
                {3, false },
            };
        }

        [Test]
        public void Commands_Will_Fully_Undo_After_Failure()
        {
            var flags = GetNewFlags();

            var faultyCommand = new Command(
                () =>
                {
                    flags.Should().BeEquivalentTo(new Dictionary<int, bool>()
                    {
                        {0, true},
                        {1, true},
                        {2, false},
                        {3, false}
                    });
                    throw new Exception($"This is an expected exception");
                },
                () => Console.WriteLine("Nothing to undo"),
                e => Console.WriteLine(e.Message));


            var commands = new List<ICommand>();
            commands.Add(new Command(() => Call(flags, 0), () => Undo(flags, 0), WriteLineOnFailure));
            commands.Add(new Command(() => Call(flags, 1), () => Undo(flags, 1), WriteLineOnFailure));
            commands.Add(faultyCommand);
            commands.Add(new Command(() => Call(flags, 3), () => Undo(flags, 3), WriteLineOnFailure));

            var sut = new CompositeCommand(commands.ToArray());

            sut.UndoOnFailure = true;

            sut.ContinueAfterFailure.Should().Be(false, "Setting Continue after failure should set this to false since " +
                                                        "they can both be false but neither can be true at the same time.");
            flags.Should().BeEquivalentTo(new Dictionary<int, bool>()
            {
                {0, false},
                {1, false},
                {2, false},
                {3, false}
            }, "We haven't invoked Call() yet");

            sut.Success.Should().BeFalse("We haven't invoked Call() yet.");

            sut.Call();

            flags.Should().BeEquivalentTo(new Dictionary<int, bool>()
            {
                {0, false},
                {1, false},
                {2, false},
                {3, false}
            }, "Flags should be unflagged during the UndoOnFailure operation.");
        }

        [Test]
        public void Duplicated_Commands_Should_Be_Equivalent_But_Not_Identical()
        {
            var cmd1 = new Command(
                callAction: () => Console.WriteLine("Call1"),
                undoAction: () => Console.WriteLine("Undo1"),
                failureAction: e => Console.WriteLine($"Failed1: {e.Message}"));

            var cmd2 = new Command(
                callAction: () => Console.WriteLine("Call2"),
                undoAction: () => Console.WriteLine("Undo2"),
                failureAction: e => Console.WriteLine($"Failed2: {e.Message}"));

            var cmd3 = new Command(
                callAction: () => Console.WriteLine("Call3"),
                undoAction: () => Console.WriteLine("Undo3"),
                failureAction: e => Console.WriteLine($"Failed3: {e.Message}"));


            var sut = new CompositeCommand(cmd1, cmd2, cmd3);

            sut.Call();

            sut.Success.Should().BeTrue();

            var protoCommand = sut.XmlProtoType();

            sut.Success.Should().BeTrue();
            protoCommand.Success.Should().BeFalse();


            sut.Should().BeEquivalentTo(protoCommand);
            sut.Should().Should().NotBeSameAs(protoCommand);
        }

        [Test]
        public void Duplicated_Commands_Should_Yield_Same_Result()
        {
            int testVal = 0;
            var sut = new CompositeCommand(
                new Command(() => testVal++, () => testVal--, WriteLineOnFailure),
                new Command(() => testVal++, () => testVal--, WriteLineOnFailure),
                new Command(() => testVal++, () => testVal--, WriteLineOnFailure),
                new Command(() => testVal++, () => testVal--, WriteLineOnFailure));

            sut.Call();

            testVal.Should().Be(sut.Count, "Each command increments this by one");

            sut.Call();

             testVal.Should().Be(sut.Count, "Assuming the previous Call() was successful the subsequent call should have exited early.");
             sut.Success.Should().BeTrue();


            var protoCommand = sut.XmlProtoType();
            sut.Success.Should().BeTrue("Duplicating the sut should not mutate it in any way.");
            sut.Success.Should().BeFalse("Duplicated commands should have their Success set to false so that they can be executed.");
            sut.Call();
            protoCommand.Call();

            testVal.Should().Be(sut.Count * 2, "We have 2 sets of identical commands.");

            protoCommand.Call();

            testVal.Should().Be(sut.Count * 2, "As with its singular counterpart, even duplicated commands should be Idempotent.");
        }
    }
}
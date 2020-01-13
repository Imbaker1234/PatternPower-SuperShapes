using System;
using Commands;
using FluentAssertions;
using NUnit.Framework;
using Prototype;

namespace CommandsTests
{
    [TestFixture]
    public class CommandTests
    {
        private static int TestValue { get; set; }
        private static bool FailedTest { get; set; }
        private readonly Action _incrementTestValue = () => TestValue++;
        private readonly Action _decrementTestValue = () => TestValue--;
        private readonly Action<Exception> _flagFailedTest = e =>
        {
            Console.WriteLine(e.Message);
            FailedTest = true;
        };

        private readonly Action _throwException = () => throw new Exception("This always fails");
        

        [SetUp]
        public void SetUp()
        {
            TestValue = 0;

            FailedTest = false;
        }

        [Test]
        public void Command_Will_Fire()
        {
            var sut = new Command(
                callAction: _incrementTestValue, 
                undoAction: _decrementTestValue, 
                failureAction: _flagFailedTest);

            sut.Call();

            TestValue.Should().Be(1, "A successful Call() should add +1 to the test value per Sut's CallAction");
            sut.Success.Should().BeTrue("A successful Call() will set Success to true");
        }

        [Test]
        public void Command_Will_Undo()
        {
            var sut = new Command(
                callAction: _incrementTestValue,
                undoAction: _decrementTestValue,
                failureAction: _flagFailedTest);

            sut.Call();

            TestValue.Should().Be(1, "A successful Call() should add +1 to the test value per Sut's CallAction");
            sut.Success.Should().BeTrue("A successful Call() will set Success to true");

            sut.Undo();

            TestValue.Should().Be(0, "Undo() per Sut's UndoAction should reduce this back to 0.");
            sut.Success.Should().BeFalse("A successful Undo() should return this to false.");
        }

        [Test]
        public void Command_Will_Only_Undo_If_It_Has_Successfully_Completed()
        {
            var sut = new Command(
                callAction: _incrementTestValue,
                undoAction: _decrementTestValue,
                failureAction: _flagFailedTest);

            sut.Undo();
            TestValue.Should().Be(0, "Sut's Success should be false, preventing the Undo() command from firing.");
            sut.Success.Should().BeFalse("Either this was false before Undo or Undo set this to false upon firing.");

            sut.Call();
            TestValue.Should().Be(1, "A successful Call() should add +1 to the test value per Sut's CallAction");
            sut.Success.Should().BeTrue("A successful Call() will set Success to true");

            sut.Undo();
            TestValue.Should().Be(0, "Undo() per Sut's UndoAction should reduce this back to 0.");
            sut.Success.Should().BeFalse("A successful Undo() should return this to false.");
        }

        [Test]
        public void Command_Will_Fire_Failure_Clause_On_Failure()
        {
            var sut = new Command(
                callAction: _throwException,
                undoAction: _decrementTestValue,
                failureAction: _flagFailedTest);

            
            sut.Call();

            FailedTest.Should().BeTrue("We threw an exception with Sut's action which should fire the FailureAction and set this to true.");
        }

        [Test]
        public void Commands_Should_Be_Idempotent()
        {
            var sut = new Command(
                callAction: _incrementTestValue,
                undoAction: _decrementTestValue,
                failureAction: _flagFailedTest);

            sut.Call();

            TestValue.Should().Be(1, "Call should increment this to 1");

            sut.Call();

            TestValue.Should().Be(1, "Sut should not increment this a second time, instead " +
                                     "returning early when it detects that it has already successfully completed.");
        }

        [Test]
        public void Duplicated_Commands_Should_Be_Equivalent_But_Not_Identitical()
        {
            var sut = new Command(
                callAction: _incrementTestValue,
                undoAction: _decrementTestValue,
                failureAction: _flagFailedTest);

            var protoCommand = sut.XmlProtoType();

            sut.Should().BeEquivalentTo(protoCommand);
            sut.Should().NotBeSameAs(protoCommand);
        }

        [Test]
        public void Duplicated_Commands_Should_Call_Even_If_The_Original_Command_Was_Successful()
        {
            var sut = new Command(
                callAction: () => TestValue++,
                undoAction: _decrementTestValue,
                failureAction: _flagFailedTest);

            var protoCommand = sut.XmlProtoType();
            
            sut.Call();

            TestValue.Should().Be(1, "Call should increment this to 1");

            sut.Call();

            TestValue.Should().Be(1, "Sut should not increment this a second time, instead " +
                                     "returning early when it detects that it has already successfully completed.");
            sut.Success.Should().BeTrue();


            protoCommand.Should().BeOfType<Command>();
            sut.Success.Should().BeTrue();

            protoCommand.Success.Should().BeFalse();

            protoCommand.Call();

            protoCommand.Success.Should().BeTrue();

            TestValue.Should().Be(2, "The protoCommand is being called for the first time");

            protoCommand.Call();

            TestValue.Should().Be(2, "As with its originator the protoCommand should be idempotent");
        }
    }
}

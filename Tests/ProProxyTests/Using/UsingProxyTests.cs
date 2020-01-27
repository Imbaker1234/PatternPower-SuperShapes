using Moq;
using NUnit.Framework;
using ProProxy.Proxies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using FluentAssertions;
using ProProxy.Core;
using ProProxy.Using;

namespace ProProxyTests.Using
{
    [TestFixture]
    public class UsingProxyTests<TInnerSubject, TUsing> where TInnerSubject : class
    {
        private MockRepository mockRepository;
        private Mock<Func<TUsing>> mockFunc;
        private Mock<IFake> mockSubject;
        private Mock<IUsingShell<IDisposable>> mockShell;
        private Mock<IDisposable> mockDisposable;
        private IFake sut;
        Dictionary<string, int> CallCount;

        [SetUp]
        public void SetUp()
        {
            CallCount = new Dictionary<string, int>();

            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.mockFunc = this.mockRepository.Create<Func<TUsing>>();
            this.mockSubject = this.mockRepository.Create<IFake>();
            this.mockShell = this.mockRepository.Create<IUsingShell<IDisposable>>();
            this.mockDisposable = this.mockRepository.Create<IDisposable>();

            mockDisposable.Setup(o => o.Dispose()).Raises(disposable => AddOrIncrementCallCount("Dispose"));
            mockShell.Setup(o => o.UsingFunc).Returns(() => mockDisposable.Object);
            mockSubject.Setup(o => o.Calculate(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(0);
            mockSubject.Setup(o => o.Listen(It.IsAny<string>())).Returns("none");
            mockSubject.Setup(o => o.Speak(It.IsAny<string>()));
            this.sut = UsingProxy<IFake, IDisposable>.As<IFake>(mockShell.Object, mockSubject.Object);
        }

        [Test]
        public void TryInvokeMember_StateUnderTest_ExpectedBehavior()
        {
            sut.Calculate(3, 58, 21);
            CallCount["Dispose"].Should().Be(1);
        }

        public void AddOrIncrementCallCount(string key)
        {
            if (CallCount.ContainsKey(key)) CallCount[key]++;
            else CallCount.Add(key, 1);
        }
    }
}
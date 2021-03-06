// <copyright file="UsingProxyTInnerSubjectTUsingTest.cs">Copyright ©  2020</copyright>
using System;
using System.Dynamic;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProProxy.Proxies;

namespace ProProxy.Proxies.Tests
{
    /// <summary>This class contains parameterized unit tests for UsingProxy`2</summary>
    [PexClass(typeof(UsingProxy<, >))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class UsingProxyTInnerSubjectTUsingTest
    {
        /// <summary>Test stub for TryInvokeMember(InvokeMemberBinder, Object[], Object&amp;)</summary>
        [PexGenericArguments(typeof(object), typeof(IDisposable))]
        [PexMethod]
        public bool TryInvokeMemberTest<TInnerSubject,TUsing>(
            [PexAssumeUnderTest]UsingProxy<TInnerSubject, TUsing> target,
            InvokeMemberBinder binder,
            object[] args,
            out object result
        )
            where TInnerSubject : class, new()
            where TUsing : IDisposable
        {
            bool result01 = target.TryInvokeMember(binder, args, out result);
            return result01;
            // TODO: add assertions to method UsingProxyTInnerSubjectTUsingTest.TryInvokeMemberTest(UsingProxy`2<!!0,!!1>, InvokeMemberBinder, Object[], Object&)
        }
    }
}

using System;
using ProProxy.Core;
using ProProxy.Decorator;
using ProProxy.Proxies;

namespace ProProxy.Shells
{
    public class DecoratorShell : IShell, IDecoratorShell
    {
        internal Action PreAction, PostAction;

        internal Action<Exception> ResponseOnFailure;

        public Type CorrespondingProxy { get; set; } = typeof(DecoratorProxy<>);

        public DecoratorShell(Action preAction, Action postAction, Action<Exception> responseOnFailure, bool usingPreAction = false)
        {
            PreAction = preAction;
            PostAction = postAction;
            ResponseOnFailure = responseOnFailure;
        }
    }
}
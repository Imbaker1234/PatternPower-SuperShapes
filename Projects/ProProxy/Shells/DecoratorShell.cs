using System;
using ProProxy.Proxies;

namespace ProProxy.Shells
{
    public class DecoratorShell : IShell
    {
        public Type CorrespondingProxy { get; set; } = typeof(DecoratorProxy<>);

        internal Action PreAction, PostAction;

        internal Action<Exception> ResponseOnFailure;

        public DecoratorShell(Action preAction, Action postAction, Action<Exception> responseOnFailure)
        {
            PreAction = preAction;
            PostAction = postAction;
            ResponseOnFailure = responseOnFailure;
        }
    }
}
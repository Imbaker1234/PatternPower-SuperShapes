using System;

namespace ProProxy
{
    public class DecoratorShell
    {
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
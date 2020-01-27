using System;
using ProProxy.Core;

namespace ProProxy.Proxies
{
    public class UsingShell<T> : IUsingShell<T>
    {
        public Type CorrespondingProxy { get; set; }
        public Func<T> UsingFunc { get; set; }

        public UsingShell(Func<T> usingFunc)
        {
            UsingFunc = usingFunc;
        }
    }
}
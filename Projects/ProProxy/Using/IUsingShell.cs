using System;
using ProProxy.Core;

namespace ProProxy.Proxies
{
    public interface IUsingShell<T> : IShell
    {
        Func<T> UsingFunc { get; set; }
    }
}
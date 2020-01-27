using System;
using ProProxy.Core;
using ProProxy.Proxies;

namespace ProProxy.Using
{
    public interface IUsingProxy<TUsing> : IProxy<TUsing> where TUsing : IDisposable
    {
        Func<TUsing> UsingFunc { get; set; }
    }
}
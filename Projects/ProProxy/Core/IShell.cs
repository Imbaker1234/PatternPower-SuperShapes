using System;
using System.Configuration;
using ProProxy.Proxies;

namespace ProProxy.Core
{
    public interface IShell
    {
        Type CorrespondingProxy { get; set; }
    }
}
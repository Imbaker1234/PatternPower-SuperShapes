using System;

namespace ProProxy
{
    public interface IProxyShell
    {
        Delegate PreAction { get; set; }
        Delegate PostAction { get; set; }
        Delegate ResponseOnFailure { get; set; }
    }
}
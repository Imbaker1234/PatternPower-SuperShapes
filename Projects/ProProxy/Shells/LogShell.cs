using System;
using System.Dynamic;
using ProProxy.Proxies;

namespace ProProxy.Shells
{
    public class LogShell
    {
        public Type CorrespondingProxy { get; set; } = typeof(LogProxy<>);

        public Action<string, InvokeMemberBinder, object[]> PreAction, PostAction;

        public Action<string, InvokeMemberBinder, object[], Exception> ResponseOnFailure;

        public LogShell(Action<string, InvokeMemberBinder, object[]> preAction, Action<string, InvokeMemberBinder, object[]> postAction, Action<string, InvokeMemberBinder, object[], Exception> responseOnFailure)
        {
            PreAction = preAction;
            PostAction = postAction;
            ResponseOnFailure = responseOnFailure;
        }
    }
}
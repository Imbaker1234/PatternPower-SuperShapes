using System;
using System.Dynamic;

namespace ProProxy
{
    public class LogShell
    {
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
using System;
using System.Dynamic;

namespace ProProxy
{
    public class LogShell
    {
        public Action<string, InvokeMemberBinder, object[]> _preAction, _postAction;

        public Action<string, InvokeMemberBinder, object[], Exception> _responseOnFailure;

        public LogShell(Action<string, InvokeMemberBinder, object[]> preAction, Action<string, InvokeMemberBinder, object[]> postAction, Action<string, InvokeMemberBinder, object[], Exception> responseOnFailure)
        {
            _preAction = preAction;
            _postAction = postAction;
            _responseOnFailure = responseOnFailure;
        }
    }
}
using System;
using System.Dynamic;
using ImpromptuInterface;

namespace ProProxy
{
    public class LogProxy<T> : DynamicObject where T : class, new()
    {
        protected Action<string, InvokeMemberBinder, object[]> _preAction, _postAction;

        protected Action<string, InvokeMemberBinder, object[], Exception> _responseOnFailure;

        protected T _subject;

        public string InterfaceBeingProxied;

        protected LogProxy(Action<string, InvokeMemberBinder, object[]> preAction, Action<string, InvokeMemberBinder, object[]> postAction, Action<string, InvokeMemberBinder, object[], Exception> responseOnFailure, T subject)
        {
            _preAction = preAction;
            _postAction = postAction;
            _responseOnFailure = responseOnFailure;
            _subject = subject;
            InterfaceBeingProxied = _subject.GetType().ToString();
        }

        /// <exception cref="T:System.ArgumentException">'I' must be an Interface!</exception>
        public static I As<I>(Action<string, InvokeMemberBinder, object[]> preAction, 
            Action<string, InvokeMemberBinder, object[]> postAction, Action<string, 
                InvokeMemberBinder, object[], Exception> responseOnFailure, T subject = null) where I : class
        {
            if (!typeof(I).IsInterface)
                throw new ArgumentException("'I' must be an Interface!", "Type<I>");

            if (subject is null) subject = new T();

            var product = new LogProxy<T>(preAction, postAction, responseOnFailure, subject).ActLike<I>();

            return product;
        }

        /// <exception cref="T:System.ArgumentException">'I' must be an Interface!</exception>
        public static I As<I>(LogShell ls, T subject = null) where I : class
        {
            return As<I>(ls._preAction, ls._postAction, ls._responseOnFailure, subject);
        }

        /// <exception cref="T:System.Exception">Condition.</exception>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                _preAction?.Invoke(typeof(T).ToString(),binder, args);

                result = _subject.GetType().GetMethod(binder.Name).Invoke(_subject, args);

                _postAction?.Invoke(typeof(T).ToString(), binder, args);

                return true;
            }
            catch (Exception e)
            {
                _responseOnFailure?.Invoke(binder, args, e);
                throw;
            }
        }
    }
}
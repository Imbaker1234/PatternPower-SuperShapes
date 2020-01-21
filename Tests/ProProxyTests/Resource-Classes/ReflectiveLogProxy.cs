using System;
using System.Dynamic;
using ImpromptuInterface;

namespace ProProxy
{
    internal class ReflectiveLogProxy<T> : Proxy<T> where T : class, new()
    {
        protected Action<string, InvokeMemberBinder, object[]> PreAction, PostAction;

        protected Action<string, InvokeMemberBinder, object[], Exception> ResponseOnFailure;

        internal ReflectiveLogProxy(Action<string, InvokeMemberBinder, object[]> preAction, Action<string, InvokeMemberBinder, object[]> postAction, Action<string, InvokeMemberBinder, object[], Exception> responseOnFailure, T innerSubject) : base(innerSubject)
        {
            PreAction = preAction;
            PostAction = postAction;
            ResponseOnFailure = responseOnFailure;
        }

        /// <exception cref="T:System.ArgumentException">'I' must be an Interface!</exception>
        public static I As<I>(Action<string, InvokeMemberBinder, object[]> preAction,
            Action<string, InvokeMemberBinder, object[]> postAction, Action<string,
                InvokeMemberBinder, object[], Exception> responseOnFailure, T subject = null) where I : class
        {
            if (!typeof(I).IsInterface)
                throw new ArgumentException("'I' must be an Interface!", "Type<I>");

            if (subject is null) subject = new T();

            var product = new ReflectiveLogProxy<T>(preAction, postAction, responseOnFailure, subject).ActLike<I>();

            return product;
        }

        /// <exception cref="T:System.ArgumentException">'I' must be an Interface!</exception>
        public static I As<I>(LogShell ls, T subject = null) where I : class
        {
            return As<I>(ls.PreAction, ls.PostAction, ls.ResponseOnFailure, subject);
        }


        /// <exception cref="T:System.Exception">Condition.</exception>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                PreAction?.Invoke(InnerSubjectName, binder, args);

                result = InnerSubject.GetType().GetMethod(binder.Name).Invoke(InnerSubject, args);

                PostAction?.Invoke(InnerSubjectName, binder, args);

                return true;
            }
            catch (Exception e)
            {
                ResponseOnFailure?.Invoke(InnerSubjectName, binder, args, e);
                throw;
            }
        }
    }
}
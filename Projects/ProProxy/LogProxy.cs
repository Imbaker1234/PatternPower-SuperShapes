using System;
using System.Dynamic;
using System.Linq;
using ImpromptuInterface;

namespace ProProxy
{
    public class LogProxy<T> : Proxy<T> where T : class, new()
    {
        protected new Action<string, InvokeMemberBinder, object[]> PreAction, PostAction;

        protected new Action<string, InvokeMemberBinder, object[], Exception> ResponseOnFailure;

        protected LogProxy(Delegate preAction, Delegate postAction, Delegate responseOnFailure, T innerSubject, Action<string, InvokeMemberBinder, object[]> preAction2, Action<string, InvokeMemberBinder, object[]> postAction2, Action<string, InvokeMemberBinder, object[], Exception> responseOnFailure2) : base(preAction, postAction, responseOnFailure, innerSubject)
        {
            PreAction = preAction2;
            PostAction = postAction2;
            ResponseOnFailure = responseOnFailure2;
        }

//        /// <exception cref="T:System.ArgumentException">'I' must be an Interface!</exception>
//        public static I As<I>(Action<string, InvokeMemberBinder, object[]> preAction, 
//            Action<string, InvokeMemberBinder, object[]> postAction, Action<string, 
//                InvokeMemberBinder, object[], Exception> responseOnFailure, T subject = null) where I : class
//        {
//            if (!typeof(I).IsInterface)
//                throw new ArgumentException("'I' must be an Interface!", "Type<I>");
//
//            if (subject is null) subject = new T();
//
////            var product = new LogProxy<T>(preAction, postAction, responseOnFailure, subject).ActLike<I>();
//
//            return product;
//        }

        /// <exception cref="T:System.ArgumentException">'I' must be an Interface!</exception>
//        public static I As<I>(LogShell ls, T subject = null) where I : class
//        {
//            return As<I>(ls._preAction, ls._postAction, ls._responseOnFailure, subject);
//        }

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
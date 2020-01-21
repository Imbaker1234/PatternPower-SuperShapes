﻿using System;
using System.Dynamic;
using ImpromptuInterface;
using ProProxy.Shells;

namespace ProProxy.Proxies
{
    public class DecoratorProxy<T> : Proxy<T> where T : class, new()
    {
        protected Action PreAction, PostAction;

        protected Action<Exception> ResponseOnFailure;



        public DecoratorProxy(Action preAction, Action postAction, Action<Exception> responseOnFailure, T innerSubject) : base(innerSubject)
        {
            PreAction = preAction;
            PostAction = postAction;
            ResponseOnFailure = responseOnFailure;
        }

        /// <exception cref="T:System.ArgumentException">'I' must be an Interface!</exception>
        public static I As<I>(Action preAction, Action postAction, Action<Exception> responseOnFailure, T instance = null) where I : class
        {
            instance = ValidateInterface(typeof(T), typeof(I), instance);

            var product = new DecoratorProxy<T>(preAction, postAction, responseOnFailure, instance).ActLike<I>();

            return product;
        }

        /// <exception cref="T:System.ArgumentException">'I' must be an Interface!</exception>
        public static I As<I>(DecoratorShell ds, T subject = null) where I : class
        {
            return As<I>(ds.PreAction, ds.PostAction, ds.ResponseOnFailure, subject);
        }

        /// <exception cref="T:System.Exception">Condition.</exception>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                PreAction?.Invoke();

                base.TryInvokeMember(binder, args, out result);

                PostAction?.Invoke();

                return true;
            }
            catch (Exception e)
            {
                ResponseOnFailure?.Invoke(e);
                throw;
            }
        }
    }
}
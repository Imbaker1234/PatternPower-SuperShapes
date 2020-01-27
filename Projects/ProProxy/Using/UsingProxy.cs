using System;
using System.Dynamic;
using Dynamitey;
using ImpromptuInterface;
using ProProxy.Core;
using ProProxy.Proxies;

namespace ProProxy.Using
{
    public class UsingProxy<TInnerSubject, TUsing> : Proxy<TInnerSubject>, IUsingProxy<TUsing> where TInnerSubject : class where TUsing : IDisposable
    {
        public Func<TUsing> UsingFunc { get; set; }
        
        //Create New is Private
        private UsingProxy(Func<TUsing> usingFunc, TInnerSubject innerSubject) : base(innerSubject)
        {
            UsingFunc = usingFunc;
            InnerSubject = innerSubject;
        }
        
        //Shell Constructor
        public static I As<I>(IUsingShell<TUsing> shell, TInnerSubject instance = null) where I : class
        {
            return As<I>(shell.UsingFunc, instance);
        }

        //Individual Value Constructor
        public static I As<I>(Func<TUsing> usingFunc, TInnerSubject instance = null) where I : class
        {
            ValidateInterface(typeof(TInnerSubject), typeof(I), instance);

            var product = new UsingProxy<TInnerSubject, TUsing>(usingFunc, instance).ActLike<I>();

            return product;
        }

        //Proxy Existing
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            using (UsingFunc.Invoke())
            {
             result = DelegateCache[binder.Name].FastDynamicInvoke(args);
            }
            return true;
        }
    }
}
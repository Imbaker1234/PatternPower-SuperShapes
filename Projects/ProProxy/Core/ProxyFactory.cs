using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using ImpromptuInterface;
using ProProxy.Proxies;

namespace ProProxy.Core
{
    public static class ProxyFactory
    {
        public static I As<I, T>(this IProxy<T> instance) where I : class
        {
            ValidateInterface(instance.InnerType, typeof(I));
            return instance.ActLike<I>();
        }

        private static void ValidateInterface(Type innerSubject, Type outerInterface)
        {
            if (!outerInterface.IsInterface) throw new ArgumentException("As<I>: 'I' must be an Interface!", "Type<I>");
            if (!innerSubject.IsAssignableFrom(outerInterface))
                throw new ArgumentException(
                    "As<I>: InnerSubject must implement 'I' interface!", nameof(innerSubject));
        }

//        public static IProxy Proxy<T>(this T innerSubject, IShell shell)
//        {
//            Type proxyType = shell.CorrespondingProxy;
//            return (IProxy) Activator.CreateInstance(proxyType, shell, innerSubject);
//        }
    }
}
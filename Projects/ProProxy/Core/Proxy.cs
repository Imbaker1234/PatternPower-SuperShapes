using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Dynamitey;
using TypeCache;

namespace ProProxy.Core
{
    public abstract class Proxy<T> : DynamicObject, IProxy<T> where T : class
    {
        public Type InnerType { get; set; }
        internal T InnerSubject { get; set; }
        protected string InnerSubjectName { get; set; }
        protected IDictionary<string, Delegate> DelegateCache { get; }

        protected Proxy()
        {
        }

        protected Proxy(T innerSubject)
        {
            Type innerType = typeof(T);
            InnerSubject = innerSubject;
            InnerSubjectName = innerType.ToString().Split('+').Last();
            DelegateCache = DelegateCacheFactory.GenerateCache(InnerSubject);
        }

        protected static void ValidateInterface(Type innerSubject, Type outerInterface, T instance)
        {
            if (!outerInterface.IsInterface) throw new ArgumentException("As<I>: 'I' must be an Interface!", "Type<I>");
            if (typeof(T).IsAssignableFrom(outerInterface)) throw new ArgumentException(
                "As<I>: InnerSubject must implement 'I' interface!", nameof(innerSubject));
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = DelegateCache[binder.Name].FastDynamicInvoke(args);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = DelegateCache["get_" + binder.Name].FastDynamicInvoke();
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            DelegateCache["set_" + binder.Name].FastDynamicInvoke(value);
            return true;
        }

        public override string ToString()
        {
            return InnerSubject.ToString();
        }

    }
}
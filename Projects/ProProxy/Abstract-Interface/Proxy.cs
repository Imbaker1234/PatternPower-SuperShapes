using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Dynamitey;
using ProProxy.Cache;

namespace ProProxy
{
    public abstract class Proxy<T> : DynamicObject where T : class, new()
    {
        protected T InnerSubject { get; set; }
        protected string InnerSubjectName { get; set; }
        protected DelegateCache<T> DelegateCache { get; set; }

        protected Proxy()
        {
        }

        protected Proxy(T innerSubject)
        {
            Type innerType = typeof(T);
            InnerSubject = innerSubject;
            InnerSubjectName = innerType.ToString().Split('+').Last();
            DelegateCache = new DelegateCache<T>(InnerSubject);
        }

        /// <exception cref="T:System.Exception">Condition.</exception>
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
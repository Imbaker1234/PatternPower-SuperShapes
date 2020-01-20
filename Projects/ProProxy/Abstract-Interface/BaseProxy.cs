using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Dynamitey;
using FastMember;

namespace ProProxy
{
    public abstract class BaseProxy<T> : DynamicObject where T : class, new()
    {
        protected T InnerSubject { get; set; }
        protected string InnerSubjectName { get; set; }

        protected Dictionary<string, CacheableInvocation> MethodCache { get; set; }
        protected Dictionary<string, PropertyInfo> PropertyCache { get; set; }

        protected TypeAccessor Accessor { get; set; }

        protected BaseProxy()
        {
        }

        protected BaseProxy(T innerSubject)
        {
            Type innerType = typeof(T);
            InnerSubject = innerSubject;
            InnerSubjectName = innerType.ToString().Split('+').Last();

            Accessor = TypeAccessor.Create(typeof(T));

            var methodInfos = typeof(T).GetMethods();
            MethodCache = new Dictionary<string, CacheableInvocation>(methodInfos.Length);
            foreach (var methodInfo in methodInfos)
            {
                if (!methodInfo.IsPublic) continue;

                InvocationKind kind;

                if (methodInfo.Name.StartsWith("get")) kind = InvocationKind.Get;
                else if (methodInfo.Name.StartsWith("set")) kind = InvocationKind.Set;
                else kind = InvocationKind.InvokeMember;

                var c = CacheableInvocation.CreateCall(kind, methodInfo.Name, new CallInfo(methodInfo.GetParameters().Length), InnerSubject);

                MethodCache.Add(methodInfo.Name, c);
            }

            var properties = InnerSubjectName.GetType().GetProperties();

            PropertyCache = new Dictionary<string, PropertyInfo>(properties.Length * 2);
            foreach (var propertyInfo in properties)
            {
            }
        }

        /// <exception cref="T:System.Exception">Condition.</exception>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = MethodCache[binder.Name].Invoke(InnerSubject, args);
            return true;
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = MethodCache[$"get_{binder.Name}"].Invoke(InnerSubject);
//            result = InnerSubject.GetType().GetProperty(binder.Name).GetValue(InnerSubject);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            MethodCache[$"set_{binder.Name}"].Invoke(InnerSubject, value);
            //            InnerSubject.GetType().GetProperty(binder.Name).SetValue(InnerSubject, value);
            return true;
        }

        public override string ToString()
        {
            return InnerSubject.ToString();
        }
    }
}
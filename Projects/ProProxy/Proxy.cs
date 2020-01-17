using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Dynamitey.DynamicObjects;
using FastMember;
using ImpromptuInterface;

namespace ProProxy
{
    public class Proxy<T> : DynamicObject where T : class, new()
    {
        protected T InnerSubject { get; set; }
        protected string InnerSubjectName { get; set; }
        protected TypeAccessor Accessor { get; set; }
        protected Dictionary<string, MethodInfo> Methods { get; set; }

        protected Proxy(Delegate preAction, Delegate postAction, Delegate responseOnFailure, T innerSubject)
        {
            Type tType = typeof(T);
            InnerSubject = innerSubject;
            InnerSubjectName = InnerSubject.GetType().ToString().Split('+').Last();
            Accessor = TypeAccessor.Create(tType);

            InnerSubjectName = Accessor.ToString();

            Methods = new Dictionary<string, MethodInfo>();

            foreach (var method in tType.GetMethods())
            {
                Methods.Add(method.Name, method);
            }
        }

        /// <exception cref="T:System.ArgumentException">'I' must be an Interface!</exception>
        internal static I As<I>(IProxyShell ps, T subject = default(T)) where I : class
        {
            if (!typeof(I).IsInterface)
                throw new ArgumentException("'I' must be an Interface!", "Type<I>");

            var product = new Proxy<T>(ps.PreAction, ps.PostAction, ps.ResponseOnFailure, subject).ActLike<I>();

            return product;
        }

        /// <exception cref="T:System.Exception">Condition.</exception>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = Methods[binder.Name].Invoke(InnerSubject, args);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Accessor[InnerSubject, binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Accessor[InnerSubject, binder.Name] = value;
            return true;
        }

        public override string ToString()
        {
            return InnerSubject.ToString();
        }
    }
}
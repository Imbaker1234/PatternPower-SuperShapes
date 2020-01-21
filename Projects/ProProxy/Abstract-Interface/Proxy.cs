using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Dynamitey;

namespace ProProxy
{
    public abstract class Proxy<T> : DynamicObject where T : class, new()
    {
        protected T InnerSubject { get; set; }
        protected string InnerSubjectName { get; set; }
        protected ConcurrentDictionary<string, Delegate> MethodCache;

        protected Proxy()
        {
        }

        protected Proxy(T innerSubject)
        {
            Type innerType = typeof(T);
            InnerSubject = innerSubject;
            InnerSubjectName = innerType.ToString().Split('+').Last();

            var methodInfos = typeof(T).GetMethods();

            var properties = InnerSubjectName.GetType().GetProperties();

            MethodCache = new ConcurrentDictionary<string, Delegate>();

            foreach (var method in typeof(T).GetMethods())
            {
                if(method.Name.Contains("Equals")) continue;
                var parameters = method.GetParameters();

                List<Type> paramTypes = new List<Type>();

                if (method.ReturnType != typeof(void)) paramTypes.Add(method.ReturnType);

                paramTypes.AddRange(parameters.Select(p => p.ParameterType));

                Type delType = typeof(Action);
                if (method.ReturnType == typeof(void))
                {
                    switch (paramTypes.Count)
                    {
                        case 0:
                            delType = typeof(Action);
                            break;
                        case 1:
                            delType = typeof(Action<>).MakeGenericType(paramTypes.ToArray());
                            break;
                        case 2:
                            delType = typeof(Action<,>).MakeGenericType(paramTypes.ToArray());
                            break;
                        case 3:
                            delType = typeof(Action<,,>).MakeGenericType(paramTypes.ToArray());
                            break;
                        case 4:
                            delType = typeof(Action<,,,>).MakeGenericType(paramTypes.ToArray());
                            break;
                    }
                }
                else
                {
                    switch (paramTypes.Count)
                    {
                        case 1:
                            delType = typeof(Func<>).MakeGenericType(paramTypes.ToArray());
                            break;
                        case 2:
                            delType = typeof(Func<,>).MakeGenericType(paramTypes.ToArray());
                            break;
                        case 3:
                            delType = typeof(Func<,,>).MakeGenericType(paramTypes.ToArray());
                            break;
                        case 4:
                            delType = typeof(Func<,,,>).MakeGenericType(paramTypes.ToArray());
                            break;
                    }
                }

                try
                {
                    MethodCache.TryAdd(method.Name, method.CreateDelegate(delType, InnerSubject));
                }
                catch (Exception e)
                {
                    throw new Exception($"{method.Name}: {e.Message}");
                }
            }
        }

        /// <exception cref="T:System.Exception">Condition.</exception>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = MethodCache[binder.Name].FastDynamicInvoke(args);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = MethodCache[binder.Name].FastDynamicInvoke();
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            MethodCache[binder.Name].FastDynamicInvoke(value);
            return true;
        }

        public override string ToString()
        {
            return InnerSubject.ToString();
        }
    }
}
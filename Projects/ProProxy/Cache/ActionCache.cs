using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dynamitey;

namespace ProProxy
{
    public class ActionCache<T> : IDictionary<string, Delegate>
    {
        public Dictionary<string, Delegate> Cache { get; set; }

        public T Subject { get; set; }

        public ActionCache()
        {
            var type = typeof(T);
            Cache = new Dictionary<string, Delegate>();

            foreach (var method in type.GetMethods())
            {
                // Cache.Add(method.Name, new CacheableInvocation(InvocationKind.InvokeMemberAction, method.Name, method.GetParameters().Length));
                if (!method.IsPublic) return;

                if (method.ReturnType == typeof(void))
                {
                    Cache.Add(method.Name, MagicAction<T>(method));
                }
                else
                {
                    Cache.Add(method.Name, MagicFunc<T>(method));
                }
            }

//            var properties = type.GetProperties();
//
//            foreach (var property in properties)
//            {
//                var getter = property.GetMethod;
//                var setter = property.SetMethod;
//
//                Cache.Add(getter.Name, MagicFunc(getter));
//                Cache.Add(setter.Name, MagicFunc(setter));
//            }
        }

        static Action<T> MagicAction<T>(MethodInfo method)
        {
            var parameter = method.GetParameters().SingleOrDefault();
            var instance = Expression.Parameter(typeof(T), "instance");
            var argument = Expression.Parameter(typeof(object), "argument");

            MethodCallExpression methodCall;

            if (parameter != null)
            {
                methodCall = Expression.Call(
                    instance,
                    method,
                    Expression.Convert(argument, parameter.ParameterType)
                );
            }
            else
            {
                methodCall = Expression.Call(
                    instance,
                    method);
            }

            return Expression.Lambda<Action<T>>(
                Expression.Convert(methodCall, typeof(void)),
                instance, argument
            ).Compile();
        }

        static Func<T> MagicFunc<T>(MethodInfo method)
        {
            var parameter = method.GetParameters().SingleOrDefault();
            var instance = Expression.Parameter(typeof(T), "instance");
            var argument = Expression.Parameter(typeof(object), "argument");

            MethodCallExpression methodCall;

            if (parameter != null)
            {
                methodCall = Expression.Call(
                    instance,
                    method,
                    Expression.Convert(argument, parameter.ParameterType)
                );
            }
            else
            {
                methodCall = Expression.Call(
                    instance,
                    method);
            }

            return Expression.Lambda<Func<T>>(
                Expression.Convert(methodCall, typeof(object)),
                instance, argument
            ).Compile();
        }

        public IEnumerator<KeyValuePair<string, Delegate>> GetEnumerator()
        {
            return Cache.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Cache).GetEnumerator();
        }

        public void Add(KeyValuePair<string, Delegate> item)
        {
            Cache.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Cache.Clear();
        }

        public bool Contains(KeyValuePair<string, Delegate> item)
        {
            return Cache.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, Delegate>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(KeyValuePair<string, Delegate> item)
        {
            return Cache.Remove(item.Key);
        }

        public int Count => Cache.Count;

        public bool IsReadOnly => false;

        public bool ContainsKey(string key)
        {
            return Cache.ContainsKey(key);
        }

        public void Add(string key, Delegate value)
        {
            Cache.Add(key, value);
        }

        public bool Remove(string key)
        {
            return Cache.Remove(key);
        }

        public bool TryGetValue(string key, out Delegate value)
        {
            return Cache.TryGetValue(key, out value);
        }

        public Delegate this[string key]
        {
            get => Cache[key];
            set => Cache[key] = value;
        }

        public ICollection<string> Keys => Cache.Keys;

        public ICollection<Delegate> Values => Cache.Values;
    }
}
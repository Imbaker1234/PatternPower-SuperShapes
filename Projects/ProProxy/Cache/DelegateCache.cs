﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ProProxy.Cache
{
    public class DelegateCache<T> : IDictionary<string, Delegate>
    {
        private IDictionary<string, Delegate> _impl;
        private T _instance;
        public DelegateCache(T instance)
        {
            _instance = instance;
            _impl = new ConcurrentDictionary<string, Delegate>();

            var genericActionArray = new[]
            {
                typeof(Action<>),
                typeof(Action<,>),
                typeof(Action<,,>),
                typeof(Action<,,,>),
                typeof(Action<,,,,>),
                typeof(Action<,,,,,>),
                typeof(Action<,,,,,,>),
                typeof(Action<,,,,,,,>),
                typeof(Action<,,,,,,,,>),
                typeof(Action<,,,,,,,,,>),
                typeof(Action<,,,,,,,,,,>),
                typeof(Action<,,,,,,,,,,,>),
                typeof(Action<,,,,,,,,,,,,>),
                typeof(Action<,,,,,,,,,,,,,>),
                typeof(Action<,,,,,,,,,,,,,,>),
                typeof(Action<,,,,,,,,,,,,,,,>)
            };

            var genericFuncArray = new[]
            {
                typeof(Func<>),
                typeof(Func<,>),
                typeof(Func<,,>),
                typeof(Func<,,,>),
                typeof(Func<,,,,>),
                typeof(Func<,,,,,>),
                typeof(Func<,,,,,,>),
                typeof(Func<,,,,,,,>),
                typeof(Func<,,,,,,,,>),
                typeof(Func<,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,,,,,>)
            };

            foreach (var method in typeof(T).GetMethods())
            {
                if (method.Name.Contains("Equals")) continue;

                List<Type> paramTypes = new List<Type>();

                if (method.ReturnType != typeof(void)) paramTypes.Add(method.ReturnType);

                paramTypes.AddRange(
                    method.GetParameters().Select(p => p.ParameterType));

                var sourceArray = method.ReturnType == typeof(void) ? genericActionArray : genericFuncArray;

                Type delType = sourceArray[paramTypes.Count - 1].MakeGenericType(paramTypes.ToArray());

                try
                {
                    _impl.Add(method.Name, method.CreateDelegate(delType, _instance));
                }
                catch (Exception e)
                {
                    throw new Exception($"FAILED TO CACHE DELEGATE: {method.Name}: {e.Message}");
                }
            }
        }


        public IEnumerator<KeyValuePair<string, Delegate>> GetEnumerator()
        {
            return _impl.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _impl).GetEnumerator();
        }

        public void Add(KeyValuePair<string, Delegate> item)
        {
            _impl.Add(item);
        }

        public void Clear()
        {
            _impl.Clear();
        }

        public bool Contains(KeyValuePair<string, Delegate> item)
        {
            return _impl.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, Delegate>[] array, int arrayIndex)
        {
            _impl.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, Delegate> item)
        {
            return _impl.Remove(item);
        }

        public int Count => _impl.Count;

        public bool IsReadOnly => _impl.IsReadOnly;

        public bool ContainsKey(string key)
        {
            return _impl.ContainsKey(key);
        }

        public void Add(string key, Delegate value)
        {
            _impl.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _impl.Remove(key);
        }

        public bool TryGetValue(string key, out Delegate value)
        {
            return _impl.TryGetValue(key, out value);
        }

        public Delegate this[string key]
        {
            get => _impl[key];
            set => _impl[key] = value;
        }

        public ICollection<string> Keys => _impl.Keys;

        public ICollection<Delegate> Values => _impl.Values;
    }
}
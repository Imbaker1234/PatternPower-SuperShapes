using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TypeCache
{
    public static class DelegateCacheFactory
    {
        private static Type[] GenericActionArray = new[]
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

        private static Type[] GenericFuncArray = new[]
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

        public static IDictionary<string, Delegate> GenerateCache<T>(T instance)
        {
            var product = new Dictionary<string, Delegate>();

            foreach (var method in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                List<Type> paramTypes = new List<Type>();

                int minus = 1;

                paramTypes.AddRange(
                    method.GetParameters().Select(p => p.ParameterType));

                if (method.ReturnType != typeof(void))
                {
                    paramTypes.Add(method.ReturnType);
                }

                var sourceArray = method.ReturnType == typeof(void) ? GenericActionArray : GenericFuncArray;

                Type delType = sourceArray[paramTypes.Count - 1].MakeGenericType(paramTypes.ToArray());

                try
                {
                    var @delegate = Delegate.CreateDelegate(delType, instance, method);
                    product.Add(method.Name, @delegate);
                }
                catch (Exception e)
                {
                    var providedTypes = paramTypes.Select(p => p.ToString());
                    var expectedTypes = method.GetParameters().Select(p => p.ToString());
                    Console.WriteLine(paramTypes.ToString());
                    var ex = new Exception($"FAILED TO CACHE DELEGATE: {method.Name}: {e.Message}\n" +
                                           $"ProvidedTypes: {string.Join(",", providedTypes)}\n" +
                                           $"ExpectedTypes: {string.Join(",", expectedTypes)}\n");

                    throw ex;
                }
            }

            return product;
        }
    }
}
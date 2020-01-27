using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using ProProxy.Core;

namespace ProProxy.Decorator
{
    public interface IDecoratorProxy<T> where T : class, IProxy<T>
    {
    }
}
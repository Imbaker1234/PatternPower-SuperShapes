using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace ProProxy.Core
{
    public interface IProxy<T>
    {
        Type InnerType { get; set; }
    }
}
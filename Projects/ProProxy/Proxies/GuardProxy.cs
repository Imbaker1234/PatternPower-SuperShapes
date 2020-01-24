using System;

namespace ProProxy.Proxies
{
    public class GuardProxy<InnerType> : Proxy<InnerType> where InnerType : class, new()
    {
        protected Func<bool> Guard;
        protected Func<Exception> InvalidAccessResponse;
        protected Action<Exception> ResponseOnFailure;


    }
}
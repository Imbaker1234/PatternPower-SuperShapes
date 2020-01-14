using System;
using System.Dynamic;
using ImpromptuInterface;

namespace ProProxy
{
    public class ProProxy<T> : DynamicObject where T : class, new()
    {
        private readonly Action _preAction, _postAction;

        private readonly Action<Exception> _responseOnFailure;

        private readonly T _subject;

        protected ProProxy(Action preAction, Action postAction, Action<Exception> _responseOnFailure, T subject)
        {
            _preAction = preAction;
            _postAction = postAction;
            this._responseOnFailure = _responseOnFailure;
            _subject = subject;
        }

        /// <exception cref="T:System.ArgumentException">'I' must be an Interface!</exception>
        public I As<I>() where I : class
        {
            if (!typeof(I).IsInterface)
                throw new ArgumentException("'I' must be an Interface!");

            return new ProProxy<T>(_preAction, _postAction, _responseOnFailure, _subject).ActLike<I>();
        }

        /// <exception cref="T:System.Exception">Condition.</exception>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                _preAction.Invoke();

                result = _subject.GetType().GetMethod(binder.Name).Invoke(_subject, args);

                _postAction.Invoke();

                return true;
            }
            catch (Exception e)
            {
                _responseOnFailure.Invoke(e);
                throw;
            }
        }
    }
}
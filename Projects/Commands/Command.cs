using System;
using Prototype;

namespace Commands
{
    public class Command : ICommand
    {
        protected Action CallAction { get; set; }
        protected Action UndoAction { get; set; }
        protected Action<Exception> FailureAction { get; set; }
        public bool Success { get; set; }

        public ICommand Prototype()
        {
            var product = this.XmlProtoType();
            product.CallAction = CallAction;
            product.UndoAction = UndoAction;
            product.FailureAction = FailureAction;

            return product;
        }

        public void Call()
        {
            if (Success) return;

            try
            {
                CallAction.Invoke();
                Success = true;
            }
            catch (Exception e)
            {
                FailureAction(e);
            }
        }

        public void Undo()
        {
            try
            {
                if (Success)
                {
                    UndoAction.Invoke();
                    Success = false;
                }
            }
            catch (Exception e)
            {
                FailureAction(e);
            }
        }

        public Command(Action callAction, Action undoAction, Action<Exception> failureAction)
        {
            CallAction = callAction;
            UndoAction = undoAction;
            FailureAction = failureAction;
        }

        public Command()
        {
        }
    }
}
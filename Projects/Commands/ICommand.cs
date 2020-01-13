using System;
using System.Runtime.Serialization;
using Prototype;

namespace Commands
{
    public interface ICommand : IPrototype<ICommand>
    {
        bool Success { get; set; }
        void Call();
        void Undo();
    }
}

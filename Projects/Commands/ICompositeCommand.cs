using System.Collections.Generic;

namespace Commands
{
    public interface ICompositeCommand : ICommand, IList<ICommand>
    {
        bool ContinueAfterFailure { get; set; }
        bool UndoOnFailure { get; set; }
    }
}
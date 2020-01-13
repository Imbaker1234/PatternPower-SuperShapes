using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prototype;

namespace Commands
{
    public class CompositeCommand : ICompositeCommand
    {
        public ICompositeCommand Prototype()
        {
            foreach (var command in Commands)
            {
                
            }
        }

        public bool Success
        {
            get
            {
                foreach (var command in Commands)
                {
                    if (command.Success is false) return false;
                }

                return true;
            }
            set
            {
                foreach (var command in Commands)
                {
                    command.Success = value;
                }
            }
        }

        private bool _continueAfterFailure;

        public bool ContinueAfterFailure
        {
            get => _continueAfterFailure;
            set
            {
                if (value is true)
                {
                    UndoOnFailure = false;
                }

                _continueAfterFailure = value;
            }
        }

        private bool _undoOnFailure;

        public bool UndoOnFailure
        {
            get => _undoOnFailure;
            set
            {
                if (value is true)
                {
                    ContinueAfterFailure = false;
                }

                _undoOnFailure = value;
            }
        }

        private List<ICommand> Commands { get; set; } = new List<ICommand>();

        public CompositeCommand()
        {
        }

        public CompositeCommand(params ICommand[] commands)
        {
            foreach (var cmd in commands)
            {
                Commands.Add(cmd);
            }
        }

        public void Call()
        {
            if (Success) return;

            ICommand lastCommand = null;

            foreach (var command in Commands)
            {
                if (ContinueAfterFailure is true || lastCommand is null || lastCommand.Success is true)
                {
                    command.Call();
                }
                else
                {
                    if (UndoOnFailure) Undo();
                }

                lastCommand = command;
            }
        }

        public void Undo()
        {
            //We cast to an IEnumerable to gain access to a reverse
            //method which is not mutating.
            var commands = (IEnumerable<ICommand>) Commands;

            foreach (var command in commands.Reverse().Where(command => command.Success))
            {
                command.Undo();
            }
        }


        #region List-Members

        public IEnumerator<ICommand> GetEnumerator()
        {
            return Commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Commands).GetEnumerator();
        }

        public void Add(ICommand item)
        {
            Commands.Add(item);
        }

        public void Clear()
        {
            Commands.Clear();
        }

        public bool Contains(ICommand item)
        {
            return Commands.Contains(item);
        }

        public void CopyTo(ICommand[] array, int arrayIndex)
        {
            Commands.CopyTo(array, arrayIndex);
        }

        public bool Remove(ICommand item)
        {
            return Commands.Remove(item);
        }

        public int Count => Commands.Count;

        public bool IsReadOnly => ((ICollection<ICommand>) Commands).IsReadOnly;

        public int IndexOf(ICommand item)
        {
            return Commands.IndexOf(item);
        }

        public void Insert(int index, ICommand item)
        {
            Commands.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Commands.RemoveAt(index);
        }

        public ICommand this[int index]
        {
            get => Commands[index];
            set => Commands[index] = value;
        }

        #endregion
    }
}
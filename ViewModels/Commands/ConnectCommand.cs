using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace P2P_Chat_App.ViewModels.Commands
{
    internal class ConnectCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private ConnectionViewModel _parent;

        public ConnectionViewModel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public ConnectCommand(ConnectionViewModel parent)
        {
            this.Parent = parent;
        }
        public bool CanExecute(object? parameter)
        {
            return true;
        }
        public void Execute(object? parameter)
        {
            Parent.Connect();
        }
    }
}

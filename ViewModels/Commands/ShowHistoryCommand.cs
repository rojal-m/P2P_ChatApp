using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace P2P_Chat_App.ViewModels.Commands
{
    internal class ShowHistoryCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private MainViewModel _parent;

        public MainViewModel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public ShowHistoryCommand(MainViewModel parent)
        {
            this.Parent = parent;
        }
        public bool CanExecute(object? parameter)
        {
            return true;
        }
        public void Execute(object? parameter)
        {
            Parent.showHistory(parameter.ToString());
        }
    
    }
}

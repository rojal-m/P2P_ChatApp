using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace P2P_Chat_App.ViewModels
{
    public class PopupViewModel
    {
        private MainViewModel _parent;
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public ICommand StartCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        

        public PopupViewModel(MainViewModel viewModel)
        {
            _parent = viewModel;
            _parent.ConnectionIsOpen = true;
            StartCommand = new DelegateCommand(Start);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private void Start()
        {
            // Perform start action
            Trace.WriteLine("Reached here");
        }

        private void Cancel()
        {
            // Perform cancel action
        }
    }
}

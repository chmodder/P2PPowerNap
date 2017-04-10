using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WPFClient.Handlers;
using WPFClient.Helpers;
using WPFClient.Model;

namespace WPFClient.ViewModels
{
    public class MainWindowViewModel : Notifications
    {
        private string _localFileDirectory;
        public string LocalFileDirectory { get { return _localFileDirectory; } set { _localFileDirectory = value; } }

        private string _fileName;
        public string FileName { get { return _fileName; } set { _fileName = value; } }

        private MainViewModelHandler _handler;
        public MainViewModelHandler Handler { get { return _handler; } set { _handler = value; } }

        public TCPServer Server { get; set; }

        public MyTraceListener Listener { get; set; }

        public ICommand GetFileCmd { get; set; }

        public ICommand RemoveAllCmd { get; set; }


        public MainWindowViewModel()
        {
            LocalFileDirectory = "C:/temp/";
            Handler = new MainViewModelHandler(this);

            #region Trace Listener setup
            /*------Experimentation: substitute for TextBoxTraceListener--------*/
            Trace.AutoFlush = true;
            Listener = new MyTraceListener();
            Trace.Listeners.Add(Listener);
            /*------Experimentation: End--------*/
            #endregion

            Server = new TCPServer(LocalFileDirectory, 14593);

            GetFileCmd = new RelayCommand(Handler.GetFile);
            RemoveAllCmd = new RelayCommand(Handler.RemoveAll);

        }

    }
}

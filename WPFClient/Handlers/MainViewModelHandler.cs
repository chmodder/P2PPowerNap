using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.ViewModels;
using WPFClient.WCFRepositorySoapServiceReference;

namespace WPFClient.Handlers
{
    public class MainViewModelHandler
    {
        private MainWindowViewModel _viewModel;
        public MainWindowViewModel ViewModel { get { return _viewModel; } set { _viewModel = value; } }

        public MainViewModelHandler(MainWindowViewModel mainWindowViewModel)
        {
            this._viewModel = mainWindowViewModel;
        }

        public void GetFile()
        {
            ViewModel.Server.GetFile(ViewModel.LocalFileDirectory,ViewModel.FileName);
        }
    }
}

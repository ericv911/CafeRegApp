using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CafeRegApp
{
    public class InstallRegistratieAppViewModel : BaseViewModel
    {
        //private fields
        #region private stuff
        private string _wachtwoord;
        private ICommand _installCoronaAppCommand;
        private bool canExecute = true;
        #endregion

        //public properties 
        #region properties

        public string Wachtwoord
        {
            get { return _wachtwoord; }
            set
            {
                _wachtwoord = value;
                OnPropertyChanged("Wachtwoord");
            }
        }

        public bool CanExecute
        {
            get
            {
                return this.canExecute;
            }
            set
            {
                if (this.canExecute == value)
                {
                    return;
                }
                this.canExecute = value;
            }
        }
        #endregion
        // commands
        #region command interfaces
        public ICommand InstallCoronaAppCommand { get { return _installCoronaAppCommand; } set { _installCoronaAppCommand = value; } }

        #endregion
        // constructor
        #region constructor
        public InstallRegistratieAppViewModel(Window window)
        {
            /* window events */
            window.Loaded += (sender, e) =>
            {
            };

            window.Closed += (sender, e) =>
            {
            };

            InstallCoronaAppCommand = new RelayCommand(InstallCoronaApp, param => this.canExecute);
        }
        #endregion
        //private methods
        #region private methods

        private void InstallCoronaApp(object obj)
        {
            string path = @"C:\CorRegA";
            string pathdesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (!Directory.Exists(path))
            {
                // This path is a directory
                Directory.CreateDirectory(path);
            }
            string[] files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\program");
            foreach (string s in files)
            {
                System.IO.File.Copy(s, Path.Combine(path, Path.GetFileName(s)), true);
            }
            files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\shortcut");
            foreach (string s in files)
            {
                System.IO.File.Copy(s, Path.Combine(pathdesktop, Path.GetFileName(s)), true);
            }
            CreateIni();
            MessageBox.Show("Installatie afgerond.");
            System.Windows.Application.Current.Shutdown();
        }

        private void CreateIni()
        {
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\CorRegA\data");
            if (!Directory.Exists(path))
            {
                // This path is a directory
                Directory.CreateDirectory(path);
            }
            return;
        }
        #endregion
    }  
}

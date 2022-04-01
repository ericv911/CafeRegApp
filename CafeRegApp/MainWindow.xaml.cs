using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CafeRegApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new RegistratieAppViewModel(this);
            InitializeComponent();
        }
        //blijkbaar is onderstaande code geen schending van het MVVM Model, in de codebehind wordt het password veld enke gebind aan een class-property. 
        // de UI en de Logic zijn nog steeds compleet van elkaar gescheiden.
        //private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        //{
        //    ((dynamic)this.DataContext).InputWachtwoord = ((PasswordBox)sender).Password.ToString();
        //}
    }
}

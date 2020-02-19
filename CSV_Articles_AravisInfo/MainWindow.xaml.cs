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
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Configuration;

namespace CSV_Articles_AravisInfo
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            viewModel.viewModel theViewModel = new viewModel.viewModel(frm);

            grd_main.DataContext = theViewModel;
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_Minus_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void stk_fileMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            if(!btn_file.IsMouseOver)
                stk_fileMenu.Visibility = Visibility.Hidden;
        }

        private void btn_file_Click(object sender, RoutedEventArgs e)
        {
            stk_fileMenu.Visibility = Visibility.Visible;
        }

        private void btn_file_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!stk_fileMenu.IsMouseOver)
                stk_fileMenu.Visibility = Visibility.Hidden;
        }
    }
}

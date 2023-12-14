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
using System.Windows.Shapes;

namespace gametop
{
    /// <summary>
    /// Логика взаимодействия для Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public static Window roomfirst;

        public Menu()
        {
            InitializeComponent();
        }

        private void ex_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void pl_Click(object sender, RoutedEventArgs e)
        {
            if (roomfirst == null)
            {
                roomfirst = new MainWindow();
                roomfirst.Show();
            }
            else
            {
                roomfirst.Activate();
            }
            this.Hide();
        }

    }
}

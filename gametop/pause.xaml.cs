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
using System.Windows.Threading;

namespace gametop
{
    /// <summary>
    /// Логика взаимодействия для pause.xaml
    /// </summary>
    public partial class pause : Window
    {
        DispatcherTimer timer;
        Image player;

        public pause(DispatcherTimer timer, Image player)
        {
            InitializeComponent();
            this.timer = timer;
            this.player = player;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            player.Source = new BitmapImage(new Uri("charecter\\down.png", UriKind.RelativeOrAbsolute));
            player.Height = 166;
            player.Width = 126;
            this.Visibility = Visibility.Collapsed;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

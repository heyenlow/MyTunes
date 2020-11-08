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

namespace MyTunes
{
    /// <summary>
    /// Interaction logic for RemoveForm.xaml
    /// </summary>
    public partial class RemoveForm : Window
    {
        public RemoveForm()
        {
            InitializeComponent();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

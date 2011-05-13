using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CCMManager.Resources
{
    /// <summary>
    /// Interaction logic for AddClassroomDialog.xaml
    /// </summary>
    public partial class AddClassroomDialog : Window
    {
        public AddClassroomDialog()
        {
            InitializeComponent();
            this.ClassroomName.Focus();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

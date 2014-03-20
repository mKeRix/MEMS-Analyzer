using MEMS_Analyzer.Content.Data;
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

namespace MEMS_Analyzer.Pages
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class Device : UserControl
    {
        public Device()
        {
            InitializeComponent();
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SensorViewModel)this.DataContext;
            // consider setting this up using MVVM
            if (!viewModel.sensorConn.isConnected)
            {
                if (viewModel.sensorConn.connectPort(ComboBoxPorts.Text))
                {
                    ButtonConnect.Content = "Trennen";
                    ComboBoxPorts.IsEnabled = false;
                }
            }
            else
            {
                if (viewModel.sensorConn.disconnectPort())
                {
                    ButtonConnect.Content = "Verbinden";
                    ComboBoxPorts.IsEnabled = true;
                }
            }
        }
    }
}

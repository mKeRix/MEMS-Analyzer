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
            if (!viewModel.sensorConn.isConnected)
                viewModel.sensorConn.ConnectPort(ComboBoxPorts.Text);
            else
                viewModel.sensorConn.DisconnectPort();
        }

        private void ButtonSettingsSave_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SensorViewModel)this.DataContext;
            viewModel.sensorConn.SaveSettings(int.Parse(ComboBoxAccel.Text), int.Parse(ComboBoxGyro.Text), (int)SliderRefreshRate.Value);
        }
    }
}

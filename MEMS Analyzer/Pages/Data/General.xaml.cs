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

namespace MEMS_Analyzer.Pages.Data
{
    /// <summary>
    /// Interaction logic for General.xaml
    /// </summary>
    public partial class General : UserControl
    {
        public General()
        {
            InitializeComponent();
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SensorViewModel)this.DataContext;
            viewModel.sensorConn.StartMeasure();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SensorViewModel)this.DataContext;
            viewModel.sensorConn.StopMeasure();
        }
    }
}

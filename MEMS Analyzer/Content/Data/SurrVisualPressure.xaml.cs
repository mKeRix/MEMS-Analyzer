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

namespace MEMS_Analyzer.Content.Data
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class SurrVisualPressure : UserControl
    {
        public SurrVisualPressure()
        {
            InitializeComponent();
            // set checkbox legend style
            plotterPressure.Description.LegendItem.ContentTemplate = this.FindResource("LineLegendItemContentTemplate") as DataTemplate;
        }

        private void CheckBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (sender as CheckBox).IsChecked = !(sender as CheckBox).IsChecked;
            e.Handled = true;
        }

        private void plotter_Loaded(object sender, RoutedEventArgs e)
        {
            // set Y axis restrictions
            plotter.Viewport.Restrictions.Clear();
            ViewportAxesRangeRestriction restr = new ViewportAxesRangeRestriction();
            restr.YRange = new DisplayRange(900, 1100);
            plotter.Viewport.Restrictions.Add(restr);
        }
    }
}

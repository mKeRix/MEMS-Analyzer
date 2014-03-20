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
    /// Interaction logic for AccelVisual.xaml
    /// </summary>
    public partial class AccelVisual : UserControl
    {
        public AccelVisual()
        {
            InitializeComponent();
            // set Y axis restrictions
            ViewportAxesRangeRestriction restr = new ViewportAxesRangeRestriction();
            restr.YRange = new DisplayRange(-4.5, 4.5);
            plotter.Viewport.Restrictions.Add(restr);
            // set checkbox legend style
            plotterAccelX.Description.LegendItem.ContentTemplate = this.FindResource("LineLegendItemContentTemplate") as DataTemplate;
            plotterAccelY.Description.LegendItem.ContentTemplate = this.FindResource("LineLegendItemContentTemplate") as DataTemplate;
            plotterAccelZ.Description.LegendItem.ContentTemplate = this.FindResource("LineLegendItemContentTemplate") as DataTemplate;
        }

        private void CheckBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (sender as CheckBox).IsChecked = !(sender as CheckBox).IsChecked;
            e.Handled = true;
        }
    }
}

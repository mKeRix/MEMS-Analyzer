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
        }
    }
}

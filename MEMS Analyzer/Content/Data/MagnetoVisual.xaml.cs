﻿using System;
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
    /// Interaction logic for MagnetoVisual.xaml
    /// </summary>
    public partial class MagnetoVisual : UserControl
    {
        public MagnetoVisual()
        {
            InitializeComponent();
            // set checkbox legend style
            plotterMagnetoX.Description.LegendItem.ContentTemplate = this.FindResource("LineLegendItemContentTemplate") as DataTemplate;
            plotterMagnetoY.Description.LegendItem.ContentTemplate = this.FindResource("LineLegendItemContentTemplate") as DataTemplate;
            plotterMagnetoZ.Description.LegendItem.ContentTemplate = this.FindResource("LineLegendItemContentTemplate") as DataTemplate;
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
            restr.YRange = new DisplayRange(-1300, 1300);
            plotter.Viewport.Restrictions.Add(restr);
        }
    }
}

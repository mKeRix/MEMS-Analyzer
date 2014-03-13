using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEMS_Analyzer.Content.Data
{
    class SensorViewModel
    {
        public ObservableCollection<SensorData> Items { get; set; }
    }
}

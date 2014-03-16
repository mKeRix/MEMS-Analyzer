using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEMS_Analyzer.Content.Data
{
    public class SensorData
    {
        public int id { get; set; }
        public double accelX { get; set; }
        public double accelY { get; set; }
        public double accelZ { get; set; }
        public double gyroX { get; set; }
        public double gyroY { get; set; }
        public double gyroZ { get; set; }
        public double magnetoX { get; set; }
        public double magnetoY { get; set; }
        public double magnetoZ { get; set; }
        public double airPressure { get; set; }
        public double airTemp { get; set; }
    }
}

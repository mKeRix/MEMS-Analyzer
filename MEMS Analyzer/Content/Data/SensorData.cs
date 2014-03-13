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
        public float accelX { get; set; }
        public float accelY { get; set; }
        public float accelZ { get; set; }
        public float gyroX { get; set; }
        public float gyroY { get; set; }
        public float gyroZ { get; set; }
        public float magnetoX { get; set; }
        public float magnetoY { get; set; }
        public float magnetoZ { get; set; }
        public float airPressure { get; set; }
        public float airTemp { get; set; }
    }
}

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

        public double time { get; set; }

        public double accelXMeter
        {
            get { return Math.Round(accelX * 9.80665, 2); }
        }

        public double accelYMeter
        {
            get { return Math.Round(accelY * 9.80665, 2); }
        }

        public double accelZMeter
        {
            get { return Math.Round(accelZ * 9.80665, 2); }
        }

        public double accelSum
        {
            get { return Math.Round(Math.Sqrt((accelX * accelX) + (accelY * accelY) + (accelZ * accelZ)), 2); }
        }

        public double accelSumHorizontal
        {
            get { return Math.Round(Math.Sqrt((accelSum * accelSum) - 1), 2); }
        }

        public double accelSumMeter
        {
            get { return Math.Round(accelSum * 9.80665, 2); }
        }

        public double accelSumHorizontalMeter
        {
            get { return Math.Round(accelSumHorizontal * 9.80665, 2); }
        }
    }
}

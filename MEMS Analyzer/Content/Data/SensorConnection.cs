using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEMS_Analyzer.Content.Data
{
    public class SensorConnection
    {
        private SerialPort sensorPort;

        public string[] availablePorts
        {
            get
            {
                return SerialPort.GetPortNames();
            }
        }

        public bool connectPort(string port)
        {
            sensorPort = new SerialPort(port);

            try
            {
                sensorPort.Open();
                sensorPort.DataReceived += sensorPort_DataReceived;
                return true;
            }
            catch
            {
                return false;
            }
        }

        void sensorPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // TODO: add data usage (explode into fragments and add to data collection)
            System.Diagnostics.Debug.WriteLine(sensorPort.ReadLine());
        }
    }
}

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
        public SensorConnection()
        {
            sensorPort = new SerialPort();
        }

        public SerialPort sensorPort { get; private set; }

        public string[] availablePorts
        {
            get
            {
                return SerialPort.GetPortNames();
            }
        }

        public bool connectPort(string port)
        {
            sensorPort.PortName = port;

            try
            {
                sensorPort.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

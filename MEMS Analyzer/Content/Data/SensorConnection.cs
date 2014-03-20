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
            isConnected = false;
        }

        public SerialPort sensorPort { get; private set; }
        public bool isConnected { get; private set; }

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
                isConnected = true;
                return true;
            }
            catch
            {
                isConnected = false;
                return false;
            }
        }

        public bool disconnectPort()
        {
            try
            {
                sensorPort.Close();
                isConnected = false;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

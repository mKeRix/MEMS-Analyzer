using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEMS_Analyzer.Content.Data
{
    public class SensorConnection : INotifyPropertyChanged
    {
        public SensorConnection()
        {
            sensorPort = new SerialPort();
            isConnected = false;
        }

        public SerialPort sensorPort { get; private set; }

        private bool _isConnected;
        public bool isConnected
        {
            get { return _isConnected; }
            private set
            {
                _isConnected = value;
                NotifyPropertyChanged("isConnected");
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

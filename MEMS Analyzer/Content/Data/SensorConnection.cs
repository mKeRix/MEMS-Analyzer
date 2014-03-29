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
            accelLimit = 4;
            gyroLimit = 2000;
            refreshRate = 10;
            portName = "";
        }

        public SerialPort sensorPort { get; private set; }

        private string b_portName;
        public string portName
        {
            get { return b_portName; }
            set
            {
                b_portName = value;
                NotifyPropertyChanged("portName");
            }
        }

        private int b_accelLimit;
        public int accelLimit
        {
            get { return b_accelLimit; }
            set
            {
                b_accelLimit = value;
                NotifyPropertyChanged("accelLimit");
                NotifyPropertyChanged("accelLimitItem");
            }

        }

        private int b_gyroLimit;
        public int gyroLimit
        {
            get { return b_gyroLimit; }
            set
            {
                b_gyroLimit = value;
                NotifyPropertyChanged("gyroLimit");
                NotifyPropertyChanged("gyroLimitItem");
            }
        }

        public int internRefreshRate { get; private set; }
        private int b_refreshRate;
        public int refreshRate
        {
            get { return b_refreshRate; }
            set
            {
                b_refreshRate = value;
                internRefreshRate = value / 10;
                NotifyPropertyChanged("refreshRate");
            }
        }

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

        public bool ConnectPort(string port)
        {
            sensorPort.PortName = port;
            portName = port;

            try
            {
                sensorPort.Open();
                sensorPort.WriteLine("default"); // hotfix to deal with settings, as they cannot be read out easily
                accelLimit = 4;
                gyroLimit = 2000;
                refreshRate = 10;
                isConnected = true;
                return true;
            }
            catch
            {
                isConnected = false;
                return false;
            }
        }

        public bool DisconnectPort()
        {
            try
            {
                sensorPort.Close();
                isConnected = false;
                portName = "";
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void StartMeasure()
        {
            try
            {
                sensorPort.WriteLine("start");
            }
            catch (System.IO.IOException)
            {
                // port timed out
                isConnected = false;
            }
            catch (System.InvalidOperationException)
            {
                // port is not open
                isConnected = false;
            }
        }

        public void StopMeasure()
        {
            try
            {
                sensorPort.WriteLine("stop");
            }
            catch (System.IO.IOException)
            {
                isConnected = false;
            }
            catch (System.InvalidOperationException)
            {
                isConnected = false;
            }
        }

        public void SaveSettings(int _accelLimit, int _gyroLimit, int _refreshRate)
        {
            accelLimit = _accelLimit;
            gyroLimit = _gyroLimit;
            refreshRate = _refreshRate;

            if (isConnected)
            {
                // sleep is bad (unresponsive), but we have to give the sensor time to catch up (TODO: consider using something that does not lock up the whole thread)
                sensorPort.WriteLine("accel_fs " + accelLimit.ToString());
                System.Threading.Thread.Sleep(20);
                sensorPort.WriteLine("gyro_fs " + gyroLimit.ToString());
                System.Threading.Thread.Sleep(20);
                sensorPort.WriteLine("refresh " + refreshRate.ToString());
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

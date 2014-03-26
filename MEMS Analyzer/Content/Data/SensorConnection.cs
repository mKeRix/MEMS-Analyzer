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
        public int accelLimit { get; private set; }
        public int gyroLimit { get; private set; }
        public int internRefreshRate { get; private set; }
        private int b_refreshRate;
        public int refreshRate
        {
            get { return b_refreshRate; }
            private set
            {
                b_refreshRate = value;
                internRefreshRate = value / 10;
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
            catch (System.TimeoutException)
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
            catch (System.TimeoutException)
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

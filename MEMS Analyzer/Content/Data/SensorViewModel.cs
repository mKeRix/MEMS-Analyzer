using CsvHelper.Configuration;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MEMS_Analyzer.Content.Data
{
    public class SensorViewModel : INotifyPropertyChanged
    {
        public SensorViewModel()
        {
            sensorConn = new SensorConnection();
            bufferCache = "";
            sensorConn.sensorPort.DataReceived += sensorPort_DataReceived;
            sensorConn.PropertyChanged += sensorConn_PropertyChanged;

            dataItems = new ObservableCollection<SensorData>();
            tmp_dataItems = new ObservableCollection<SensorData>(dataItems);
            dataItems.CollectionChanged += dataItems_CollectionChanged;
        }

        public SensorConnection sensorConn { get; set; }

        void sensorConn_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("sensorConn");
        }

        private ObservableCollection<SensorData> tmp_dataItems;
        private ObservableCollection<SensorData> _dataItems;
        public ObservableCollection<SensorData> dataItems
        {
            get { return _dataItems; }
            set
            {
                _dataItems = value;
                // notify of changes
                NotifyPropertyChanged("dataItems");
                NotifyPropertyChanged("lastItem");
            }
        }

        public SensorData lastItem
        {
            get 
            {
                if (dataItems.Count > 0)
                    return dataItems.Last();
                else
                    return new SensorData();
            }
        }

        private string bufferCache;
        void sensorPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // fill our own cache with the SerialPort data
            bufferCache += sensorConn.sensorPort.ReadExisting();
            var bufferArray = bufferCache.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);

            if (sensorConn.needStatus)
            {
                // incredibily hacky and hardcoded (bad!) way to grab the settings, but the format supplied isn't really helpful
                if (bufferArray.Count() >= 11)
                {
                    // read accel limit
                    Regex regSettings = new Regex(@"Beschleunigungsgrenze: (\d+)g");
                    var matchSettings = regSettings.Match(bufferArray[8]);
                    if (matchSettings.Success)
                    {
                        var groupSettings = matchSettings.Groups;
                        sensorConn.accelLimit = int.Parse(groupSettings[1].Captures[0].Value);
                    }
                    // read gyro limit
                    regSettings = new Regex(@"Gyroscopegrenze: (\d+)Grad/s");
                    matchSettings = regSettings.Match(bufferArray[9]);
                    if (matchSettings.Success)
                    {
                        var groupSettings = matchSettings.Groups;
                        sensorConn.gyroLimit = int.Parse(groupSettings[1].Captures[0].Value);
                    }
                    // read refresh rate
                    regSettings = new Regex(@"Wiederholungsrate: (\d+)Hz");
                    matchSettings = regSettings.Match(bufferArray[10]);
                    if (matchSettings.Success)
                    {
                        var groupSettings = matchSettings.Groups;
                        sensorConn.refreshRate = int.Parse(groupSettings[1].Captures[0].Value);
                    }

                    // empty our cache
                    bufferCache = "";
                    sensorConn.needStatus = false;
                }
            }
            else
            {
                // check if it is a complete and valid dataset
                foreach (string bufferLine in bufferArray)
                {
                    var bufferLineArray = bufferLine.Split(';');

                    if (bufferLineArray.Length == 13)
                    {
                        // clear collection if measurement is restarted
                        if (bufferLineArray[0] == "1")
                            dataItems.Clear();

                        // check if it is a complete and valid dataset
                        try
                        {
                            // ignore the last part of the array, as it is an escape sequence and cannot be converted to double
                            var dataFragments = bufferLineArray.Take(bufferLineArray.Length - 1).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToList();
                            dataItems.Add(new SensorData { id = (int)dataFragments[0], accelX = dataFragments[1], accelY = dataFragments[2], accelZ = dataFragments[3], gyroX = dataFragments[4], gyroY = dataFragments[5], gyroZ = dataFragments[6], magnetoX = dataFragments[7], magnetoY = dataFragments[8], magnetoZ = dataFragments[9], airPressure = dataFragments[10], airTemp = dataFragments[11], time = (double)dataFragments[0] / sensorConn.refreshRate });
                        }
                        catch (System.FormatException)
                        {
                            // TODO: add error handling
                        }
                    }
                    else
                    {
                        bufferCache = bufferLine;
                    }
                }
            }
        }

        // handle property changes
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        void dataItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("lastItem");

            if ((lastItem.id % sensorConn.internRefreshRate) == 0)
            {
                // create a secure copy that is updated at 10 Hz, otherwise plotting engine will complain
                tmp_dataItems = new ObservableCollection<SensorData>(dataItems);

                // update any charts
                NotifyPropertyChanged("AccelXData");
                NotifyPropertyChanged("AccelYData");
                NotifyPropertyChanged("AccelZData");
                NotifyPropertyChanged("AccelXMeterData");
                NotifyPropertyChanged("AccelYMeterData");
                NotifyPropertyChanged("AccelZMeterData");
                NotifyPropertyChanged("AccelSumData");
                NotifyPropertyChanged("AccelSumHorData");
                NotifyPropertyChanged("AccelSumMeterData");
                NotifyPropertyChanged("AccelSumHorData");

                NotifyPropertyChanged("GyroXData");
                NotifyPropertyChanged("GyroYData");
                NotifyPropertyChanged("GyroZData");

                NotifyPropertyChanged("MagnetoXData");
                NotifyPropertyChanged("MagnetoYData");
                NotifyPropertyChanged("MagnetoZData");

                NotifyPropertyChanged("PressureData");
                NotifyPropertyChanged("TemperatureData");
            }
        }

        // All the diagram sources

        private CompositeDataSource _AccelXData;
        public CompositeDataSource AccelXData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.accelX));
                yData.SetYMapping(y => y);
                _AccelXData = xData.Join(yData);
                return _AccelXData;
            }
        }

        private CompositeDataSource _AccelYData;
        public CompositeDataSource AccelYData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.accelY));
                yData.SetYMapping(y => y);
                _AccelYData = xData.Join(yData);
                return _AccelYData;
            }
        }

        private CompositeDataSource _AccelZData;
        public CompositeDataSource AccelZData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.accelZ));
                yData.SetYMapping(y => y);
                _AccelZData = xData.Join(yData);
                return _AccelZData;
            }
        }

        private CompositeDataSource _AccelXMeterData;
        public CompositeDataSource AccelXMeterData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.accelXMeter));
                yData.SetYMapping(y => y);
                _AccelXMeterData = xData.Join(yData);
                return _AccelXMeterData;
            }
        }

        private CompositeDataSource _AccelYMeterData;
        public CompositeDataSource AccelYMeterData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.accelYMeter));
                yData.SetYMapping(y => y);
                _AccelYMeterData = xData.Join(yData);
                return _AccelYMeterData;
            }
        }

        private CompositeDataSource _AccelZMeterData;
        public CompositeDataSource AccelZMeterData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.accelZMeter));
                yData.SetYMapping(y => y);
                _AccelZMeterData = xData.Join(yData);
                return _AccelZMeterData;
            }
        }

        private CompositeDataSource _AccelSumData;
        public CompositeDataSource AccelSumData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.accelSum));
                yData.SetYMapping(y => y);
                _AccelSumData = xData.Join(yData);
                return _AccelSumData;
            }
        }

        private CompositeDataSource _AccelSumHorData;
        public CompositeDataSource AccelSumHorData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.accelSumHorizontal));
                yData.SetYMapping(y => y);
                _AccelSumHorData = xData.Join(yData);
                return _AccelSumHorData;
            }
        }

        private CompositeDataSource _AccelSumMeterData;
        public CompositeDataSource AccelSumMeterData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.accelSumMeter));
                yData.SetYMapping(y => y);
                _AccelSumMeterData = xData.Join(yData);
                return _AccelSumMeterData;
            }
        }

        private CompositeDataSource _AccelSumHorMeterData;
        public CompositeDataSource AccelSumHorMeterData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.accelSumHorizontalMeter));
                yData.SetYMapping(y => y);
                _AccelSumHorMeterData = xData.Join(yData);
                return _AccelSumHorMeterData;
            }
        }

        private CompositeDataSource _GyroXData;
        public CompositeDataSource GyroXData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.gyroX));
                yData.SetYMapping(y => y);
                _GyroXData = xData.Join(yData);
                return _GyroXData;
            }
        }

        private CompositeDataSource _GyroYData;
        public CompositeDataSource GyroYData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.gyroY));
                yData.SetYMapping(y => y);
                _GyroYData = xData.Join(yData);
                return _GyroYData;
            }
        }

        private CompositeDataSource _GyroZData;
        public CompositeDataSource GyroZData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.gyroZ));
                yData.SetYMapping(y => y);
                _GyroZData = xData.Join(yData);
                return _GyroZData;
            }
        }

        private CompositeDataSource _MagnetoXData;
        public CompositeDataSource MagnetoXData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.magnetoX));
                yData.SetYMapping(y => y);
                _MagnetoXData = xData.Join(yData);
                return _MagnetoXData;
            }
        }

        private CompositeDataSource _MagnetoYData;
        public CompositeDataSource MagnetoYData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.magnetoY));
                yData.SetYMapping(y => y);
                _MagnetoYData = xData.Join(yData);
                return _MagnetoYData;
            }
        }

        private CompositeDataSource _MagnetoZData;
        public CompositeDataSource MagnetoZData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.magnetoZ));
                yData.SetYMapping(y => y);
                _MagnetoZData = xData.Join(yData);
                return _MagnetoZData;
            }
        }

        private CompositeDataSource _PressureData;
        public CompositeDataSource PressureData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.airPressure));
                yData.SetYMapping(y => y);
                _PressureData = xData.Join(yData);
                return _PressureData;
            }
        }

        private CompositeDataSource _TemperatureData;
        public CompositeDataSource TemperatureData
        {
            get
            {
                var xData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.time));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(tmp_dataItems.Select(v => v.airTemp));
                yData.SetYMapping(y => y);
                _TemperatureData = xData.Join(yData);
                return _TemperatureData;
            }
        }
    }

    public class SensorClassMap : CsvClassMap<SensorData>
    {
        [Obsolete("The parent of this method is obsolete and will be removed in the next release.")]
        public override void CreateMap()
        {
            Map(m => m.id);
            Map(m => m.accelX);
            Map(m => m.accelY);
            Map(m => m.accelZ);
            Map(m => m.gyroX);
            Map(m => m.gyroY);
            Map(m => m.gyroZ);
            Map(m => m.magnetoX);
            Map(m => m.magnetoY);
            Map(m => m.magnetoZ);
            Map(m => m.airPressure);
            Map(m => m.airTemp);
        }
    }
}

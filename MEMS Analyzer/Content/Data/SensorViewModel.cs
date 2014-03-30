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

        private string bufferCache;
        void sensorPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // fill our own cache with the SerialPort data
            bufferCache += sensorConn.sensorPort.ReadExisting();
            var bufferArray = bufferCache.Split(new string[]{"\r\n"}, System.StringSplitOptions.None);

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
            }
        }
    }

    public class SensorClassMap : CsvClassMap<SensorData>
    {
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

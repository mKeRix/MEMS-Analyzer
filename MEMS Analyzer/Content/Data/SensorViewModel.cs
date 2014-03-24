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
            sensorConn.sensorPort.DataReceived += sensorPort_DataReceived;
            sensorConn.PropertyChanged += sensorConn_PropertyChanged;

            dataItems = new ObservableCollection<SensorData>();
            dataItems.CollectionChanged += dataItems_CollectionChanged;

            // LoadData();
        }

        public SensorConnection sensorConn { get; set; }

        void sensorConn_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("sensorConn");
        }


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

        private void LoadData()
        {
            dataItems.Add(new SensorData { id = 0, accelX = 0.231, accelY = 0.345, accelZ = -1.234 });
            dataItems.Add(new SensorData { id = 1, accelX = 1.231, accelY = 1.345, accelZ = -0.234 });
        }

        private CompositeDataSource _AccelXData;
        public CompositeDataSource AccelXData
        {
            get
            {
                var xData = new EnumerableDataSource<int>(dataItems.Select(v => v.id));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(dataItems.Select(v => v.accelX));
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
                var xData = new EnumerableDataSource<int>(dataItems.Select(v => v.id));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(dataItems.Select(v => v.accelY));
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
                var xData = new EnumerableDataSource<int>(dataItems.Select(v => v.id));
                xData.SetXMapping(x => x);
                var yData = new EnumerableDataSource<double>(dataItems.Select(v => v.accelZ));
                yData.SetYMapping(y => y);
                _AccelZData = xData.Join(yData);
                return _AccelZData;
            }
        }

        void sensorPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string bufferLine = sensorConn.sensorPort.ReadLine();
            var bufferArray = bufferLine.Split(';');

            // check if it is a complete and valid dataset
            if (bufferArray.Length == 13)
            {
                // clear collection if measurement is restarted
                if (bufferArray[0] == "1")
                    dataItems.Clear();

                // ignore the last part of the array, as it is an escape sequence and cannot be converted to double
                var dataFragments = bufferArray.Take(bufferArray.Length - 1).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToList();
                dataItems.Add(new SensorData { id = (int)dataFragments[0], accelX = dataFragments[1], accelY = dataFragments[2], accelZ = dataFragments[3], gyroX = dataFragments[4], gyroY = dataFragments[5], gyroZ = dataFragments[6], magnetoX = dataFragments[7], magnetoY = dataFragments[8], magnetoZ = dataFragments[9], airPressure = dataFragments[10], airTemp = dataFragments[11] });
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
            NotifyPropertyChanged("dataItems");
            NotifyPropertyChanged("lastItem");

            // update any charts
            NotifyPropertyChanged("AccelXData");
            NotifyPropertyChanged("AccelYData");
            NotifyPropertyChanged("AccelZData");
        }
    }
}

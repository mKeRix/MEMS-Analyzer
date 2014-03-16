using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEMS_Analyzer.Content.Data
{
    public class SensorViewModel : INotifyPropertyChanged
    {
        public SensorViewModel()
        {
            dataItems = new ObservableCollection<SensorData>();
            dataItems.CollectionChanged += dataItems_CollectionChanged;

            LoadData();
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
            get { return dataItems.Last(); }
        }

        public void LoadData()
        {
            dataItems.Add(new SensorData { id = 0, accelX = 0.231, accelY = 0.345, accelZ = -1.234 });
            dataItems.Add(new SensorData { id = 1, accelX = 1.231, accelY = 1.345, accelZ = -0.234 });
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
        }
    }
}

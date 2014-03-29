using CsvHelper;
using MEMS_Analyzer.Content.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MEMS_Analyzer.Pages.Data
{
    /// <summary>
    /// Interaction logic for General.xaml
    /// </summary>
    public partial class General : UserControl
    {
        public General()
        {
            InitializeComponent();
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SensorViewModel)this.DataContext;
            viewModel.sensorConn.StartMeasure();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SensorViewModel)this.DataContext;
            viewModel.sensorConn.StopMeasure();
        }

        private void ButtonSaveFile_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SensorViewModel)DataContext;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV Dateien (*.csv)|*.csv";
            dialog.Title = "CSV Datei speichern";
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                StreamWriter writer = new StreamWriter(dialog.FileName);
                writer.WriteLine(String.Format("Sensor: 2    Frequenz: {0}Hz    Beschleunigungsgrenze: {1}g    Gyroscopegrenze: {2}Grad/s", viewModel.sensorConn.refreshRate, viewModel.sensorConn.accelLimit, viewModel.sensorConn.gyroLimit));
                writer.WriteLine("");

                using (var csv = new CsvWriter(writer))
                {
                    csv.Configuration.RegisterClassMap<SensorClassMap>();
                    csv.Configuration.Delimiter = ";";
                    csv.Configuration.QuoteNoFields = true;
                    csv.Configuration.HasHeaderRecord = false;
                    csv.WriteRecords(viewModel.dataItems);
                }

                writer.Close();
                writer.Dispose();
            }
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SensorViewModel)DataContext;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSV Dateien (*.csv)|*.csv";
            dialog.Title = "CSV Datei öffnen";
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                StreamReader reader = new StreamReader(dialog.FileName);

                // read first line as it cointains config values
                Regex regSettings = new Regex(@"Sensor: (\d+)    Frequenz: (\d+)Hz    Beschleunigungsgrenze: (\d+)g    Gyroscopegrenze: (\d+)Grad/s");
                var matchSettings = regSettings.Match(reader.ReadLine());
                if (matchSettings.Success)
                {
                    var groupSettings = matchSettings.Groups;
                    viewModel.sensorConn.refreshRate = int.Parse(groupSettings[2].Captures[0].Value);
                    viewModel.sensorConn.accelLimit = int.Parse(groupSettings[3].Captures[0].Value);
                    viewModel.sensorConn.gyroLimit = int.Parse(groupSettings[4].Captures[0].Value);
                }

                // before we add anything, clear the old data
                viewModel.dataItems.Clear();

                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.RegisterClassMap<SensorClassMap>();
                    csv.Configuration.Delimiter = ";";
                    csv.Configuration.QuoteNoFields = true;
                    csv.Configuration.HasHeaderRecord = false;
                    csv.Configuration.SkipEmptyRecords = true;
                    csv.Configuration.CultureInfo = System.Globalization.CultureInfo.InvariantCulture;

                    while (csv.Read())
                    {
                        viewModel.dataItems.Add(csv.GetRecord<SensorData>());
                        viewModel.lastItem.time = (double)viewModel.lastItem.id / viewModel.sensorConn.refreshRate;
                    }
                }

                reader.Close();
                reader.Dispose();
            }
        }
    }
}

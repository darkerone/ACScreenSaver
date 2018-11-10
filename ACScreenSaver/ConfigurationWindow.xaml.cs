using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ACScreenSaver
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        private string _configurationFilePath = @"ACSS_Configuration.acss";
        private ConfigurationModel _configurationModel;

        public ConfigurationWindow()
        {
            InitializeComponent();

            _configurationModel = new ConfigurationModel();
            _configurationModel.RestoreConfiguration();

            this.ImagesDirectoryPath_TextBlock.Text = _configurationModel.ImagesDirectoryPath;
            this.IntervalTime_IntegerUpDown.Value = _configurationModel.IntervalTime;
            this.IsRandom_CheckBox.IsChecked = _configurationModel.IsRandom;
        }

        private void Button_Parcourir_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                this.ImagesDirectoryPath_TextBlock.Text = fbd.SelectedPath;
            }
        }

        private void Button_Annuler_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Button_Valider_Click(object sender, RoutedEventArgs e)
        {
            _configurationModel.ImagesDirectoryPath = this.ImagesDirectoryPath_TextBlock.Text;
            _configurationModel.IntervalTime = this.IntervalTime_IntegerUpDown.Value.Value * 1000;
            _configurationModel.IsRandom = this.IsRandom_CheckBox.IsChecked.Value;

            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string jsonConfiguration = JsonConvert.SerializeObject(_configurationModel, jsonSerializerSettings);
            System.IO.File.WriteAllText(_configurationFilePath, jsonConfiguration);
            System.Windows.Application.Current.Shutdown();
        }
    }
}

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
        private ConfigurationModel _configurationModel;
        private bool _isDialogWindow = false;

        public ConfigurationWindow(bool isDialogWindow = false)
        {
            _isDialogWindow = isDialogWindow;

            InitializeComponent();

            _configurationModel = new ConfigurationModel();
            _configurationModel.RestoreConfiguration();

            this.ImagesDirectoryPath_TextBlock.Text = _configurationModel.ImagesDirectoryPath;
            this.ImageDisplayDuration_IntegerUpDown.Value = _configurationModel.ImageDisplayDuration / 1000;
            this.IsRandom_CheckBox.IsChecked = _configurationModel.IsRandom;
            this.TimerDurationGap_IntegerUpDown.Value = _configurationModel.TimerDurationGap / 1000;
            this.TimerDisplayDuration_IntegerUpDown.Value = _configurationModel.TimerDisplayDuration / 1000;
            this.NumberOfSuccessiveSameFolderFiles_IntegerUpDown.Value = _configurationModel.NumberOfSuccessiveSameFolderFiles;
            this.PanoramaDisplayDuration_IntegerUpDown.Value = _configurationModel.PanoramaDisplayDuration / 1000;
            this.IsYearDisplayed_CheckBox.IsChecked = _configurationModel.IsYearDisplayed;
            this.DisplayInformationsDuration_IntegerUpDown.Value = _configurationModel.DisplayInformationDuration / 1000;
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
            if (_isDialogWindow)
            {
                this.Close();
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void Button_Valider_Click(object sender, RoutedEventArgs e)
        {
            _configurationModel.ImagesDirectoryPath = this.ImagesDirectoryPath_TextBlock.Text;
            _configurationModel.ImageDisplayDuration = this.ImageDisplayDuration_IntegerUpDown.Value.Value * 1000;
            _configurationModel.IsRandom = this.IsRandom_CheckBox.IsChecked.Value;
            _configurationModel.TimerDurationGap = this.TimerDurationGap_IntegerUpDown.Value.Value * 1000;
            _configurationModel.TimerDisplayDuration= this.TimerDisplayDuration_IntegerUpDown.Value.Value * 1000;
            _configurationModel.NumberOfSuccessiveSameFolderFiles = this.NumberOfSuccessiveSameFolderFiles_IntegerUpDown.Value.Value;
            _configurationModel.PanoramaDisplayDuration = this.PanoramaDisplayDuration_IntegerUpDown.Value.Value * 1000;
            _configurationModel.IsYearDisplayed = this.IsYearDisplayed_CheckBox.IsChecked.Value;
            _configurationModel.DisplayInformationDuration = this.DisplayInformationsDuration_IntegerUpDown.Value.Value * 1000;

            _configurationModel.SaveConfiguration();

            if (_isDialogWindow)
            {
                this.Close();
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}

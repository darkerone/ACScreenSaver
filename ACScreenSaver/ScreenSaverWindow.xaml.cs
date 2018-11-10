using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ACScreenSaver
{
    /// <summary>
    /// Logique d'interaction pour ScreenSaverWindow.xaml
    /// </summary>
    public partial class ScreenSaverWindow : Window
    {
        private bool _isFullDisplay = true;
        private Timer _timer;

        private ScreenSaverManager _screenSaverManager;

        public ScreenSaverWindow()
        {
            InitializeComponent();

            _screenSaverManager = new ScreenSaverManager();

            _timer = new System.Timers.Timer();
            _timer.Interval = _screenSaverManager.ConfigurationModel.IntervalTime * 1000;
            _timer.Elapsed += _timer_Elapsed;

            DisplayFullScreenSaver();
        }

        #region Events

        /// <summary>
        /// Au clic sur le bouton "Quitter l'écran de veille"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Quit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// Au clic sur le bouton "Réafficher l'écran de veille"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_FullDisplay_Click(object sender, RoutedEventArgs e)
        {
            DisplayFullScreenSaver();
        }

        /// <summary>
        /// Lorsque l'on relache une touche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenSaverWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (_isFullDisplay)
            {
                switch (e.Key)
                {
                    case Key.Space:
                        _timer.Enabled = !_timer.Enabled;
                        break;
                    case Key.Left:
                        _timer.Stop();
                        GoToPreviousImage();
                        _timer.Start();
                        break;
                    case Key.Right:
                        _timer.Stop();
                        GoToNextImage();
                        _timer.Start();
                        break;
                    default:
                        DisplayInformations();
                        break;
                }
            }
        }

        /// <summary>
        /// Lorsque l'on relache le clic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenSaver_Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isFullDisplay)
            {
                DisplayInformations();
            }
        }

        /// <summary>
        /// Quand le timer est écoulé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Le thread du timer n'est pas le même que le thread de l'UI donc on demande à l'UI de faire le travail
            this.Dispatcher.Invoke(() =>
            {
                GoToNextImage();
            });
        }

        private void Button_AddToDeleted(object sender, RoutedEventArgs e)
        {
            _screenSaverManager.AddCurrentImageToDeleted();
        }

        private void Button_AddToNotDisplayed(object sender, RoutedEventArgs e)
        {
            _screenSaverManager.AddCurrentImageToNotDisplayed();
        }

        #endregion

        /// <summary>
        /// Définit l'image à afficher
        /// </summary>
        /// <param name="uri"></param>
        private void SetScreenSaverImage(string uri)
        {
            if(uri != null)
            {
                ScreenSaver_Image.Source = new BitmapImage(new Uri(uri));
            }
            else
            {
                ScreenSaver_Image.Source = null;
            }
        }

        /// <summary>
        /// Affiche l'image en plein écran
        /// </summary>
        private void DisplayFullScreenSaver()
        {
            _isFullDisplay = true;
            GoToImageOfIndex(_screenSaverManager.GetCurrentImageIndex());
            _timer.Start();

            ScreenSaver_Window.WindowState = WindowState.Maximized;
            ScreenSaver_Window.WindowStyle = WindowStyle.None;
            Image_Block.Visibility = Visibility.Visible;
            Informations_Block.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Affiche les informations
        /// </summary>
        private void DisplayInformations()
        {
            _isFullDisplay = false;
            _timer.Stop();

            ScreenSaverInformation_Image.Source = ScreenSaver_Image.Source;
            string currentImagePath = _screenSaverManager.GetCurrentImagePath();
            if(currentImagePath != null)
            {
                FileInfo imageInfo = new FileInfo(currentImagePath);
                ImageDirectoryName_TextBlock.Text = imageInfo.FullName;
            }
            else
            {
                ImageDirectoryName_TextBlock.Text = "Aucune image n'est affichée";
            }

            ScreenSaver_Window.WindowState = WindowState.Normal;
            ScreenSaver_Window.WindowStyle = WindowStyle.SingleBorderWindow;
            Image_Block.Visibility = Visibility.Collapsed;
            Informations_Block.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Affiche l'image dont l'index est passé en paramètre
        /// </summary>
        /// <param name="imageFileIndex"></param>
        private void GoToImageOfIndex(int imageFileIndex)
        {

            _screenSaverManager.SetCurrentImageIndex(imageFileIndex);
            string nextImageFilePath = _screenSaverManager.GetCurrentImagePath();
            SetScreenSaverImage(nextImageFilePath);
        }

        /// <summary>
        /// Affiche l'image suivante
        /// </summary>
        private void GoToNextImage()
        {
            int imageFileIndex = _screenSaverManager.GetCurrentImageIndex();
            // Tant qu'on a pas le droit d'afficher l'image, on prend la suivante
            do
            {
                imageFileIndex = imageFileIndex + 1;

                if (_screenSaverManager.GetLastImageIndex() < imageFileIndex)
                {
                    imageFileIndex = 0;
                }
            } while (!_screenSaverManager.CanDisplayImageOfIndex(imageFileIndex));
            
            GoToImageOfIndex(imageFileIndex);
        }

        /// <summary>
        /// Affiche l'image précédante
        /// </summary>
        private void GoToPreviousImage()
        {
            int imageFileIndex = _screenSaverManager.GetCurrentImageIndex();
            // Tant qu'on a pas le droit d'afficher l'image, on prend la précédante
            do
            {
                imageFileIndex = imageFileIndex - 1;

                if (imageFileIndex < 0)
                {
                    imageFileIndex = _screenSaverManager.GetLastImageIndex();
                }
            } while (!_screenSaverManager.CanDisplayImageOfIndex(imageFileIndex));
            
            GoToImageOfIndex(imageFileIndex);
        }
    }
}

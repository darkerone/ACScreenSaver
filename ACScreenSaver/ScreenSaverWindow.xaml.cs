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
        /// <summary>
        /// Définit si on est en plein écran ou non
        /// </summary>
        private bool _isFullDisplay = true;

        /// <summary>
        /// Timer pour gérer le temps d'affichage des photos
        /// </summary>
        private Timer _imageTimer;

        /// <summary>
        /// Définit si l'interval du timer des images a été modifié
        /// </summary>
        private bool _isIntervalTimerModified = false;

        /// <summary>
        /// Timer pour gérer le temps d'affichage du timer à l'écran
        /// </summary>
        private Timer _displayIntervalTimerTimer;

        private ScreenSaverManager _screenSaverManager;

        public ScreenSaverWindow()
        {
            InitializeComponent();

            _screenSaverManager = new ScreenSaverManager();

            _imageTimer = new System.Timers.Timer();
            _imageTimer.Interval = _screenSaverManager.ConfigurationModel.IntervalTime;
            _imageTimer.Elapsed += _timer_Elapsed;

            _displayIntervalTimerTimer = new System.Timers.Timer();

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
                        _imageTimer.Enabled = !_imageTimer.Enabled;
                        break;
                    case Key.Left:
                        _imageTimer.Stop();
                        GoToPreviousImage();
                        _imageTimer.Start();
                        break;
                    case Key.Right:
                        _imageTimer.Stop();
                        GoToNextImage();
                        _imageTimer.Start();
                        break;
                    case Key.Up:
                        _imageTimer.Stop();
                        TemporarilyIncreaseTimer();
                        DisplayTimerInterval(_screenSaverManager.ConfigurationModel.DisplayIntervalTimeTime);
                        _imageTimer.Start();
                        break;
                    case Key.Down:
                        _imageTimer.Stop();
                        TemporarilyDecreaseTimer();
                        DisplayTimerInterval(_screenSaverManager.ConfigurationModel.DisplayIntervalTimeTime);
                        _imageTimer.Start();
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

        private void Button_AddToDeleted_Click(object sender, RoutedEventArgs e)
        {
            _screenSaverManager.AddCurrentImageToDeleted();
            Button_AddToDeleted.IsEnabled = false;
        }

        private void Button_AddToNotDisplayed_Click(object sender, RoutedEventArgs e)
        {
            _screenSaverManager.AddCurrentImageToNotDisplayed();
            Button_AddToNotDisplayed.IsEnabled = false;
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
            _imageTimer.Start();

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
            _imageTimer.Stop();

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
            Button_AddToDeleted.IsEnabled = true;
            Button_AddToNotDisplayed.IsEnabled = true;
        }

        /// <summary>
        /// Affiche l'image dont l'index est passé en paramètre
        /// </summary>
        /// <param name="imageFileIndex"></param>
        private void GoToImageOfIndex(int imageFileIndex)
        {
            // Si l'interval du timer a été modifié
            if (_isIntervalTimerModified)
            {
                // On lui redonne sa valeur initiale
                _imageTimer.Interval = _screenSaverManager.ConfigurationModel.IntervalTime;
                DisplayTimerInterval(0);
            }
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

        /// <summary>
        /// Augmente temporairement (le temps d'une image) l'interval du timer
        /// </summary>
        private void TemporarilyIncreaseTimer()
        {
            _imageTimer.Interval = _imageTimer.Interval + _screenSaverManager.ConfigurationModel.IntervalTimeGap;
            _isIntervalTimerModified = true;
        }

        /// <summary>
        /// Diminue temporairement (le temps d'une image) l'interval du timer
        /// </summary>
        private void TemporarilyDecreaseTimer()
        {
            int newTimerInterval = (int)_imageTimer.Interval - _screenSaverManager.ConfigurationModel.IntervalTimeGap;
            if(newTimerInterval < 1000)
            {
                newTimerInterval = 1000;
            }
            _imageTimer.Interval = newTimerInterval;
            _isIntervalTimerModified = true;
        }

        /// <summary>
        /// Affiche l'interval du timer pendant le temps demandé
        /// </summary>
        /// <param name="displayTime">Temps (en seconde) d'affichage. Infini si null.</param>
        private void DisplayTimerInterval(int? displayTime = null)
        {
            _displayIntervalTimerTimer.Stop();
            if (displayTime == null)
            {
                Timer_TextBlock.Text = (_imageTimer.Interval / 1000).ToString();
                Timer_TextBlock.Visibility = Visibility.Visible;
            }
            else if (displayTime > 0)
            {
                Timer_TextBlock.Text = (_imageTimer.Interval / 1000).ToString();
                Timer_TextBlock.Visibility = Visibility.Visible;
                _displayIntervalTimerTimer.Interval = displayTime.Value * 1000;
                _displayIntervalTimerTimer.Elapsed += _displayIntervalTimerTimer_Elapsed;
                _displayIntervalTimerTimer.Start();
            }
            else
            {
                Timer_TextBlock.Visibility = Visibility.Hidden;
            }
        }

        private void _displayIntervalTimerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Le thread du timer n'est pas le même que le thread de l'UI donc on demande à l'UI de faire le travail
            this.Dispatcher.Invoke(() =>
            {
                DisplayTimerInterval(0);
            });
        }
    }
}

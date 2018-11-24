using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Animation;
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
        /// Timer pour gérer le temps d'affichage des photos
        /// </summary>
        private Timer _imageTimer;

        /// <summary>
        /// Définit si l'interval du timer des images a été modifié
        /// </summary>
        private bool _isTimerDurationModified = false;

        /// <summary>
        /// Timer pour gérer le temps d'affichage du timer à l'écran
        /// </summary>
        private Timer _timerDurationDisplayTimer;

        /// <summary>
        /// Timer pour gérer le temps d'affichage des informations
        /// </summary>
        private Timer _informationsDisplayTimer;

        public static Stopwatch _stopwatch = new Stopwatch();

        private ScreenSaverManager _screenSaverManager;

        public ScreenSaverWindow(ScreenSaverManager screenSaverManager)
        {
            InitializeComponent();

            _screenSaverManager = screenSaverManager;

            _imageTimer = new Timer();
            SetImageTimerDuration(_screenSaverManager.Configuration.ImageDisplayDuration);
            _imageTimer.Elapsed += _timer_Elapsed;

            _timerDurationDisplayTimer = new Timer();
            _timerDurationDisplayTimer.Elapsed += _timerDurationDisplayTimer_Elapsed;

            _informationsDisplayTimer = new Timer();
            _informationsDisplayTimer.Elapsed += _informationsDisplayTimer_Elapsed;

            Informations_Grid.Visibility = Visibility.Hidden;
            Timer_TextBlock.Visibility = Visibility.Hidden;
            Year_TextBlock.Visibility = Visibility.Hidden;

            _stopwatch.Start();
        }

        #region Events

        /// <summary>
        /// Lorsque l'on relache une touche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenSaverWindow_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    _imageTimer.Enabled = !_imageTimer.Enabled;
                    break;
                case Key.Left:
                    _imageTimer.Stop();
                    _screenSaverManager.GoToPreviousImage();
                    _imageTimer.Start();
                    break;
                case Key.Right:
                    _imageTimer.Stop();
                    _screenSaverManager.GoToNextImage();
                    _imageTimer.Start();
                    break;
                case Key.Up:
                    _imageTimer.Stop();
                    TemporarilyIncreaseTimer();
                    DisplayTimerDuration(_screenSaverManager.Configuration.TimerDisplayDuration);
                    _imageTimer.Start();
                    break;
                case Key.Down:
                    _imageTimer.Stop();
                    TemporarilyDecreaseTimer();
                    DisplayTimerDuration(_screenSaverManager.Configuration.TimerDisplayDuration);
                    _imageTimer.Start();
                    break;
                default:
                    _screenSaverManager.DisplayInformationsWindow();
                    break;
            }
        }

        /// <summary>
        /// Lorsque l'on relache le clic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenSaver_Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _screenSaverManager.DisplayInformationsWindow();
        }

        /// <summary>
        /// Lorsque la souris bouge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScreenSaver_Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_stopwatch.ElapsedTicks % 50L == 0L)
            {
                DisplayInformations(_screenSaverManager.Configuration.DisplayInformationDuration);
            }
        }

        /// <summary>
        /// Quand le timer des images est écoulé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Le thread du timer n'est pas le même que le thread de l'UI donc on demande à l'UI de faire le travail
            this.Dispatcher.Invoke(() =>
            {
                _imageTimer.Stop();
                _screenSaverManager.GoToNextImage();
            });
        }

        /// <summary>
        /// Quand le timer d'affichage du timer est écoulé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timerDurationDisplayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Le thread du timer n'est pas le même que le thread de l'UI donc on demande à l'UI de faire le travail
            this.Dispatcher.Invoke(() =>
            {
                DisplayTimerDuration(0);
            });
        }

        /// <summary>
        /// Quand le timer d'affichage des informations est écoulé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _informationsDisplayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Le thread du timer n'est pas le même que le thread de l'UI donc on demande à l'UI de faire le travail
            this.Dispatcher.Invoke(() =>
            {
                DisplayInformations(0);
            });
        }

        #endregion

        /// <summary>
        /// Définit l'image à afficher
        /// </summary>
        /// <param name="uri"></param>
        public void SetScreenSaverImage(string uri)
        {
            try
            {
                this.Cursor = Cursors.None;

                // Si l'interval du timer a été modifié
                if (_isTimerDurationModified)
                {
                    // On lui redonne sa valeur initiale
                    SetImageTimerDuration(_screenSaverManager.Configuration.ImageDisplayDuration);
                    DisplayTimerDuration(0);
                }

                if (uri == null)
                {
                    throw new Exception("Chemin de l'image manquant");
                }

                Logger.LogDebug("Affichage de l'image : " + uri);
                ScreenSaver_Image.Source = new BitmapImage(new Uri(uri));

                // Réinitialise le zoom
                Zoom_Border.Reset();

                System.Drawing.Image imgDrawing = System.Drawing.Image.FromFile(uri);
                Zoom_Border.AutoRotateImage(imgDrawing);

                // Panorama
                // Ratios
                double imageRatio = (float)imgDrawing.Width / (float)imgDrawing.Height;
                double screenRation = SystemParameters.WorkArea.Width / SystemParameters.WorkArea.Height;
                if (screenRation < imageRatio)
                {
                    SetImageTimerDuration(_screenSaverManager.Configuration.PanoramaDisplayDuration);
                    _isTimerDurationModified = true;
                    Zoom_Border.MakeImagePanorama(imgDrawing, _screenSaverManager.Configuration.PanoramaDisplayDuration);
                }

                // Affichage de l'année
                if (_screenSaverManager.Configuration.IsYearDisplayed)
                {
                    Year_TextBlock.Visibility = Visibility.Visible;
                    Year_TextBlock.Text = _screenSaverManager.GetCurrentImageYear();
                }
                else
                {
                    Year_TextBlock.Visibility = Visibility.Hidden;
                }

                // Affichage des informations
                ImagePath_TextBlock.Text = uri;

                _imageTimer.Start();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                _screenSaverManager.GoToNextImage();
                return;
            }
            
        }

        public void HideFullScreenSaver() {
            _imageTimer.Stop();
            this.Hide();
        }

        /// <summary>
        /// Augmente temporairement (le temps d'une image) l'interval du timer
        /// </summary>
        private void TemporarilyIncreaseTimer()
        {
            SetImageTimerDuration(_imageTimer.Interval + _screenSaverManager.Configuration.TimerDurationGap);
            _isTimerDurationModified = true;
        }

        /// <summary>
        /// Diminue temporairement (le temps d'une image) l'interval du timer
        /// </summary>
        private void TemporarilyDecreaseTimer()
        {
            int newTimerDuration = (int)_imageTimer.Interval - _screenSaverManager.Configuration.TimerDurationGap;
            if (newTimerDuration < 1000)
            {
                newTimerDuration = 1000;
            }
            SetImageTimerDuration(newTimerDuration);
            _isTimerDurationModified = true;
        }

        /// <summary>
        /// Affiche l'interval du timer pendant le temps demandé
        /// </summary>
        /// <param name="displayTime">Temps (en milliseconde) d'affichage. Infini si null.</param>
        private void DisplayTimerDuration(int? displayTime = null)
        {
            _timerDurationDisplayTimer.Stop();
            if (displayTime == null)
            {
                Timer_TextBlock.Text = (_imageTimer.Interval / 1000).ToString();
                Timer_TextBlock.Visibility = Visibility.Visible;
            }
            else if (displayTime > 0)
            {
                Timer_TextBlock.Text = (_imageTimer.Interval / 1000).ToString();
                Timer_TextBlock.Visibility = Visibility.Visible;
                _timerDurationDisplayTimer.Interval = displayTime.Value;
                _timerDurationDisplayTimer.Start();
            }
            else
            {
                Timer_TextBlock.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Définit la durée d'affichage d'une image
        /// </summary>
        /// <param name="duration">Durée en milisecondes</param>
        private void SetImageTimerDuration(double duration)
        {
            _imageTimer.Interval = duration < 1000 ? 1000 : duration;
        }

        /// <summary>
        /// Affiche les informations pendant le temps demandé
        /// </summary>
        /// <param name="displayTime">Temps (en milliseconde) d'affichage. Infini si null.</param>
        private void DisplayInformations(int? displayTime = null)
        {
            _informationsDisplayTimer.Stop();
            if (displayTime == null)
            {
                Informations_Grid.Visibility = Visibility.Visible;
            }
            else if (displayTime > 0)
            {
                Informations_Grid.Visibility = Visibility.Visible;
                _informationsDisplayTimer.Interval = displayTime.Value;
                _informationsDisplayTimer.Start();
            }
            else
            {
                Informations_Grid.Visibility = Visibility.Hidden;
            }
        }
    }
}

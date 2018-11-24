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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ACScreenSaver
{
    /// <summary>
    /// Logique d'interaction pour ScreenSaverInfoWindow.xaml
    /// </summary>
    public partial class InformationsWindow : Window
    {
        private ScreenSaverManager _screenSaverManager;

        public InformationsWindow(ScreenSaverManager screenSaverManager)
        {
            InitializeComponent();

            _screenSaverManager = screenSaverManager;
        }
        
        /// <summary>
        /// Affiche les informations
        /// </summary>
        public void SetImageInformations(string uri)
        {
            try
            {
                if (uri == null)
                {
                    throw new Exception("Aucune image n'est affichée");
                }

                ScreenSaverInformation_Image.Source = new BitmapImage(new Uri(uri));
                System.Drawing.Image imgDrawing = System.Drawing.Image.FromFile(uri);
                //ImageHelper.RotateImage(ScreenSaverInformation_Image, ImageHelper.GetRotation(imgDrawing));
                FileInfo imageInfo = new FileInfo(uri);
                ImageDirectoryName_TextBlock.Text = imageInfo.FullName;

                Button_AddToDeleted.IsEnabled = true;
                Button_AddToNotDisplayed.IsEnabled = true;
                
            }
            catch (Exception ex) {
                Logger.LogError(ex.Message);
                ImageDirectoryName_TextBlock.Text = "Aucune image n'est affichée";
                Button_AddToDeleted.IsEnabled = false;
                Button_AddToNotDisplayed.IsEnabled = false;
            }
            this.Cursor = Cursors.Arrow;
        }

        public void HideImageInformations()
        {
            this.Hide();
        }

        /// <summary>
        /// Au clic sur le bouton "A supprimer"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AddToDeleted_Click(object sender, RoutedEventArgs e)
        {
            _screenSaverManager.AddCurrentImageToDeleted();
            Button_AddToDeleted.IsEnabled = false;
        }

        /// <summary>
        /// Au clic sur le bouton "Ne plus afficher"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AddToNotDisplayed_Click(object sender, RoutedEventArgs e)
        {
            _screenSaverManager.AddCurrentImageToNotDisplayed();
            Button_AddToNotDisplayed.IsEnabled = false;
        }

        /// <summary>
        /// Au clic sur le bouton "Légende"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Legende_Click(object sender, RoutedEventArgs e)
        {
            LegendeWindow legendeWindow = new LegendeWindow();
            legendeWindow.ShowDialog();
        }

        /// <summary>
        /// Au clic sur le bouton "Précédente"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Precedent_Click(object sender, RoutedEventArgs e)
        {
            _screenSaverManager.GoToPreviousImage();
        }

        /// <summary>
        /// Au clic sur le bouton "Suivante"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Suivant_Click(object sender, RoutedEventArgs e)
        {
            _screenSaverManager.GoToNextImage();
        }

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
            _screenSaverManager.DisplayScreenSaverWindow();
        }
    }
}

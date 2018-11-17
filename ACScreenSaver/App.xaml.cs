using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ACScreenSaver
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            try
            {
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ACScreenSaver");
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            // Preview mode--display in little window in Screen Saver dialog
            // (Not invoked with Preview button, which runs Screen Saver in
            // normal /s mode).
            if (e.Args[0].ToLower().StartsWith("/p"))
            {
                Logger.LogDebug("Mode preview");
                //PreviewWindow previewWindow = new PreviewWindow();
                //previewWindow.Show();
            }
            // Normal screensaver mode.  Either screen saver kicked in normally,
            // or was launched from Preview button
            else if (e.Args[0].ToLower().StartsWith("/s"))
            {
                Logger.LogDebug("Mode screen saver");
                ScreenSaverWindow screensaver = new ScreenSaverWindow();
                screensaver.Show();
            }
            // Config mode, launched from Settings button in screen saver dialog
            else if (e.Args[0].ToLower().StartsWith("/c"))
            {
                Logger.LogDebug("Mode configuration");
                ConfigurationWindow configurationWindow = new ConfigurationWindow();
                configurationWindow.Show();
            }
            // If not running in one of the sanctioned modes, shut down the app
            // immediately (because we don't have a GUI).
            else
            {
                Logger.LogError("Aucun argument spécifié");
                ScreenSaverWindow screensaver = new ScreenSaverWindow();
                screensaver.Show();
            }
        }
     }
}

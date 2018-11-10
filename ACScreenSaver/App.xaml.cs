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
            // Preview mode--display in little window in Screen Saver dialog
            // (Not invoked with Preview button, which runs Screen Saver in
            // normal /s mode).
            if (e.Args[0].ToLower().StartsWith("/p"))
            {
                //PreviewWindow previewWindow = new PreviewWindow();
                //previewWindow.Show();
            }
            // Normal screensaver mode.  Either screen saver kicked in normally,
            // or was launched from Preview button
            else if (e.Args[0].ToLower().StartsWith("/s"))
            {
                ScreenSaverWindow screensaver = new ScreenSaverWindow();
                screensaver.Show();
            }
            // Config mode, launched from Settings button in screen saver dialog
            else if (e.Args[0].ToLower().StartsWith("/c"))
            {
                ConfigurationWindow configurationWindow = new ConfigurationWindow();
                configurationWindow.Show();
            }
            // If not running in one of the sanctioned modes, shut down the app
            // immediately (because we don't have a GUI).
            else
            {
                ScreenSaverWindow screensaver = new ScreenSaverWindow();
                screensaver.Show();
            }
        }
     }
}

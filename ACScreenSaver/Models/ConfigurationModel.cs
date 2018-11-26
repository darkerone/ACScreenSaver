using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ACScreenSaver
{
    public class ConfigurationModel
    {
        private string _configurationFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ACScreenSaver\ACSS_Configuration.acss";

        public ConfigurationModel()
        {
            ImagesDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); ;
            ImageDisplayDuration = 5000;
            IsRandom = true;
            TimerDurationGap = 2000;
            TimerDisplayDuration = 2000;
            NumberOfSuccessiveSameFolderFiles = 1;
            PanoramaDisplayDuration = ImageDisplayDuration * 3;
            IsYearDisplayed = true;
            YearColor = new Color() {
                R = 255,
                G = 185,
                B = 0,
                A = 178,
            };
            DisplayInformationDuration = 3000;
        }

        /// <summary>
        /// Restaure la configuration à partir du fichier de configuration
        /// </summary>
        public void RestoreConfiguration()
        {
            if (File.Exists(_configurationFilePath))
            {
                Logger.LogDebug("Restauration de la configuration");
                string json = System.IO.File.ReadAllText(_configurationFilePath);
                var jsonSerializerSettings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                ConfigurationModel configurationModelTmp = JsonConvert.DeserializeObject<ConfigurationModel>(json, jsonSerializerSettings);

                ImagesDirectoryPath = configurationModelTmp.ImagesDirectoryPath;
                ImageDisplayDuration = configurationModelTmp.ImageDisplayDuration;
                IsRandom = configurationModelTmp.IsRandom;
                TimerDurationGap = configurationModelTmp.TimerDurationGap;
                TimerDisplayDuration = configurationModelTmp.TimerDisplayDuration;
                NumberOfSuccessiveSameFolderFiles = configurationModelTmp.NumberOfSuccessiveSameFolderFiles;
                PanoramaDisplayDuration = configurationModelTmp.PanoramaDisplayDuration;
                IsYearDisplayed = configurationModelTmp.IsYearDisplayed;
                YearColor = configurationModelTmp.YearColor;
                DisplayInformationDuration = configurationModelTmp.DisplayInformationDuration;
            }
            else
            {
                Logger.LogDebug("Aucun fichier de configuration trouvé");
            }
        }

        public void SaveConfiguration()
        {
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            string jsonConfiguration = JsonConvert.SerializeObject(this, jsonSerializerSettings);
            try
            {
                System.IO.File.WriteAllText(_configurationFilePath, jsonConfiguration);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }

        /// <summary>
        /// Définit le répertoire contenant les images à afficher
        /// </summary>
        public string ImagesDirectoryPath { get; set; }

        /// <summary>
        /// Temps d'affichage d'une image (en millisecondes)
        /// </summary>
        public int ImageDisplayDuration { get; set; }

        /// <summary>
        /// Temps d'affichage d'une panorama (en millisecondes)
        /// </summary>
        public int PanoramaDisplayDuration { get; set; }

        /// <summary>
        /// Définit si les photos seront affichées de manière aléatoire
        /// </summary>
        public bool IsRandom { get; set; }

        /// <summary>
        /// Définit de combien le timer sera augementé/diminué
        /// </summary>
        public int TimerDurationGap { get; set; }

        /// <summary>
        /// Définit le temps que le timer restera affiché
        /// </summary>
        public int TimerDisplayDuration { get; set; }

        /// <summary>
        /// Définit le nombre de fichiers successifs du même dossier qui seront affichés en aléatoire
        /// </summary>
        public int NumberOfSuccessiveSameFolderFiles { get; set; }

        /// <summary>
        /// Définit si l'année de la photo est affichée sur l'image ou non
        /// </summary>
        public bool IsYearDisplayed { get; set; }

        /// <summary>
        /// Définit la couleur du texte de l'année
        /// </summary>
        public Color YearColor { get; set; }

        /// <summary>
        /// Temps d'affichage d'des informations (en millisecondes)
        /// </summary>
        public int DisplayInformationDuration { get; set; }
    }
}

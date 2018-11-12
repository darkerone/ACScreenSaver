using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACScreenSaver
{
    public class ConfigurationModel
    {
        private string _configurationFilePath = @"ACSS_Configuration.acss";

        public ConfigurationModel()
        {
            ImagesDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); ;
            ImageDisplayDuration = 10;
            IsRandom = true;
            TimerDurationGap = 2;
            TimerDisplayDuration = 2;
            NumberOfSuccessiveSameFolderFiles = 1;
        }

        /// <summary>
        /// Restaure la configuration à partir du fichier de configuration
        /// </summary>
        public void RestoreConfiguration()
        {
            if (File.Exists(_configurationFilePath))
            {
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
            System.IO.File.WriteAllText(_configurationFilePath, jsonConfiguration);
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
    }
}

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
            IntervalTime = 10;
            IsRandom = true;
            IntervalTimeGap = 2;
            DisplayIntervalTimeTime = 2;
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
                IntervalTime = configurationModelTmp.IntervalTime;
                IsRandom = configurationModelTmp.IsRandom;
                IntervalTimeGap = configurationModelTmp.IntervalTimeGap;
                DisplayIntervalTimeTime = configurationModelTmp.DisplayIntervalTimeTime;
            }
        }

        /// <summary>
        /// Définit le répertoire contenant les images à afficher
        /// </summary>
        public string ImagesDirectoryPath { get; set; }

        /// <summary>
        /// Temps d'affichage d'une image (en millisecondes)
        /// </summary>
        public int IntervalTime { get; set; }

        /// <summary>
        /// Définit si les photos seront affichées de manière aléatoire
        /// </summary>
        public bool IsRandom { get; set; }

        /// <summary>
        /// Définit de combien le timer sera augementé/diminué
        /// </summary>
        public int IntervalTimeGap { get; set; }

        /// <summary>
        /// Définit le temps que le timer restera affiché
        /// </summary>
        public int DisplayIntervalTimeTime { get; set; }
    }
}

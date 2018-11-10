using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACScreenSaver
{
    public class ScreenSaverManager
    {
        private string _deletedImagesFilePath = @"ACSS_Deleted.acss";
        private string _notDisplayedImagesFilePath = @"ACSS_NotDisplayed.acss";
        private string _historicFilePath = @"ACSS_Historic.acss";

        public ConfigurationModel ConfigurationModel { get; set; }

        private List<string> _imagesFilePathList = new List<string>();
        private int _currentImageIndex = 0;

        private List<string> _notDisplayedImagesPathList = new List<string>();

        public ScreenSaverManager()
        {
            ConfigurationModel = new ConfigurationModel();
            ConfigurationModel.RestoreConfiguration();
            RestoreNotDisplayedImageList();
            InitImagesList();
        }

        /// <summary>
        /// Restaure la liste des images qui ne seront pas affichées
        /// </summary>
        private void RestoreNotDisplayedImageList()
        {
            _notDisplayedImagesPathList.Clear();
            if (File.Exists(_notDisplayedImagesFilePath))
            {
                string[] lines = System.IO.File.ReadAllLines(_notDisplayedImagesFilePath);
                foreach (string line in lines)
                {
                    // Ex : DD/MM/AAAA hh:mm:ss - C:\Path\To\Image.jpg
                    // On retire la date
                    string[] splittedLine = line.Split('-');
                    splittedLine = splittedLine.Skip(1).ToArray();

                    // On reconstitue le chemin de l'image
                    string imagePath = string.Join("-", splittedLine);

                    _notDisplayedImagesPathList.Add(imagePath);
                }
            }
        }

        /// <summary>
        /// Renvoie l'index de l'image courante
        /// </summary>
        /// <returns></returns>
        public int GetCurrentImageIndex()
        {
            return _currentImageIndex;
        }

        /// <summary>
        /// Renvoie le chemin de l'image courante
        /// </summary>
        /// <returns></returns>
        public string GetCurrentImagePath()
        {
            if (_imagesFilePathList.Any())
            {
                return _imagesFilePathList[_currentImageIndex];
            }
            return null;
        }

        /// <summary>
        /// Renvoie l'index de la dernière image de la liste
        /// </summary>
        /// <returns></returns>
        public int GetLastImageIndex()
        {
            return _imagesFilePathList.Count - 1;
        }

        /// <summary>
        /// Renvoie le chemin de l'image dont l'index est passé en paramètre
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetImagePathOfIndex(int index)
        {
            return _imagesFilePathList[index];
        }

        /// <summary>
        /// Définit l'index de la nouvelle image courante
        /// </summary>
        /// <param name="index"></param>
        public void SetCurrentImageIndex(int index)
        {
            _currentImageIndex = index;
            AddCurrentImageToHistoric();
        }

        /// <summary>
        /// Initialise la liste des images à afficher
        /// </summary>
        public void InitImagesList()
        {
            DirectoryInfo d = new DirectoryInfo(ConfigurationModel.ImagesDirectoryPath);
            string[] files = System.IO.Directory.GetFiles(ConfigurationModel.ImagesDirectoryPath, "*.jpg", SearchOption.TopDirectoryOnly);
            _imagesFilePathList.Clear();
            _imagesFilePathList.AddRange(files);
        }

        /// <summary>
        /// Ajoute le chemin de l'image courante au fichier des images à supprimer
        /// </summary>
        public void AddCurrentImageToDeleted()
        {
            List<string> filesPathToAdd = new List<string>();
            filesPathToAdd.Add(GetCurrentTimeString() + " - " + GetCurrentImagePath());
            System.IO.File.AppendAllLines(_deletedImagesFilePath, filesPathToAdd);
        }

        /// <summary>
        /// Ajoute le chemin de l'image courante au fichier des images à passer lors de la lecture
        /// </summary>
        public void AddCurrentImageToNotDisplayed()
        {
            List<string> filesPathToAdd = new List<string>();
            filesPathToAdd.Add(GetCurrentTimeString() + " - " + GetCurrentImagePath());
            System.IO.File.AppendAllLines(_notDisplayedImagesFilePath, filesPathToAdd);
            RestoreNotDisplayedImageList();
        }

        /// <summary>
        /// Renvoie l'heure et la date actuelle au format string
        /// </summary>
        /// <returns></returns>
        private string GetCurrentTimeString()
        {
            return DateTime.Now.ToString();
        }

        public bool CanDisplayImageOfIndex(int index)
        {
            bool isInList = false;
            string imagePath = GetImagePathOfIndex(index);
            foreach (string notDisplayedImagePath in _notDisplayedImagesPathList)
            {
                if (notDisplayedImagePath.Contains(imagePath))
                {
                    isInList = true;
                }
            }
            return !isInList;
        }

        /// <summary>
        /// Ajoute l'image courante à l'historique des images qui ont été affichées
        /// </summary>
        public void AddCurrentImageToHistoric()
        {
            List<string> filesPathToAdd = new List<string>();
            filesPathToAdd.Add(GetCurrentTimeString() + " - " + GetCurrentImagePath());
            System.IO.File.AppendAllLines(_historicFilePath, filesPathToAdd);
        }
    }
}
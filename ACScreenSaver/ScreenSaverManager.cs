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
        private string _deletedImagesFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ACScreenSaver\ACSS_Deleted.acss";
        private string _notDisplayedImagesFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ACScreenSaver\ACSS_NotDisplayed.acss";
        private string _historicFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ACScreenSaver\ACSS_Historic.acss";

        private FileManager _fileManager;

        public ConfigurationModel Configuration { get; set; }

        /// <summary>
        /// Liste des images à afficher
        /// </summary>
        private string[] _imagesFilePathList = new string[] { };

        /// <summary>
        /// Index de l'image courante
        /// </summary>
        private int _currentImageIndex = 0;

        /// <summary>
        /// Liste des images à ne pas afficher
        /// </summary>
        private List<string> _notDisplayedImagesPathList = new List<string>();

        public ScreenSaverManager()
        {
            Configuration = new ConfigurationModel();
            Configuration.RestoreConfiguration();
            RestoreNotDisplayedImageList();

            _fileManager = new FileManager();

            InitImagesList();
        }

        #region Public methods

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
            return _imagesFilePathList.Length - 1;
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
        /// Ajoute le chemin de l'image courante au fichier des images à supprimer
        /// </summary>
        public void AddCurrentImageToDeleted()
        {
            try
            {
                List<string> filesPathToAdd = new List<string>();
                filesPathToAdd.Add(GetCurrentTimeString() + " - " + GetCurrentImagePath());
                System.IO.File.AppendAllLines(_deletedImagesFilePath, filesPathToAdd);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }

        /// <summary>
        /// Ajoute le chemin de l'image courante au fichier des images à passer lors de la lecture
        /// </summary>
        public void AddCurrentImageToNotDisplayed()
        {
            try
            {
                List<string> filesPathToAdd = new List<string>();
                filesPathToAdd.Add(GetCurrentTimeString() + " - " + GetCurrentImagePath());
                System.IO.File.AppendAllLines(_notDisplayedImagesFilePath, filesPathToAdd);
                RestoreNotDisplayedImageList();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }

        /// <summary>
        /// Définit si l'image a le droit d'être affichée
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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
            try
            {
                List<string> filesPathToAdd = new List<string>();
                filesPathToAdd.Add(GetCurrentTimeString() + " - " + GetCurrentImagePath());
                System.IO.File.AppendAllLines(_historicFilePath, filesPathToAdd);
            }
            catch(Exception e)
            {
                Logger.LogError(e.Message);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Restaure la liste des images qui ne seront pas affichées
        /// </summary>
        private void RestoreNotDisplayedImageList()
        {
            _notDisplayedImagesPathList.Clear();
            if (File.Exists(_notDisplayedImagesFilePath))
            {
                Logger.LogDebug("Restauration des chemins des fichiers à ne pas afficher");
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
            else
            {
                Logger.LogDebug("Aucun fichier trouvé pour la restauration des chemins des fichiers à ne pas afficher");
            }
        }

        /// <summary>
        /// Renvoie l'heure et la date actuelle au format string
        /// </summary>
        /// <returns></returns>
        private string GetCurrentTimeString()
        {
            return DateTime.Now.ToString();
        }

        /// <summary>
        /// Initialise la liste des images à afficher
        /// </summary>
        private void InitImagesList()
        {
            if (Configuration.IsRandom)
            {
                Logger.LogDebug("Mode de visualisation aléatoire");
                _imagesFilePathList = _fileManager.GenerateRandomSameFolderFilePathList(Configuration.ImagesDirectoryPath, Configuration.NumberOfSuccessiveSameFolderFiles);
            }
            else
            {
                Logger.LogDebug("Mode de visualisation non aléatoire");
                Logger.LogDebug("Récupération de la liste des fichiers");
                _imagesFilePathList = Directory.GetFiles(Configuration.ImagesDirectoryPath, "*.jpg", SearchOption.AllDirectories);
            }
        }

        #endregion
    }
}
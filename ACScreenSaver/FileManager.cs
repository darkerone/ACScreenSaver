using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACScreenSaver
{
    public class FileManager
    {
        /// <summary>
        /// Génère une liste aléatoire de fichiers par groupe de fichiers successifs du même dossier
        /// </summary>
        /// <param name="directoryPath">Chemin du dossier</param>
        /// <param name="numberOfSuccessiveSameFolderFiles">Nombre de fichieres appartenant au même dossier</param>
        /// <returns></returns>
        public string[] GenerateRandomSuccessiveFilePathList(string directoryPath, int numberOfSuccessiveSameFolderFiles)
        {
            if(numberOfSuccessiveSameFolderFiles < 1)
            {
                numberOfSuccessiveSameFolderFiles = 1;
            }

            string[] files = Directory.GetFiles(directoryPath, "*.jpg", SearchOption.AllDirectories);
            List<string> foundFilesList = files.ToList();
            
            int seed = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Random rnd = new Random(seed);
            //files = files.OrderBy(x => rnd.Next()).ToArray();

            List<string> randomFilesList = new List<string>();
            // Tant que la liste des fichiers récupérés n'est pas vide
            while(foundFilesList.Count > 0)
            {
                // On tire l'index d'un fichier au hasard dans la liste
                int randomIndex = rnd.Next(foundFilesList.Count);
                // Récupère l'index du dernier fichier du même répertoire que le fichier tiré dans la limite de n fichiers
                int lastNextFilesIndexes = GetLastNextFilesIndexesOfSameFolder(foundFilesList, randomIndex, numberOfSuccessiveSameFolderFiles - 1);
                // Déplace les fichiers d'une liste à l'autre
                randomFilesList.AddRange(foundFilesList.GetRange(randomIndex, lastNextFilesIndexes - randomIndex + 1));
                foundFilesList.RemoveRange(randomIndex, lastNextFilesIndexes - randomIndex + 1);
            }

            return randomFilesList.ToArray();
        }

        /// <summary>
        /// Génère une liste aléatoire de fichiers par groupe de fichiers aléatoires du même dossier
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="numberOfSameFolderFiles"></param>
        /// <returns></returns>
        public string[] GenerateRandomSameFolderFilePathList(string directoryPath, int numberOfSameFolderFiles)
        {
            if (numberOfSameFolderFiles < 1)
            {
                numberOfSameFolderFiles = 1;
            }

            string[] files = Directory.GetFiles(directoryPath, "*.jpg", SearchOption.AllDirectories);

            List<FileModel> filesModelList = files.Select(x => new FileModel() {
                Path = x,
                ParentFolderPath = Path.GetDirectoryName(x)
            }).ToList();

            int seed = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Random rnd = new Random(seed);

            List<string> randomFileList = new List<string>();
            while (filesModelList.Count > 0)
            {
                // On tire l'index d'un fichier au hasard dans la liste
                int randomFileIndex = rnd.Next(filesModelList.Count);
                // On récupère l'index du premier fichier du répertoire du fichier tiré au hasard
                int firstSameFolderFileIndex = FindFirstFileIndexOfSameDirectory(filesModelList, randomFileIndex);
                // On récupère l'index du dernier fichier du répertoire du fichier tiré au hasard
                int lastSameFolderFileIndex = FindLastFileIndexOfSameDirectory(filesModelList, randomFileIndex);

                // Pour autant de fichier que l'on veut sélectionner dans le fichier
                for (int i = 0; i < numberOfSameFolderFiles; i++)
                {
                    if(firstSameFolderFileIndex <= lastSameFolderFileIndex)
                    {
                        // On tire un fichier au hasard dans le répertoire du fichier tiré au hasard
                        int randomSameFolder = rnd.Next(firstSameFolderFileIndex, lastSameFolderFileIndex + 1);
                        // On ajoute le fichier dans la liste aléatoire
                        randomFileList.Add(filesModelList[randomSameFolder].Path);
                        // On retire le fichier de la liste des fichiers
                        filesModelList.Remove(filesModelList[randomSameFolder]);
                        // On diminue l'index de fin car on a retiré le fichier
                        lastSameFolderFileIndex--;
                    }
                }
            }

            return randomFileList.ToArray();
        }

        /// <summary>
        /// Renvoie l'index du premier fichier du répertoire du fichier dont l'index est passé en paramètre
        /// </summary>
        /// <param name="filesList"></param>
        /// <param name="filePathIndex"></param>
        /// <returns></returns>
        private int FindFirstFileIndexOfSameDirectory(List<FileModel> filesList, int filePathIndex)
        {
            bool isInSameFolder = true;
            int firstIndex = filePathIndex - 1;
            // Tant que le fichier appartient au même répertoire
            while (isInSameFolder)
            {
                if(firstIndex < 0 ||
                   filesList[firstIndex].ParentFolderPath != filesList[filePathIndex].ParentFolderPath)
                {
                    isInSameFolder = false;
                }
                else
                {
                    firstIndex--;
                }
            }
            return firstIndex + 1;
        }

        /// <summary>
        /// Renvoie l'index du dernier fichier du répertoire du fichier dont l'index est passé en paramètre
        /// </summary>
        /// <param name="filesList"></param>
        /// <param name="filePathIndex"></param>
        /// <returns></returns>
        private int FindLastFileIndexOfSameDirectory(List<FileModel> filesList, int filePathIndex)
        {
            bool isInSameFolder = true;
            int lastIndex = filePathIndex + 1;
            // Tant que le fichier appartient au même répertoire
            while (isInSameFolder)
            {
                if (lastIndex >= filesList.Count ||
                   filesList[lastIndex].ParentFolderPath != filesList[filePathIndex].ParentFolderPath)
                {
                    isInSameFolder = false;
                }
                else
                {
                    lastIndex++;
                }
            }
            return lastIndex - 1;
        }

        /// <summary>
        /// Renvoie l'index du dernier des fichiers suivant qui appartiennent au même dossier dans la limite de numberOfFiles fichiers
        /// </summary>
        /// <param name="filesList"></param>
        /// <param name="filePathIndex"></param>
        /// <param name="numberOfFiles"></param>
        /// <returns></returns>
        public int GetLastNextFilesIndexesOfSameFolder(List<string> filesList, int filePathIndex, int numberOfFiles)
        {
            int lastNextFilesIndexes = filePathIndex;

            // Dossier parent du fichier
            string directoryPath = Directory.GetParent(filesList[filePathIndex]).FullName;

            // Dernier index théorique (si tous les fichiers suivant appartiennent au même répertoire)
            int theoricLastNextFileIndex = filePathIndex + numberOfFiles;
            // On vérifié que l'index théorique ne dépasse pas la taille de la liste
            if(theoricLastNextFileIndex >= filesList.Count)
            {
                // Si c'est le cas, on limite à la fin de la liste
                theoricLastNextFileIndex = filesList.Count - 1;
            }

            // Pour les n fichiers suivant
            for (int i = filePathIndex; i <= theoricLastNextFileIndex; i++)
            {
                string directoryPathTmp = Directory.GetParent(filesList[i]).FullName;
                // Si le dossier parent est le même
                if (directoryPath == directoryPathTmp)
                {
                    lastNextFilesIndexes = i;
                }
            }

            return lastNextFilesIndexes;
        }
    }
}

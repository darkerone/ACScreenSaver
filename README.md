# ACScreenSaver
Ecran de veille pour faire défiler des photos et afficher des informations sur les photos

# Problématique
L'écran de veille Windows d'origine ne permet pas de savoir où se trouve la photo sur le disque dur ou de savoir à quel évennement/année elle correspond.

## Presentation
ACScreenSaver est un écran de veille permettant de faire passer des photos.
Une multitude de fonctionnalités existent en plus de celles proposées par l'écran de veille photo classique de Windows.
- Fenêtre de configuration intégrée au configurateur d'écran de veille Windows
- Mode aléatoire ou non
- Mode aléatoire avec le choix du nombre de photos consécutives prises dans le même dossier
- Durée d'affichage de chaque image
- Possibilité d'augementer le temps d'affichage de la photo courante avec les flèches haut et bas
- Aller à la photo suivante/précédente grâce aux flèches droite et gauche
- Faire défiler les panoramas 
- Possibilité de zoomer/dézoomer (le bouton droit réinitialise)
- Les photos vertical sont affichée verticalement
- Possibilité d'afficher le nom du premier dossier dans lequel se trouve la photo. 
	Ex : Si "D:\User\Images" contient les dossiers "2017" et "2018", on prend le nom du dossier auquel appartient l'image : "2017" ou "2018"
- Affichage des informations de la photo lorsque l'on bouge la souris
- L'appuie sur une touche permet d'afficher une fenètre d'informations sur la photo
- Possibilité de marque une photo comme "A supprimer"
- Possibilité de marquer une photo comme "Ne plus afficher"
- Enregistrement de l'historique des photos qui ont été affichées
- Play/Pause lors de l'appuie sur la barre d'espace

## Installation
Placer les fichiers suivant dans C:\Windows :
- ACScreenSaver.scr
- Newtonsoft.Json.dll
- Xceed.Wpf.Toolkit.dll

Ces fichiers sont générés en mode Release.

## Notes
Les fichiers créés par l'application se trouvent dans "Mes Documents\ACScreenSaver".

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using TMPro;
//tp4
/// <summary>
/// Permet d'ajouter un joueur avec ses données dans le fichier tim afin d'écrire et de lire les infos du joueur comme son score et son nom
/// </summary>
[CreateAssetMenu(menuName = "TIM/Sauvegarde", fileName = "Sauvegarde")]
public class SOSauvegarde : ScriptableObject
{
    [DllImport("__Internal")]
    static extern void SynchroniserWebGL();
    [Header("Données du joueur")]
    [SerializeField] string _nomJoueur = "defaut"; //nom joueur par defaut
    [SerializeField] SOPerso _donneesPerso; //va chercher les données du joueur pendant jeu

    [Header("Progression")]
    [SerializeField] int _niveauxComplet; //rendu quel niveau
    [SerializeField] int _joyauxAccumule; //nb joyaux accumulés
    [SerializeField] int _tempsRestant; //reste cmb temps compteur

    [Header("Scores")]
    [SerializeField] List<NomScore> _lesNomsScores = new List<NomScore>{
        new NomScore{nom = "Bob", score = 10},
        new NomScore{nom = "Alice", score = 20}
    }; //liste des joueurs dans top 3 ayant chacun un nom et un score
    [SerializeField] int calculTotal; //calcul du score

    public string _niveauxText; //pour passer les textes au textmeshpro dans le script niveauFin //texte du niveau
    public string _tempsText; //texte temps
    public string _joyauxText; //texte joyaux
    public string _totalText; //texte total
    public string _nomsScoresText; //texte des joueurs dans la liste du high score
    public bool _joueurPodium = false; //es-tu dans le top 3
    string _fichier = "Score.tim"; //nom du fichier tim

    /// <summary>
    /// Initialise nom joueur et l'enlève si il n'a pas changé son nom
    /// </summary>
    public void Initialiser() 
    {
        _lesNomsScores.RemoveAll(NomScore => NomScore.nom == "defaut"); //enlève de la liste tous les noms qui ont defaut(C'est pour si un joueur entre pas son nom dans high score et quitte)
        _nomJoueur = "defaut"; //réassigne nom par défaut
        _joueurPodium = false; //il n'est plus sur podium si le prochain joueur aurait cette valeur
    }
    
    /// <summary>
    /// Est appelé quand on rentre un nom dans le input field
    /// </summary>
    /// <param name="champTexte">Le nom du joueur passé par le input field</param>
    public void AjouterNomScore(TextMeshProUGUI champTexte) 
    {

        
        Debug.Log(_nomJoueur);
        for(int i = 0; i <  _lesNomsScores.Count; i++) //Chaque joueurs dans la liste
        {
           if(_lesNomsScores[i].nom == _nomJoueur) //si le nom est defaut et qu'il est dans la liste du top 3 et qu'il change nom
           {
                _nomJoueur = champTexte.text; //nom joueur defaut devient celui du inputfield
                _lesNomsScores[i].nom = _nomJoueur; //assigne à la variable
                break;
           }
        }
        Debug.Log(_nomJoueur);
        UpdateNomsScoresText(); //appel pour update le nom à l'écran
        
    }
    /// <summary>
    /// Ajout d'un joueur et trie pour savoir si score est assez haut pour être dans liste.
    /// </summary>
    public void AjouterJoueurAListe()
    {
        _lesNomsScores.Add(new NomScore { nom = _nomJoueur, score = calculTotal}); //ajoute le joueur à liste
        _lesNomsScores.Sort((a,b) => b.score.CompareTo(a.score)); //si score est assez grand il est dans liste

         if (_lesNomsScores.Count > 3) //enlève les plus petits et garde 3
         {
             _lesNomsScores.RemoveRange(3, _lesNomsScores.Count - 3);
         }
    }
    
    /// <summary>
    /// Update le nom à l'écran
    /// </summary>
     public void UpdateNomsScoresText() //est appelé seulement par AjouterNomScore
    {
        _joueurPodium = false;
        Debug.Log(_nomJoueur);
        _nomsScoresText = "";
        for(int i = 0; i <  _lesNomsScores.Count; i++) //pour chaque joueurs sur le podium
        {
            _nomsScoresText += (i +1 )+ ". " + (_lesNomsScores[i].nom + " : " + _lesNomsScores[i].score + "\n"); //met les noms sur le podium 1 tim 2 etc 3 etc

            if (_lesNomsScores[i].nom == _nomJoueur) //si son nom est dans la liste y est sur le podium, donc va pouvoir entrer son nom
            {
                _joueurPodium = true;
            }
        }
        Debug.Log(_nomsScoresText);
    }

    [ContextMenu("Lire fichier")]
    
    /// <summary>
    /// La méthode "LireFichier" est appelée lorsque le joueur sélectionne un niveau. 
    /// Elle lit le contenu du fichier de données situé dans le répertoire de stockage persistant de l'application et met à jour les variables de l'objet de 
    /// scriptable correspondant en utilisant la méthode 
    /// "FromJsonOverwrite" de la classe JsonUtility.
    /// </summary>
    
    public void LireFichier()// appelé dans le boutton niveau  et transfert les infos du fichier aux infos du scriptable object
    {
        Debug.Log("lire");
        string fichierEtChemin = Application.persistentDataPath + "/" + _fichier; //destination fichier
        Debug.Log(fichierEtChemin);
        if(File.Exists(fichierEtChemin)) //si le fichier y existe
        {
            string contenu = File.ReadAllText(fichierEtChemin); //lire text 
            Debug.Log(contenu);
            JsonUtility.FromJsonOverwrite(contenu, this); //transforme Json en contenu pour scriptable object
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            #endif
        }
    }
    
    /// <summary>
    //La méthode "EcrireFichier" est appelée lorsque le joueur termine un niveau et soumet son score. 
    // Elle trie les scores et met à jour les variables correspondantes de l'objet de scriptable. 
    // Ensuite, elle convertit cet objet en une chaîne JSON en utilisant la méthode "ToJson" de la classe JsonUtility 
    // et écrit cette chaîne dans le fichier de données à l'aide de la méthode "WriteAllText" de la classe File.
    /// </summary>
    public void EcrireFichier() //lorsqu'on envoit le InputField, y est appelé. Écrit dans le fichier
    {
        Debug.Log("ecrire");
        _lesNomsScores.Sort((a,b) => b.score.CompareTo(a.score)); //fait le trie pour que les meilleurs scores soient dans la liste
        _niveauxComplet = _donneesPerso.niveau; //assigne les données perso au scriptable object pour les écrire dans fichier.
        _joyauxAccumule = _donneesPerso.argent;
        _tempsRestant = (int)_donneesPerso.compteur;
        string fichierEtChemin = Application.persistentDataPath + "/" + _fichier; //destination fichier
        Debug.Log(fichierEtChemin);
        //File.WriteAllText(fichierEtChemin, "Bonjour");
        string contenu = JsonUtility.ToJson(this, true); //passe le scriptable object en format json
        Debug.Log(contenu);
        File.WriteAllText(fichierEtChemin, contenu); //ecrit le scriptable object dans un fichier texte
        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("WebGL");
            SynchroniserWebGL();
        }
        
    }
    /// <summary>
    //Lorsqu'on entre dans la scène de fin on l'appel de Niveaufin
    // calcul des 3 donnees du joueur
    /// </summary>
    public void AfficherScore()
    {
        int calculNiveaux = _niveauxComplet * 100; //les niveaux complétés * nombre
        int calculJoyaux = _joyauxAccumule * 10; //les joyaux accumulés * nombre
        int calculTemps = _tempsRestant * 5; //temps restant * nombre
        calculTotal = calculTemps + calculJoyaux + calculNiveaux; //le total pour savoir son score
        _niveauxText = "Niveaux Complétés: " + _niveauxComplet.ToString() + " niveau(x) * 100 = " + calculNiveaux; //crée le texte qui va être passé à textmeshpro dans NiveauFin
        _joyauxText = "Joyaux accumulés: " + _joyauxAccumule.ToString() + " joyaux * 10 = " + calculJoyaux;
        _tempsText = "Temps restant: " + _tempsRestant.ToString() + "s * 5 = " + calculTemps;
        _totalText = "Total: " + calculTotal + "points"; 

        AjouterJoueurAListe(); //ajoute joueur defaut quand on entre dans la scène #synthese
        UpdateNomsScoresText(); //mettre nom à l'écran de défaut si est dans la liste
    }
    //Crée type pour les mettre dans la liste
    [System.Serializable]
    public class NomScore
    {
        public string nom;
        public int score;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//tp4
/// <summary>
/// Permet de montrer les données à l'usager à la fin du niveau et si il est sur le podium, il peut ajouter son nom au tableau des scores
/// </summary>
public class NiveauFin : MonoBehaviour
{
    [Header("Données")]
    [SerializeField] SOPerso _donneesPerso; //accéder données perso
    [SerializeField] SOSauvegarde _sauvegarde; //accéder données sauvegarde

    [Header("UI")]
    [SerializeField] TextMeshProUGUI _niveauxText; //les textes sur niveau
    [SerializeField] TextMeshProUGUI _tempsText;
    [SerializeField] TextMeshProUGUI _joyauxText;
    [SerializeField] TextMeshProUGUI _totalText;
    [SerializeField] public TMP_InputField _nomInputField; //le input field pour entrer son nom si dans liste du top 3
    [SerializeField] TextMeshProUGUI _nomsScoresText; //le texte des 3 joueurs top 3

    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Je suis fin");
         _sauvegarde.AfficherScore(); //appel pour afficher le score
        if(_sauvegarde._joueurPodium == true) //si le score du joueur est assez bon, le input field est présent
        {
            _nomInputField.gameObject.SetActive(true);
        }
        else
        {
            _nomInputField.gameObject.SetActive(false);
        }
        _niveauxText.text = _sauvegarde._niveauxText; //assigne les textes des données sauvegarde au textmeshpro
        _tempsText.text = _sauvegarde._tempsText;
        _joyauxText.text = _sauvegarde._joyauxText;
        _totalText.text = _sauvegarde._totalText;
        _nomsScoresText.text = _sauvegarde._nomsScoresText;
    }
    /// <summary>
    /// lorsqu'on quitte met les valeurs par défauts
    /// </summary>
    void OnApplicationQuit() 
    {
        _donneesPerso.Initialiser();
        _sauvegarde.Initialiser();
    }
    /// <summary>
    /// Lorsqu'on clique sur bouton pour aller au menu principale, initialise les données
    /// </summary>
    public void AllerMenu()
    {
        _donneesPerso.Initialiser(); //initialise les donnees du joueur
        _sauvegarde.Initialiser(); //initialise données de la sauvegarde
    }

    // Update is called once per frame
    void Update()
    {
        _nomsScoresText.text = _sauvegarde._nomsScoresText; //assigne aussi au textmesh pro, mais continuellement car il change je comprenais pas comment faire fonction ici qui se fait appeler dans scriptable object
    }
    /// <summary>
    /// Arret de musique lorsqu'on clique sur le bouton
    /// </summary>
    public void ArretMusique()
    {
        GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueEvenA, false); //arrete la musique tp4
        GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueEvenB, false); //arretela musique tp4
    }
}

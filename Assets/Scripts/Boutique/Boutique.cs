using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
/// <summary>
/// Le script de la boutique qui sert à acheter des objets
/// </summary>
public class Boutique : MonoBehaviour
{
    [Header("Données personnelles")]
    [SerializeField] SOPerso _donneesPerso; //accède aux données perso
    public SOPerso donnesPerso => _donneesPerso; //get 

    [Header("Éléments d'interface utilisateur")]
    [SerializeField] TextMeshProUGUI _champArgent; //montre l'argent accumulé à l'utilisateur
    [SerializeField] TextMeshProUGUI _champNiveau; //tp4 champ niveau

    [Header("Panneau d'inventaire")]
    [SerializeField] PanneauInventaire _panneauInventaire; //tp4 panneau ui inventaire

    [Header("Singleton")]
    static Boutique _instance; //singleton de boutique
    static public Boutique instance => _instance;

    public PanneauInventaire panneauInventaire => _panneauInventaire;
    bool _estEnPlay = true;
    void Awake()
    {
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this; //pas avoir 2 instances
        donnesPerso.EffacerInventaire(); //efface l'inventaire au début
        MettreAJOURInfos();//montre l'argent lorsqu'on a entré dans boutique et l'update
        _donneesPerso.evenementMiseAJour.AddListener(MettreAJOURInfos); //Abonne MettreAJourInfos
        
    }
    /// <summary>
    /// Metre à jour les infos de la boutique
    /// </summary>
    private void MettreAJOURInfos() //montre l'argent lorsqu'on a entré dans boutique et l'update
    {
        _champArgent.text = _donneesPerso.argent + " $";
        _champNiveau.text = "Niveau " +_donneesPerso.niveau; //tp4 augmente le niveau
        
    }
    /// <summary>
    /// Lorsqu'on quitte le jeu initialise les données du perso
    /// </summary>
    void OnApplicationQuit() //si on quitte initialise donnees du perso
    {
        _donneesPerso.Initialiser();
        _estEnPlay = false;
    }

    void OnDestroy() //si y est plus là enlève les listeners
    {
        _donneesPerso.evenementMiseAJour.RemoveAllListeners();
        if(_estEnPlay) _donneesPerso.niveau++; //qui boutique augmente niveau tp4
    }
}
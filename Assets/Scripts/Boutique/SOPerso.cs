using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "Perso", menuName = "Perso")] //scriptable object
/// <summary>
/// Les données du perso et le moyen de savoir si il a acheté un objet dans la boutique tp4
/// </summary>
public class SOPerso : ScriptableObject
{
    [Header("Niveau")]
    [SerializeField][Range(1, 5)] int _niveauIni = 1;
    [SerializeField][Range(1, 5)] int _niveau = 1;

    [Header("Argent")]
    [SerializeField][Range(0, 500)] int _argentIni = 100; //argent du joueur début = 100
    [SerializeField][Range(0, 500)] int _argent = 100;

    [Header("Compteur")]
    [SerializeField][Range(0, 500)] float _compteurIni = 500f;
    [SerializeField][Range(0, 500)] float _compteur = 500f;

    [Header("Vies")]
    [SerializeField] int _nbVies = 3;
    [SerializeField] int _nbViesIni = 3;


    public int nbVies
    {
        get => _nbVies;
        set
        {
            _nbVies = Mathf.Clamp(value, 0, int.MaxValue);
            evenementMiseAJour.Invoke();
        }
    }
    public int niveau
    {
        get => _niveau;
        set
        {
            _niveau = Mathf.Clamp(value, 0, int.MaxValue);
            _evenementMiseAJour.Invoke();
        }
    }
    public int argent 
    { 
        get => _argent;
        set  
        {
            _argent = Mathf.Clamp(value,0, int.MaxValue); //pour etre sur que c'est pas un nombre négatif ou trop gros
            evenementMiseAJour.Invoke(); //appel evenement miseAJour lorsque l'argent est set
        }
    }
    public float compteur
    { 
        get => _compteur;
        set  
        {
            _compteur = Mathf.Clamp(value,0, int.MaxValue); //pour etre sur que c'est pas un nombre négatif ou trop gros
            evenementMiseAJour.Invoke(); //appel evenement miseAJour lorsque l'argent est set
        }
    }
    
    UnityEvent _evenementMiseAJour = new UnityEvent(); //crée un event
    public UnityEvent evenementMiseAJour => _evenementMiseAJour;
    List<SOObjet> _lesObjets = new List<SOObjet>(); //liste d'objets que joueur a

    public void Initialiser()
    {
        
        _argent = _argentIni; //début argent = argent initiale
        _niveau = _niveauIni;
        _compteur = _compteurIni;
        _nbVies = _nbViesIni;
        _lesObjets.Clear();//y a pas d'objets au début
        
    }
    /// <summary>
    /// Lorsqu'on achète un objet on y passe les donnees de l'objet
    /// argent - le prix
    /// ajoute l'objet à l'inventaire
    /// si nom = bottes il a les bottes pour le niveau pareil pour double points
    /// </summary>
    /// <param name="donneesObjet"></param>
    public void Acheter(SOObjet donneesObjet)
    {
        argent -= donneesObjet.prixDeBase;
        _lesObjets.Add(donneesObjet);
        _lesObjets.Sort((x,y) => x.nom.CompareTo(y.nom));
        AfficherInventaire(); //appel fonction
        if(donneesObjet.nom == "PotionVite")
        {
            Perso.instance.UtiliserBottes();
            Niveau.instance._champVitesseX2.text = "allooo";
        }
        if(donneesObjet.nom == "Potion2xPoints")
        {
            Perso.instance.UtiliserDoublePoints();
        }
        if(donneesObjet.nom == "Boussole")
        {
            Niveau.instance.aBoussole = true;
            Debug.Log("he");
        }

    }
    /// <summary>
    /// Afficher l'inventaire dans la boutique
    /// </summary>
     public void AfficherInventaire() //ajoute le nom de l'objet à l'inventaire
    {
        if(Boutique.instance == null) return;
        Boutique.instance.panneauInventaire.Vider(); //vide l'inventaire tp4
        PanneauVignette panneauVignette = null;
        foreach (SOObjet objet in _lesObjets) ///chaque objet si les donnees = l'objet ajoute l'objet et son nombre tp4
        {
            
            if(panneauVignette != null)
            {
                if(panneauVignette.donnees == objet)
                {
                    panneauVignette.nb++;
                    continue;
                }
            }
            panneauVignette = Boutique.instance.panneauInventaire.Ajouter(objet);
            
            
        }
        
    }
    void OnValidate() //lorsqu'on valide appel l'event
    {
        _evenementMiseAJour.Invoke();
    }
    /// <summary>
    /// Efface l'inventaire #synthese
    /// </summary>
    public void EffacerInventaire() 
    {
        _lesObjets.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//tp4
/// <summary>
/// Ajoute une vignette d'un objet à l'inventaire sur la scène de la boutique
/// </summary>
public class PanneauInventaire : MonoBehaviour
{
    [SerializeField] PanneauVignette _panneauVignetteModele; //modele d'une vignette
    /// <summary>
    /// Instancie un modele d'une vignette, donne les données reçues
    /// </summary>
    /// <param name="donnees">Donnees d'un objet</param>
    /// <returns></returns>
    public PanneauVignette Ajouter(SOObjet donnees)
    {
        PanneauVignette panneauVignette = Instantiate(_panneauVignetteModele, transform);
        panneauVignette.donnees = donnees;
        return panneauVignette;
    }
    /// <summary>
    /// enlève les vignettes dans panneauinventaire
    /// </summary>
    public void Vider() 
    {
        foreach(Transform enfant in transform) Destroy(enfant.gameObject); //pour chaque vignette dans panneau enlève la
    }
    void Start() 
    {
        if(Boutique.instance != null) Boutique.instance.donnesPerso.AfficherInventaire(); //si boutique affiche inventaire
    }
    
}


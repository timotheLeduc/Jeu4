using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// Sert à gérer les panneaux des objets
/// </summary>
public class PanneauObjet : MonoBehaviour
{
    [Header("LES DONNÉES")] //dans inspecteur on voit données comme titre
    [SerializeField] SOObjet _donnees;
    public SOObjet donnees => _donnees;

    [Header("LES CONTENEURS")]
    //les champs d'un objet
    [SerializeField] TextMeshProUGUI _champNom; //nom
    [SerializeField] TextMeshProUGUI _champPrix; //prix
    [SerializeField] TextMeshProUGUI _champDescription; //description
    [SerializeField] Image _image; //image
    [SerializeField] CanvasGroup _canvasGroup; //regroupe le panneau objet et ses infos

    void Start()
    {
        MettreAJourInfos(); //appel de fonction
        Boutique.instance.donnesPerso.evenementMiseAJour.AddListener(MettreAJourInfos); //abonner MettreAJourInfos
    }
    /// <summary>
    /// Mettre à jour les infos d'un objet
    /// </summary>
     void MettreAJourInfos()
    {
        _champNom.text = _donnees.nom; // = scriptable object associé envoie ses donnees de SOObjet
        _champPrix.text = _donnees.prixDeBase + "$";
        _champDescription.text = _donnees.description;
        _image.sprite = _donnees.sprite;
        GererDispo(); //check si y est disponible
    }
    /// <summary>
    /// Savoir si l'objet est disponible
    /// </summary>
    void GererDispo()
    {
        bool aAssezArgent = Boutique.instance.donnesPerso.argent >= _donnees.prixDeBase; //si il a plus d'argent que prix de l'objet on peut l'acheter et interagir sinon non
        if(aAssezArgent)
        {
            _canvasGroup.interactable = true;
            _canvasGroup.alpha = 1;
        }
        else
        {
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = .5f;
        }
        
    }
    /// <summary>
    /// Lorsqu'on clique sur un objet, on l'achète
    /// </summary>
    public void Acheter() //appel Acheter et passe donnees
    {
        Boutique.instance.donnesPerso.Acheter(_donnees);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//tp4
/// <summary>
/// les données d'un objet qu'on assigne à une vignette
/// </summary>
public class PanneauVignette : MonoBehaviour
{
    [SerializeField] Image _image; //image de vignette
    [SerializeField] TextMeshProUGUI _champ; //le nombre de fois qu'on l'a acheté
    SOObjet _donnees; //donnees de l'objet acheté
    int _nb  = 1; //nombre qu'on a acheté
    public int nb 
    {   
        get => _nb; 
        set {
            _nb = value; 
            _champ.text = _nb + "";
        } 
    }
    public SOObjet donnees { get => _donnees; set => _donnees = value; }

    // Start is called before the first frame update
    void Start()
    {
        _image.sprite = donnees.sprite; //assigne les variables
        nb = nb;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


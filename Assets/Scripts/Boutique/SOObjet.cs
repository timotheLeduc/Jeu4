using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Objet", menuName = "Objet boutique")] //crée scriptable object quand on veut
/// <summary>
/// Les donnnées de l'objet
/// </summary>
public class SOObjet : ScriptableObject
{
    [Header("LES DONNÉES")]
    [SerializeField] string _nom = "obj"; //nom objet
    [SerializeField][Tooltip("Image")] Sprite _sprite; //l'image de l'objet
    [SerializeField][Range(0, 200)] int _prixDeBase = 30; //prix de l'objet
    [SerializeField][TextArea] string _description; //description
    //fait des getters et setters 
    public string nom { get => _nom; set => _nom = value; }
    public Sprite sprite { get => _sprite; set => _sprite = value; }
    public int prixDeBase { get => _prixDeBase; set => _prixDeBase = Mathf.Clamp(value, 0, int.MaxValue); }
    public string description { get => _description; set => _description = value; }
}
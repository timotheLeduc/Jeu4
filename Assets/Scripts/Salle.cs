using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// La position de la salle dans la grille de salles, la position des repères dans la salle	et instanciation des ennemis
/// </summary>
public class Salle : MonoBehaviour
{
    static Vector2Int _taille = new Vector2Int(18,18);//déclaration variable statique  qui représente la taille de chaque salles
    static public Vector2Int taille => _taille;  //getter de taille
    [Header("Repères")]
    [SerializeField] Transform _repereCle; /// la position du repère tp3
    [SerializeField] Transform _reperePorte;
    [SerializeField] Transform _repereActivateur;
    [SerializeField] Transform _repereEffector;
    [SerializeField] Transform _repereEnnemi; //la position qu'aura le deuxieme ennemi #sythese

    [Header("Waypoints")]
    [SerializeField] public List<Transform> _waypoints;//liste de waypoints #synthese

    [Header("Ennemi")]
    [SerializeField] Ennemi _prefabEnnemi; //prefab ennemi #synthese
    [SerializeField] Ennemi2 _prefabEnnemi2; //prefab ennemi #synthese

    private int _nbEnnemiHasard; //nombre d'ennemis à instancier #synthese
    

    public void Tester()
    {
        Debug.Log($" {name} {transform.position}");
    }
    /// <summary>
    /// dessine le contour de la salle
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_taille.x, _taille.y, 1));
    }
    /// <summary>
    /// Place un objet sur le repère mit sur la scène
    /// Différents repres dans chaque fonction
    /// //retourne la position de la tuile qui a l'objet
    /// TP3
    /// </summary>
    /// <param name="modele"></param>
    /// <returns></returns>
    public Vector2Int PlacerCleSurRepere(GameObject modele)
    {
        Vector3 pos = _repereCle.position;
        Instantiate(modele, pos, Quaternion.identity, transform.parent);
        return Vector2Int.FloorToInt(pos); 
    }
    public Vector2Int PlacerPorteSurRepere(GameObject modele)
    {
        Vector3 pos = _reperePorte.position;
        Instantiate(modele, pos, Quaternion.identity, transform.parent);
        return Vector2Int.FloorToInt(pos);
    }
    public Vector2Int PlacerActivateurSurRepere(GameObject modele)
    {
        Vector3 pos = _repereActivateur.position;
        Instantiate(modele, pos, Quaternion.identity, transform.parent);
        return Vector2Int.FloorToInt(pos);
    }
    public Vector2Int PlacerEffectorSurRepere(GameObject modele)
    {
        Vector3 pos = _repereEffector.position;
        Instantiate(modele, pos, Quaternion.identity, transform.parent);
        return Vector2Int.FloorToInt(pos);
    }
    public void Start()
    {
        // foreach (Transform waypoint in _waypoints)
        // {
        //     // Do something with the transform
        //     Debug.Log(waypoint.position + "sxcx");
        // }
        _nbEnnemiHasard = Random.Range(1,2); //nombre d'ennemis à instancier #synthese
        if(_nbEnnemiHasard == 1 || _nbEnnemiHasard == 2) //si le nombre d'ennemis est 1 ou 2 #synthese
        {
           Ennemi ennemi = Instantiate(_prefabEnnemi, _waypoints[0].position, Quaternion.identity, transform.parent); //instancie un ennemi à la position d'un waypoint #synthese
           Ennemi2 ennemi2 = Instantiate(_prefabEnnemi2, _repereEnnemi.position, Quaternion.identity, transform.parent); //instancie un ennemi2 à la position d'un waypoint #synthese

        }
        
    
    }
    public void Awake()
    {
        Debug.Log(_waypoints[0] + "sssaa");
        Debug.Log(Niveau.instance+ "aaaaas");
        Debug.Log(Niveau.instance.waypoints+ "aaaaas");
        Niveau.instance.AjouterRepere(_waypoints[0]); //ajoute un repere à la position d'un waypoint #synthese
        Niveau.instance.AjouterRepere(_waypoints[1]); //ajoute un repere à la position d'un waypoint #synthese
    }
}

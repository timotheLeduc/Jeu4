using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Sert à détruire les tuiles de la carte selon un pourcentage et les envoyer au niveau
/// </summary>
public class CarteTuiles : MonoBehaviour
{
    //le slider pour les probabilités 0 à 100
    [Header("Pourcentage Probabilité")]
    [Range(0, 4)]
    [SerializeField] private int _pourcentageConserve; //chaque tuile a son pourcentage séréalizé grâce au slider de 1 à 4
    public int pourcentageConserve => _pourcentageConserve;  //getter de pourcentage
    float _alpha;
    

    void Awake()
    {
        
        int hasard = Random.Range(0,pourcentageConserve); //nombre hasard 0 à 4
        Tilemap tm = GetComponent<Tilemap>(); //sa tilemap
        BoundsInt bounds = tm.cellBounds; //boundsint pour avoir les limites de la tilemap
        Niveau niveau = GetComponentInParent<Niveau>(); //va chercher le script du parent niveau
        Vector3Int decalage = Vector3Int.FloorToInt(transform.position); //pour que la position soit toujours de entiers
        bool doitRester = false; //est-ce que la tilemap doit rester
        if(hasard == 0) //si le hasard = 0 on garde la tilemap
        {
            Debug.Log("Je majoute");
            doitRester = true;
            for(int y = bounds.yMin; y < bounds.yMax; y++) // boucle dans une boucle pour passer sur toutes les tuiles de la tilemap
            {
                for(int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    Vector3Int pos = new Vector3Int(x,y,0); //augmente la position chaque ligne en x et y
                    niveau.TraiterTuile(tm, doitRester, niveau, pos, decalage); //transfert de tuiles au parent niveau
                    doitRester = !doitRester; 
                }
                doitRester = !doitRester;
            }
            gameObject.SetActive(false); //une fois le transfert fait, enleve toi
        }
        else{
            gameObject.SetActive(false); //Il n'a pas passé les probabilités alors il s'enlève
        }
    }
/// <summary>
/// Pour calculer les probabilités et changer l'alpha 0 = 1 , 1 = 0.75
/// </summary>
private void OnDrawGizmos()
    {
        CalculerAlpha();

        Tilemap tm = GetComponent<Tilemap>(); //la tilemap de la carteTuiles
        tm.color = new Color(1, 1, 1, _alpha); //change alpha

    }

    private void CalculerAlpha()
    {
        if (pourcentageConserve == 0)
        {
            _alpha = 1;
        }
        else if (pourcentageConserve == 1) _alpha = 0.75f;
        else if (pourcentageConserve == 2) _alpha = 0.50f;
        else _alpha = 0.25f;
    }
}

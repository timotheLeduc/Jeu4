using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#synthese
/// <summary>
/// La classe de l'ennemi qui se déplace dans le niveau grâce à des waypoints et qui instancie un cadeau au hasard lorsqu'il meurt
/// </summary>
public class Ennemi : MonoBehaviour
{
    //Transform[] _waypoints;
    List<Transform> _waypoints = new List<Transform>(); //liste des waypoints qui servent au déplacement de l'ennemi à travers le niveau
    [SerializeField] float _vitesse = 2f; //la vitesse de l'ennemi
    
    Rigidbody2D _rb; //le rigid body de l'ennemi
    SpriteRenderer _sr;
    int _iDest; //sert à connaitre la position que l'ennemi va devoir aller
    [SerializeField] float _toleranceDest = 0.1f; //variable qui représente la distance à laquelle l'ennemi considère qu'il a atteint sa destination
    [SerializeField] GameObject[] _tPrefabsCadeaux; //tableau de prefabs que l'ennemi instancie lorsqu'il meurt, soit or = 10 points ou bombe qui enlève une vie au joueur
    
    // Start is called before the first frame update
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _waypoints = Niveau.instance.waypoints; //récupère la liste des waypoints dans la classe Niveau
        _iDest = Random.Range(0, _waypoints.Count); //choisit un waypoint de manière aléatoire que l'ennemi ira vers
        StartCoroutine(CoroutineGererTrajet()); //démarre la coroutine pour gérer le déplacement de l'ennemi
        
    }
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>(); //récupère le Rigidbody2D attaché à l'ennemi
       
    }

    // Update is called once per frame
    void Update()
    {
           
    }
    /// <summary>
    /// Instancie un cadeau aléatoire lorsque l'ennemi meurt
    /// </summary>
    /// <param name="ennemi">Le gameobject de l'ennemi</param>
    public void InstancierCadeau(GameObject ennemi)
    {
        int nbRandom = Random.Range(0,_tPrefabsCadeaux.Length); //choisit un cadeau au hasard dans le array
        GameObject cadeau = Instantiate(_tPrefabsCadeaux[nbRandom], ennemi.transform.position, Quaternion.identity); //instancie un cadeau random à position de l'ennemi tué
        //Destroy(ennemi.gameObject);
    }

    /// <summary>
    /// Gère le déplacement de l'ennemi entre les waypoints
    /// </summary>
    /// <returns></returns>
    IEnumerator CoroutineGererTrajet()
    {
        //yield return new WaitForSeconds(_delaiPremierDepart);
         //attendre avant de démarrer
         while(true) //boucle infinie
        {
            Vector2 posDest = ObtenirPosProchaineDestination(); //obtenir la prochaine destination
             while(Vector2.Distance(transform.position, posDest) > _toleranceDest) //tant que l'ennemi n'a pas atteint sa destination
            {
                //ajouter de la force vers la destination
                //AjouterForceVersDestination(posDest);
                //attendre la prochaine frame (physique!)
                yield return new WaitForFixedUpdate(); //attend la prochaine frame physique
            }

            
        }
       
    }
    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        Perso perso = other.GetComponent<Perso>(); //si il entre en collision avec le perso et que l'objet a le script perso enlève une vie
        if(perso != null)
        {
            perso.EnleverVie();
            Niveau.instance.JouerSon(0);
        }
        
    }
    /// <summary>
    /// Choisit un waypoint de manière aléatoire et retourne sa position
    /// </summary>
    /// <returns></returns>
    Vector2 ObtenirPosProchaineDestination()
    {
        _iDest = Random.Range(0, _waypoints.Count); //choisit la prochaine destination à aller dans le array des waypoints
        
        Vector2 pos = _waypoints[_iDest].position;
        return pos; //retourne la position du waypoint aléatoire
    }
    void FixedUpdate()
    {
        Vector3 direction = (_waypoints[_iDest].position - transform.position).normalized; // bouge l'ennemi de son waypoint au prochain
        _rb.velocity = direction * _vitesse;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, 90) * direction); //permet d'orienter l'ennemi vers le prochain waypoint

        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.fixedDeltaTime); //rend la rotation plus smooth grâce au slerp

        
    }








    
}

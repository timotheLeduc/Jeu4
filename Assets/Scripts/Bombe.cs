using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//#synthese
/// <summary>
/// Lorsqu'un ennemi instancie une bombe,une animation d'explosion joue, ça fait perdre une vie au joueur et ça détruit les tuiles qui sont dans son rayon
/// </summary>
public class Bombe : MonoBehaviour
{
    private Tilemap _tm; //une variable de type Tilemap qui va stocker la Tilemap du niveau sur laquelle la bombe va exploser.
    [SerializeField] int _rayonExplosion; //un entier qui représente le rayon de l'explosion de la bombe.
    public LayerMask _joueurLayer; //une variable de type LayerMask qui va stocker le layer de l'objet "joueur" dans le jeu.
    [SerializeField] float _delai = 2f; //un float qui représente le temps que la bombe doit attendre avant d'exploser.
    [SerializeField] GameObject _prefabExplosion; //un GameObject qui représente l'animation d'explosion de la bombe.
    [SerializeField] float _explosionDuree = 1f; //un float qui représente la durée de l'animation d'explosion.
    private Animator _anim;//L'animator de la bombe
    private SpriteRenderer _sr; //le sprite renderer
    // Start is called before the first frame update
    void Start()
    {
        _tm = Niveau.instance.tilemap; // On récupère la Tilemap du niveau en utilisant l'instance du script Niveau
        StartCoroutine(ExplosionCoroutine(_delai)); // On lance la coroutine ExplosionCoroutine avec le délai configuré pour déclencher l'explosion
        _anim = GetComponent<Animator>(); //composante Animator
        _sr = GetComponent<SpriteRenderer>(); //composante sprite renderer
        _anim.Play("bombe"); //Joue animation de bombe 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Cette coroutine anime l'explosion de la bombe en utilisant une animation qui change la grosseur
    /// </summary>
    /// <param name="explosion">Le gameobject de l'explosion</param>
    /// <param name="duration">La durée de l'explosion</param>
    /// <returns></returns>
    private IEnumerator AnimerExplosion(GameObject explosion, float duration)
    {
        float tempsDebut = Time.time;//sert à connaitre le temps lorsque la bombe est instanciée
        while (Time.time < tempsDebut + duration) //tant que le temps écoulé est pas plus petit que le temps du début +  ex: "2 secondes"
        {
            float t = (Time.time - tempsDebut) / duration; //diminue le temps restant
            explosion.transform.localScale = Vector3.one * (_rayonExplosion * t);
            yield return null;
        }
        explosion.transform.localScale = Vector3.one * (_rayonExplosion); // On finalise l'animation en fixant la grosseur à sa taille maximale
        DetruireTuiles(transform.position); // On détruit les tuiles dans le rayon de l'explosion
        Destroy(gameObject); //Detruit gameobject et l'explosion
        Destroy(explosion, 1.5f);
    }
    /// <summary>
    /// Cette fonction détruit les tuiles dans le rayon de l'explosion
    /// </summary>
    /// <param name="center">Le centre de la circonférence de l'explosion</param>

    public void DetruireTuiles(Vector3 center)
    {
        Vector3Int tilePosition = _tm.WorldToCell(center); // On récupère la position correspondant au centre de l'explosion

        for (int x = -_rayonExplosion; x <= _rayonExplosion; x++) //la boucle dans la boucle sert à chercher les tuiles qui sont dans le rayon  de l'explosion en x et y
        {
            for (int y = -_rayonExplosion; y <= _rayonExplosion; y++)
            {
                Vector3Int tilePos = new Vector3Int(tilePosition.x + x, tilePosition.y + y, tilePosition.z); //la position de la tuile trouvée par la boucle
                _tm.SetTile(tilePos, null); //Détruit la tuile en la mettant null
            }
        }
        if(Niveau.instance.donneesPerso.nbVies <= 0) //si le joueur n'a plus de vie
        {
            Niveau.instance.FinDePartie(); //appel fonction dans niveau pour écrire et lire fichier sauvegarde
        }
    }
    /// <summary>
    /// Instancie l'explosion et si le joueur est dans le rayon enlève une vie
    /// </summary>
    /// <param name="delai">Le temps avant que l'explosion commence</param>
    /// <returns></returns>
    public IEnumerator ExplosionCoroutine(float delai)
    {
        yield return new WaitForSeconds(delai); //attend avant de commencer l'explosion
        _sr.enabled = false;
        GameObject explosion = Instantiate(_prefabExplosion, transform.position, Quaternion.identity); //instancie explosion à position de la bombe
        explosion.transform.localScale = Vector3.zero; //L'explosion est tout petite au début
        StartCoroutine(AnimerExplosion(explosion, _explosionDuree)); //appel coroutine animer explosion 
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _rayonExplosion, _joueurLayer); //sert à savoir si le joueur est dans le rayon de l'explosion
        foreach (Collider2D collider in colliders) //si l'objet qui est dans le array a un tag Player enlève une vie au joueur
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("Touché par l'explosion");
                Niveau.instance.donneesPerso.nbVies--;
                Niveau.instance._champVies.text =  Niveau.instance.donneesPerso.nbVies + "";
                Niveau.instance.JouerSon(0);
            }
        }
    
        // explosion.transform.localScale = Vector3.one * (_rayonExplosion * 2f);
        // Destroy(explosion, 0.5f);
    }
    /// <summary>
    /// Dessine un cercle afin que le joueur puisse voir la grandeur de l'explosion
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; //change la couleur du gizmo à rouge
        Gizmos.DrawWireSphere(transform.position, _rayonExplosion); //dessine un cercle qui a un rayon gros comme l'explosion
    }
}

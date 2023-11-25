using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Le script de l'ennemi 2 qui saute et qui ralentit le joueur #synthese
/// </summary>
public class Ennemi2 : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rb; //le rigidbody de l'ennemi
    [SerializeField] float _forceSaut = 10f; //la force de saut
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SautCoroutine()); //appel coroutine
    }
    /// <summary>
    /// La coroutine qui permet à l'ennemi de sauter à chaque seconde
    /// </summary>
    /// <returns></returns>
    IEnumerator SautCoroutine() 
    {
        while (true) //boucle infinie
        {
            yield return new WaitForSeconds(1f); // attend 1 seconde

            if (Mathf.Abs(_rb.velocity.y) < 0.01f) // si il est pas déjà entrain de tomber
            {
                _rb.AddForce(Vector2.up * _forceSaut, ForceMode2D.Impulse); // donne de la force vers le haut
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Perso perso = other.GetComponent<Perso>(); //si il entre en collision avec le perso et que l'objet a le script perso diminue vitesse
        if(perso != null)
        {
            perso.DiminuerVitesse();
            Niveau.instance.JouerSon(0);
        }
        
    }
}

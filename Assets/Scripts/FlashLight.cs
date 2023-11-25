using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#synthese

/// <summary>
/// Ce script est attaché à la lumière du joueur. Lorsqu'elle est en collision avec l'ennemi, appel la fonction pour instancier cadeau dans script ennemi
/// </summary>
public class FlashLight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Lorsque la lampe torche entre en collision avec l'ennemi, va parler au script de l'ennemi précis pour que l'ennemi lâche un cadeau
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Ennemi")) //si la flashlight entre en collision avec ennemi
        {
            Niveau.instance.JouerSon(2); //joue son de zapper
            Destroy(other.gameObject); //detruit objet
        }
        Ennemi ennemi = other.GetComponent<Ennemi>(); //le script de l'ennemi
        if(ennemi != null) //si il a un script ennemi
        {
            ennemi.InstancierCadeau(other.gameObject); //appel de fonction instancier cadeau et passe le gameobject de l'ennemi
            //Debug.Log("ennemi");
            Destroy(other.gameObject); //Detruit l'ennemi
        }
        
        
    }

    
    
}

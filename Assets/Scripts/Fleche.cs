using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#synthese
/// <summary>
/// La flèche qui indique la direction de l'objet à trouver #synthese
/// </summary>
public class Fleche : MonoBehaviour
{
    GameObject target; //objet à trouver #synthese
    private void Start()
    {
        //target =  Niveau.instance.cleModele;
        target = GameObject.FindWithTag("Cle"); //trouve l'objet avec le tag cle
        
        if(Niveau.instance.aBoussole == true) //si on a la boussole
        {
            
            gameObject.SetActive(true);
        }else{
            
            gameObject.SetActive(false);
        }
    }
    private void Update()
    {
    
        if (target == null) //si on a pu de cible
        {
            
            target = GameObject.FindWithTag("Porte"); //trouve la porte
        }
        if(target != null)
        {
           

        
        Vector3 direction = target.transform.position - transform.position; //direction de la cible

        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; //angle de la cible

        
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); //rotation de la cible
        }

        
    }
}
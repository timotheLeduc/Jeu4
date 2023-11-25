using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//#tp3
/// <summary>
/// Lorsque le joueur entre en collision avec l'activateur, appel l'event et devient pu interactif
/// </summary>
public class Activateur : MonoBehaviour
{
    [SerializeField] private float _alpha = 0.5f; //le alpha de la couleur du sprite 
    private Collider2D _collider; //parler à son collider
    private SpriteRenderer _sr; //le sprite renderer
    void Start()
    {
        _collider = GetComponent<Collider2D>(); //va chercher les composantes
        _sr = GetComponent<SpriteRenderer>();

    }
    UnityEvent _evenementMiseAJour = new UnityEvent(); //crée un Unity event
    public UnityEvent evenementMiseAJour => _evenementMiseAJour; //faire un get
    public void OnTriggerEnter2D(Collider2D other) //si le joueur collision avec activateur appel le event et devient pu interactif
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ssssss");
            Niveau.instance.AppelerEvent();
            ChangerInteractif();
        }
    }
    /// <summary>
    /// change alpha et devient pu cliquable par perso
    /// </summary>
    public void ChangerInteractif() 
    {
        _sr.color = new Color(1,1,1,_alpha);
        _collider.enabled = false;
    }
}

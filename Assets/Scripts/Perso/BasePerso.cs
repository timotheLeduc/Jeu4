using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Vérifie si le personnage touche le sol
/// </summary>
public class BasePerso : MonoBehaviour
{
    [SerializeField] float _distanceDebutSol = 0.5f; 
    
    [SerializeField] LayerMask _layerMask;
    protected bool _estAuSol;
    Vector2 _grosseur = new Vector2(0.2f,0.2f);
    virtual protected void FixedUpdate()
    {
        VerifierSol();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    ///  Fonction qui vérifie si le personnage touche le sol
    /// </summary>
    private void VerifierSol()
    {
        
        
        Vector2 pointDepart = (Vector2)transform.position - new Vector2(0, _distanceDebutSol); //point centre du OverLapBox
        
        
        _estAuSol = Physics2D.OverlapBox(pointDepart,_grosseur,0, _layerMask); // vrai si overlap box touche le sol
        
        

       
    }
    /// <summary>
    /// Dessine carré de détection, si touche au sol devient vert sinon rouge
    /// </summary>
    void OnDrawGizmos() 
    {
        if(Application.isPlaying == false) VerifierSol(); //si jeu joue pas check pareil
        if(_estAuSol) Gizmos.color = Color.green; 
        else Gizmos.color = Color.red;
        Vector2 pointDepart = (Vector2)transform.position - new Vector2(0, _distanceDebutSol);
        Gizmos.DrawWireCube(pointDepart, _grosseur);
    }
}

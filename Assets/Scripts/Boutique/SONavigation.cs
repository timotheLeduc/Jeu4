using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[CreateAssetMenu(fileName = "Navigation", menuName = "Navigation")] //crée un scriptable object
/// <summary>
/// Crée un scriptable object qui permet de naviguer entre les scènes
/// </summary>
public class SONavigation : ScriptableObject
{
    [SerializeField] SOPerso _donneesPerso;
    public void Jouer()
    {
        AllerSceneSuivante();
        _donneesPerso.Initialiser(); //va à scène jeu et initialise donnees
    }
    public void SortirBoutique()
    {
        //_donneesPerso.niveau++;
        AllerScenePrecedenteDeDeux(); //écran accueil
    }
    public void AllerSceneSuivante() //va à la scène +1 de la scene qu'on est
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
    public void AllerScenePrecedente() //va à la scène -1 de la scene qu'on est
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }
    public void AllerScenePrecedenteDeDeux(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-2);
    }
    public void AllerSceneSuivanteeDeux(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+2);
    }
    public void AllerSceneSuivanteTrois(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+3);
    }
    public void AllerScenePrecedenteTrois(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-3);
    }
    
}

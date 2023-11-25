using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//tp4
/// <summary>
/// Le scriptable object d'une piste pour lui assigner ses paramètres
/// </summary>
[CreateAssetMenu(menuName = "Piste musicale", fileName = "DonneesPiste")]
public class SOPiste : ScriptableObject
{
    [SerializeField] TypePiste _type; //La piste musicale
    [SerializeField] AudioClip _clip; //Le clip
    [SerializeField] bool _estActifParDefaut; //est-ce qu'il commence à jouer au début
    [SerializeField] bool _estActif;
    AudioSource _source; //L'audiosource
    //des getters setters
    public AudioSource source => _source;
    public TypePiste type => _type;
    public AudioClip clip => _clip;
    public bool estActif
    {
        get => _estActif;
        set
        {
            _estActif = value;
           
        }
    }
    
    /// <summary>
    /// Appelé au début de GestAudio et donne des paramètres aux musiques sources
    /// </summary>
    /// <param name="source">Le audio source qu'on passe</param>
    public void Initialiser(AudioSource source)
    {
        _source = source;
        _source.clip = _clip; //assigne un clip qu'on a drag dans inspecteur
        _source.loop = true; //joue en continue
        _source.playOnAwake = false; //joue pas au début
        _source.Play(); //On les commence en même temps
        _estActif = _estActifParDefaut; //si y est actif par defaut on met le son
        AjusterVolume();

    }
    
    /// <summary>
    /// Si il estActif met le son à 1 sinon joue la mais au son de 0
    /// </summary>
    public void AjusterVolume()
    {
        if(estActif) _source.volume = GestAudio.instance.volumeMusiqueRef;
        else _source.volume = 0; 
    }

}

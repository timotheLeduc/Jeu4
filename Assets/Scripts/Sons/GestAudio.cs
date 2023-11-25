using System.Collections;
using UnityEngine;
//tp4
/// <summary>
/// Permet de jouer une musique ou un son en fade in et de l'arrêter en fade out
/// </summary>
public class GestAudio : MonoBehaviour
{
    [SerializeField] float _volumeMusiqueRef = 1; //son musique
    public float volumeMusiqueRef => _volumeMusiqueRef; //getter
    [SerializeField] SOPiste[] _tPistes; //tableau des pistes
    public SOPiste[] tPistes => _tPistes;
    AudioSource _sourceEffetsSonores;
    static GestAudio _instance; //crée singleton
    static public GestAudio instance => _instance;
    [SerializeField] public AudioClip[] _clips; //tableau clips

    void Awake()
    {
        if (_instance == null) _instance = this; //si un gestionnaire audio existe pas garde le sinon remplace le
        else
        {
            Debug.Log("Un gestionnaire audio existe déjà, donc celui sur la scène sera détruit");
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);//on veut qu'il reste à travers les scènes
        _sourceEffetsSonores = gameObject.AddComponent<AudioSource>(); //va chercher le component
        
        CreerLesSourcesMusicales();
    }
    /// <summary>
    /// pour chaque piste dans tableau ajoute une composante et assigne-lui des paramètres
    /// </summary>
    private void CreerLesSourcesMusicales()
    {
        foreach (SOPiste piste in _tPistes)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>(); //crée un audiosource
            piste.Initialiser(source); //appel de fonction qui assigne des propriétés
        }
    }


    /// <summary>
    /// Savoir si il joue le son ou pas
    /// </summary>
    /// <param name="type">quel type de piste(Base,A,B)</param>
    /// <param name="estActif">Passe un bool pour savoir si on joue ou non la musique</param>
    public void ChangerEtatLecturePiste(TypePiste type, bool estActif)
    {
        foreach (SOPiste piste in _tPistes) //pour chaque musique dans le tableau
        {
            if (piste.type == type)//si la piste est = à la piste dans l'array qu'on a passé
            {
                piste.estActif = estActif; //joue la musique si true en appelant la coroutine fadeIn
                if(estActif == true)
                {
                    Debug.Log("FadinIN");
                    StartCoroutine(FadeIn(piste.source, GestAudio.instance.volumeMusiqueRef, 1.5f)); //appel coroutine et passe la musique, son, temps que la coroutine prend
                    
                }
                else{
                    Debug.Log("FadinOUT");
                    StartCoroutine(FadeOut(piste.source, 2f)); //appel coroutine pour fadeOut lorsqu'on ferme un son
                }
                return;
            }
        }
    }
    /// <summary>
    /// Coroutine qui fadeIn le son lorsqu'on active une musique
    /// </summary>
    /// <param name="audioSource">Le audio source de la piste</param>
    /// <param name="volumeMusique">Volume de la musique à atteindre</param> 
    /// <param name="fadeTime">La durée du fade In</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator FadeIn(AudioSource audioSource, float volumeMusique, float fadeTime)
    {
        float sonDepart = 0;//son commence 0
        audioSource.volume = sonDepart; //le volume de la source = 0
        while (audioSource.volume < volumeMusique)//tant que le volume est plus petit que 1
        {
            audioSource.volume += Time.deltaTime / fadeTime; //augmente volume nombre / 1.5f
            yield return null;
        }
        audioSource.volume = volumeMusique; //volume = 1
    }
    /// <summary>
    /// Coroutine qui fadeOut lorsqu'on arrête la musique
    /// </summary>
    /// <param name="audioSource">Le audio source de la piste</param>
    /// <param name="tempsFadeOut">Le temps du fade Out</param>
    /// <returns></returns>
    private IEnumerator FadeOut(AudioSource audioSource, float tempsFadeOut)
    {
        float sonDepart = audioSource.volume; //son de la piste
        Debug.Log(audioSource.volume +" son départ");
        while (audioSource.volume > 0) //tant que c'est plus grand que 0 descend le son jusqu'à ce qu'il atteigne 0
        {
            audioSource.volume -= sonDepart * Time.deltaTime / tempsFadeOut;
            yield return null;
        }
        
    }
    /// <summary>
    /// Joue un son
    /// </summary>
    /// <param name="clip">Joue un son</param>
    public void JouerEffetSonore(AudioClip clip) 
    {
        _sourceEffetsSonores.PlayOneShot(clip);
    }


}
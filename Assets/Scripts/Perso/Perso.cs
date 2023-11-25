using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
/// <summary>
/// Classe du joueur qui détermine les mouvements du joueur et les collisions avec les objets sur la scène
/// </summary>
public class Perso : BasePerso
{
    [Header("Donnees perso")]
    [SerializeField] SOPerso _donnees; //les donnees du perso

    [Header("Scene Navigation")]
    [SerializeField] SONavigation _changeScene; //les donnees de navigation

    [Header("Joueur Mouvement")]
    [SerializeField] float _vitesse = 5; //la vitesse
    float _axeHorizontal; //float pour l'axe horizontal
    Rigidbody2D _rb; //le rigidbody du joueur
    [SerializeField] float _forceSaut = 75f; //sa force de saut

    [Header("Systeme particules")]
    [SerializeField] int _nbFramesMax = 10;
    [SerializeField] ParticleSystem[] _particleSystem;

    [Header("Flashlight")]
    [SerializeField] GameObject _flashlight; //le gameobject de la flashlight #synthese
    [SerializeField] float _tempsActifLumiere = 0.25f; //le temps que la lumière reste active #synthese

    SpriteRenderer _sr; // sprite renderer du joueur
    static int _nbGems = 0; ///static pour que tout le monde ait acces et nb de gems que le joueur a ramssé tp3
    private float _axeVertical; //l'axe vertical (joystick ou clavier)
    int _nbFrameRestants = 0;
    bool _veutSauter = false;
    static Perso _instance; ///crée un singleton pour que d'autres scripts puissent lui parler tp3
    static public Perso instance => _instance; ///faire un get pour pas que n'importe qui puisse le changer tp3
    static bool aLesBottes; ///pour savoir si il a acheté les bottes pour aller plus vite tp3
    static bool aDoublePoints; ///pour savoir si il a acheté le double points tp3
    private CinemachineVirtualCamera _vmCam; /// pour que la caméra puisse suivre le joueur tp3
    ParticleSystem _course; //Le système de particule de course #synthese
    ParticleSystem _saut; //Le système de particule de saut #synthese
    Animator _anim; //l'animator du joueur #synthese
    public bool _aActifActivateur; //tp4 Activer Activateur
    public bool _peutJouerSonSaut = false; //peux-tu jouer son saut tp4
    public bool _etaitEnAir; //était-il en air tp4
    
    private bool _estActifLumiere = false; //est-ce que la lumière est active #synthese
    private bool _estTombe = false; //est-ce qu'il est tombé #synthese
   
    /// <summary>
    /// Changre le texte du nombre de vies #synthese
    /// </summary>
    public void ChangerTexte()
    {
        Niveau.instance._champVies.text = _donnees.nbVies + ""; //le texte est égal au nombre de vies #synthese
    }
    /// <summary>
    /// Jouer particules de course #synthese
    /// </summary>
    public void JouerParticulesCourse()
    {
        _particleSystem[0].Play();
    }
    /// <summary>
    /// Jouer particules de saut #synthese
    /// </summary>
    public void JouerParticulesSaut()
    {
        _particleSystem[1].Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        _flashlight.SetActive(false); //la lampe de poche est désactivée au début #synthese
        ChangerTexte();//appel changer le texte du nombre de vies #synthese
        _course = _particleSystem[0]; //le système de particule de course est égal à la première particule dans le array #synthese
        _saut = _particleSystem[1]; //le système de particule de saut est égal à la deuxième particule dans le array #synthese
        _anim = GetComponent<Animator>(); //composante animator tp4
        if(aLesBottes) Niveau.instance._champVitesseX2.text = "Vitesse X2"; //si il a les bottes, change le texte tp4
        if(aDoublePoints) Niveau.instance._champGemsX2.text = "Joyaux X2"; //si il a double points, change le texte tp4
        if(aDoublePoints) Niveau.instance._champGemsX2.alpha = 1f; //si il a double points, change le texte tp4
        //_particleSystem = gameObject.GetComponentInChildren<ParticleSystem>(); ///va chercher part enfant de joueur tp3
        ParticleSystem.MainModule settings = _course.main; //va chercher les settings de la particule #synthese
        ParticleSystem.MainModule settings2 = _saut.main; //va chercher les settings de la particule #synthese
        settings.startColor = new ParticleSystem.MinMaxGradient(Color.red); //change la couleur de la particule #synthese
        if (_instance != null) { Destroy(gameObject); return; } ///faut qu'y aille une instance sur la scène tp3
        _instance = this;
        _rb = GetComponent<Rigidbody2D>(); //aller chercher les composants de l'objet
        _sr = GetComponent<SpriteRenderer>();
        Debug.Log(_donnees.argent);
        Debug.Log(aLesBottes + "bo");
        if(aLesBottes) //si a acheté bottes à boutique vitesse = 50 sinon 5
        {
            settings.startColor = new ParticleSystem.MinMaxGradient(Color.blue);
            _vitesse = 15;
        }else{
            _vitesse = 5;
        }
        _vmCam = FindObjectOfType<CinemachineVirtualCamera>(); //va chercher la vmcam sur scène
        if (_vmCam != null)
        {
            
            _vmCam.Follow = transform; //suit le joueur
        }
        _particleSystem[0].Stop();
        
    }
    /// <summary>
    /// Lorsqu'on finit le niveau, on initialise tout #synthese
    /// </summary>
    public void Initialiser()
    {
        aLesBottes = false;
        aDoublePoints = false;
        Niveau.instance.aBoussole = false;
    }
    /// <summary>
    /// Coroutine attendre avant de changer scène pour jouer son porte
    /// </summary>
    /// <returns>Ienumerator</returns>
    IEnumerator AppelerAllerScenePrecedenteDeDeux()  
    {
        yield return new WaitForSeconds(0.5f); //  Attend 0.5 secondes
        
        _changeScene.AllerScenePrecedenteDeDeux(); // Appel AllerScenePrecedenteDeDeux()
    }
    /// <summary>
    /// Diminue vitesse de 2 #synthese
    /// </summary>
    public void DiminuerVitesse()
    {
        _vitesse = _vitesse/2;
        StartCoroutine(RevenirVitesseNormaleCoroutine());
    }
    /// <summary>
    /// Remet la vitesse à son état initial après 2 secondes #synthese
    /// </summary>
    /// <returns></returns>
    private IEnumerator RevenirVitesseNormaleCoroutine()
    {
        yield return new WaitForSeconds(2f);
        _vitesse = _vitesse * 2;
    }
    /// <summary>
    /// Enlève de la vie au joueur #synthese
    /// </summary>
    public void EnleverVie()
    {
        
            _donnees.nbVies--; //enlève une vie #synthese
            if(_donnees.nbVies <= 0)
            {
                Niveau.instance.FinDePartie(); //appel fonction dans niveau pour écrire et lire fichier sauvegarde
                _changeScene.AllerSceneSuivante(); //Va à la scène de scores
                
            }
            ChangerTexte(); //change le texte du nombre de vies #synthese
       
    }
    /// <summary>
    /// Sent when an incoming collider makes contact with this object's
    /// collider (2D physics only).
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    // void OnCollisionEnter2D(Collision2D other)
    // {

    // }
    /// <summary>
    /// Si entre en collision avec la porte et qu'il a la clé change de scène et initialise les variables
    /// Si coll avec clé a la clé qui est dans niveau est true pis destroy
    /// Si col avec gem et a double points +2 sinon +1 et ajoute gem au nombre qu'on a
    /// TP3
    /// 
    /// </summary>
    /// <param name="other">L'autre objet en collision</param>
    void OnTriggerEnter2D(Collider2D other)
    {
    
        if(other.CompareTag("Porte")) //si coll avec porte tp4
        {
            if(Niveau.instance.aLaCle) //et si a la clé tp4
            {
                aLesBottes = false; //il n'a plus son bonus tp4
                aDoublePoints = false; //il n'a plus son bonus tp4
                Niveau.instance.aBoussole = false;
                Niveau.instance.aLaCle = false; //initialise la cle et activateur//tp4
                _aActifActivateur = false;
                GestAudio.instance.JouerEffetSonore(GestAudio.instance._clips[3]); //jouer effet sonore de la porte tp4
                GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueEvenA, Niveau.instance.aLaCle); //arrete la musique tp4
                GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueEvenB, _aActifActivateur); //arretela musique tp4
                
                StartCoroutine(AppelerAllerScenePrecedenteDeDeux()); // Appel AllerScenePrecedenteDeDeux() utilisant coroutine coroutine qui attend un nombre de temps tp4
                if(Niveau.instance.donneesPerso.niveau == 5)
                {
                    Niveau.instance.FinDePartie();
                    Niveau.instance.ArretMusique();
                    _changeScene.AllerSceneSuivante();
                }
                
            }
        }
        if(other.CompareTag("Activateur")) //si collision activateur, joue musique even activateur tp4
        {
            _aActifActivateur = true; //il a activé activateur tp4
            GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueEvenB, _aActifActivateur); //appel fonction pour jouer la piste de lecture tp4
            Debug.Log("<color=green>Musique de l'activateur activée</color>"); //debug en couleur tp4
        }
        if (other.CompareTag("Cle")) //si collision cle, joue musique tp4
        {
            GestAudio.instance.JouerEffetSonore(GestAudio.instance._clips[2]); //jouer le son de la clé tp4
            Niveau.instance.aLaCle = true; //il a clé tp4
            GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueEvenA, Niveau.instance.aLaCle); //appel fonction pour jouer la piste de lecture tp4
            Debug.Log("<color=yellow>Musique de la clé activée</color>");
            ParticleSystem partCle = other.GetComponentInChildren<ParticleSystem>(); //parle syst part clé
            partCle.Play(); //joue syst part
            StartCoroutine(DetruireObjet(other.gameObject, partCle.main.duration)); //appel coroutine
        }
        if(other.CompareTag("Or")) //si collision avec or, ajoute 10$ et détruit or #synthese
        {
            _nbGems = 10;
            Niveau.instance.AjouterGems(_nbGems); //ajoute 10$ #synthese
            Destroy(other.gameObject); //détruit or #synthese
        }
        if(other.CompareTag("Gem"))
        {
            ParticleSystem partGem = other.GetComponentInChildren<ParticleSystem>(); //parle syst part clé
            partGem.Play(); //joue syst part
            SpriteRenderer sr = other.gameObject.GetComponent<SpriteRenderer>();
            sr.color = new Color(1f,1f,1f,0f);
            StartCoroutine(DetruireObjet(other.gameObject, partGem.main.duration)); //appel coroutine
            if(aDoublePoints)
            {
                _nbGems = 2;
            }else{
                _nbGems = 1;
            }
            
            Debug.Log(_nbGems);
            Debug.Log("je fonctionne gem");
            Niveau.instance.AjouterGems(_nbGems);
            
        }
        if(other.CompareTag("Bonus"))
        {
            Niveau.instance.nbSautBonus++;
            Debug.Log(Niveau.instance.nbSautBonus);
            Destroy(other.gameObject);
        }
    }
    IEnumerator DetruireObjet(GameObject Objet, float delai) //coroutine appelée après que particule ait fini tp3
    {
        yield return new WaitForSeconds(delai); //attend le nombre de temps que particules finisse et detruis toi tp3
        Destroy(Objet);
    }
    IEnumerator ArretParticule(float delai, ParticleSystem particule) //coroutine appelée après que particule ait fini tp3
    {
        yield return new WaitForSeconds(delai); //attend le nombre de temps que particules finisse tp3
        _course.Stop();
        ParticleSystem.MainModule settings = _course.main;
        settings.startColor = new ParticleSystem.MinMaxGradient(Color.red);
    }
    /// <summary>
    /// Active la flashlight si le joueur clique sur le bouton gauche de la souris #synthese
    /// </summary>
    public void ActiverFlashlight()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!_estActifLumiere) //si la lumiere n'est pas active, active la lumiere et désactive grace à Coroutine #synthese
            {
                _estActifLumiere = true;
                _flashlight.SetActive(true);
                StartCoroutine(DesactiverFlashlight());
                
            }
        }
    }
    /// <summary>
    /// Désactive la flashlight après un certain temps #synthese
    /// </summary>
    /// <returns></returns>
    IEnumerator DesactiverFlashlight()
    {
        yield return new WaitForSeconds(_tempsActifLumiere); //attend le nombre de temps que compteur finisse #synthese
        _estActifLumiere = false;
        _flashlight.SetActive(false);//désactive la lumiere #synthese
    }
    // Update is called once per frame
    /// <summary>
    /// Pour les touches que le joueur va peser
    /// </summary>
    void Update()
    {
        
        ActiverFlashlight(); //appel fonction ActiverFlashlight #synthese
        _veutSauter = Input.GetButton("Jump"); //bouton enfoncé espace
        _axeHorizontal = Input.GetAxis("Horizontal"); //la valeur de la direction de l'usager
        _axeVertical = Input.GetAxis("Vertical");
       if(_axeHorizontal != 0)
       {
        _anim.SetBool("IdleVersCourse", true); //si il bouge horizontale, change animation à true sinon met le false tp4
        JouerParticulesCourse();
        _anim.SetFloat("Vitesse",0.5f);
       }else{
         _anim.SetBool("IdleVersCourse", false);//tp4 arrete course
         _anim.SetFloat("Vitesse",0);
       }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) //ça c'est du gros n'importe quoi _axehorizontal voulait rien savoir meme en raw
        {
            JouerParticulesCourse(); 
           _course.Play();

           StartCoroutine(ArretParticule(_course.main.duration, _course)); //appel coroutine tp3
        }
        if(_veutSauter)
        {
            ParticleSystem.MainModule settings = _saut.main; //change couleur particule saut #synthese
            settings.startColor = new ParticleSystem.MinMaxGradient(Color.yellow); //change couleur particule #synthese
            _saut.Play();
            StartCoroutine(ArretParticule(_saut.main.duration, _saut)); //appel coroutine tp3
        }
        
       
        // changer la position du sprite si joueur va vers gauche sinon change pas car sprite va etre vers droite debut
        if(_axeHorizontal < 0)
        {
            _flashlight.transform.localScale = new Vector3(-2,2,2);
            _sr.flipX = true;
            
        } 
        else if(_axeHorizontal > 0) 
        {
            _sr.flipX = false;
            _flashlight.transform.localScale = new Vector3(2,2,2);
        }
        
        
    
    }
    /// <summary>
    /// Si il était dans les airs et qu'il est au sol #synthese
    /// </summary>
    public void PeutPasSauterAnim()
    {
        _anim.SetBool("PeutSauter", false); //change animation à false si il est au sol #synthese
        _anim.SetBool("tombe", false); //change animation à false si il est au sol #synthese
    }
    /// <summary>
    /// Calculer le saut et si il peut sauter + lorsque la touche espace est enfoncée
    /// </summary>
    override protected void FixedUpdate() //override car il y a un autre FixedUpdate dans classe parent
    {

        

        base.FixedUpdate(); //appel dans parent
        _estTombe = _rb.velocity.y < 0; //si la velocité est plus petite que 0, il tombe #synthese
        if(_estTombe == true)
        {
            Debug.Log("e");
            _anim.SetBool("tombe", true); //change animation à true si il tombe #synthese
        }else{
            _anim.SetBool("tombe", false);
        }
        if(_veutSauter && Niveau.instance.nbSautBonus != 0) ///si nb de saut bonus qui est dans niveau est pas 0 et qu'y clique sur espace saute plus haut et enlève un saut bonus tp3
        {
            _forceSaut = _forceSaut * 1.5f;
            Niveau.instance.nbSautBonus--;
        }else{///sinon saut = 100 tp3
            _forceSaut = 100f;
            
        }
        if(_estAuSol){
            if(_veutSauter){ //si y est au sol et veut sauter
                _anim.SetBool("PeutSauter", true);
                JouerParticulesSaut();
                if(_peutJouerSonSaut) //si il peut jouer le son tp4
                {
                    GestAudio.instance.JouerEffetSonore(GestAudio.instance._clips[0]); //si veut sauter et que jouersonsaut est true joue son saut tp4
                    _peutJouerSonSaut = false; //remet le son saut à false
                }
                
                float fractionDeForce = (float)_nbFrameRestants / _nbFramesMax; //chaque frame, le nombre frames / par nombre max
                float puissance = _forceSaut * fractionDeForce; //puissance du saut = force * fraction (+ que nb frame est gros  + la puissance le fait monter
                _rb.AddForce(Vector2.up * puissance); // propulse vers le haut
                _nbFrameRestants--; // chaque frame -1 frame restant
                
                if(_nbFrameRestants<0)
                {
                    _nbFrameRestants = 0; //pour pas avoir chiffre en bas 0
                } 
            }
            else 
            {
                _peutJouerSonSaut = true; //si est au sol mais saute pas remet à true tp4
                _nbFrameRestants = _nbFramesMax;
            } //si au sol mais veut pas sauter remettre frames au max
        }
        else{
            bool peutSauterPlus = (_nbFrameRestants > 0); // la force peut être augmentée seulement si y est pas en hauteur
            if(_veutSauter && peutSauterPlus){ // si nb maximal de frames pas à 0 et que son doigt est encore enfoncé
                float fractionDeForce = (float)_nbFrameRestants / _nbFramesMax;
                float puissance = _forceSaut * fractionDeForce;
                _rb.AddForce(Vector2.up * puissance);
                _nbFrameRestants--;
                if(_nbFrameRestants < 0) _nbFrameRestants = 0;

            }
            else _nbFrameRestants = 0; // si y peut pas sauter plus frames restants sont à 0 jusqu'à ce qu'il retombe par terre
        }
        if(_etaitEnAir && _estAuSol)
        {
            
            PeutPasSauterAnim(); //si il était dans les airs et qu'il est au sol appel fonction #synthese
            GestAudio.instance.JouerEffetSonore(GestAudio.instance._clips[1]); //si il était dans les airs et qu'il retombe joue son tombe tp4
            
        }


        _rb.velocity = new Vector2(_axeHorizontal * _vitesse, _rb.velocity.y); // change la position du rigid body. La vitesse * vector est la force donné au 2e parametre pour le stocker chaque frame
        _etaitEnAir = !_estAuSol; //si est pas au sol, il est en air tp4
    }
   /// <summary>
   /// ///lorsqu'on ferme le jeu initialiser les donnees tp3
   /// </summary>
    void OnApplicationQuit() 
    {
        _donnees.Initialiser();
        aLesBottes = false;
        aDoublePoints = false;
        Niveau.instance.aBoussole = false;
    }
    /// <summary>
    /// Changer la variable à partir d'un autre script tp3
    /// </summary>
    public void UtiliserBottes() 
    {
        Niveau.instance._champVitesseX2.text = "Vitesse X2"; //Le texte si il a les bottes dans le ui tp4
        Niveau.instance._champVitesseX2.alpha = 1f; //Le texte si il a les bottes dans le ui tp4
        aLesBottes = true;
    
        Debug.Log(aLesBottes);
    }
    /// <summary>
    /// Changer la variable à partir d'un autre script tp3
    /// </summary>
    public void UtiliserDoublePoints() 
    {
        Niveau.instance._champGemsX2.text = "Joyaux X2"; //Le texte si il a double points dans le ui tp4
        Niveau.instance._champGemsX2.alpha = 1f; 
        
        aDoublePoints = true;
        Debug.Log(aDoublePoints);
    }
    public void ArretParticules(ParticleSystem particules)
    {
        particules.Stop();
    }
    public void AjouterObjetJeu(SOObjet objet)
    {
        Debug.Log(objet);
    }
    
}

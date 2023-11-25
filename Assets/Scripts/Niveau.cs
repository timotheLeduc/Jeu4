using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.SceneManagement;
using Cinemachine;
/// <summary>
/// La classe mère qui gère les tilemaps, les modèles, la position des portes et clés tp4
/// </summary>
public class Niveau : MonoBehaviour
{
    [SerializeField] AudioClip[] _sons; //tableau de sons #synthese
    AudioSource _audioSource; //l'audiosource du niveau #synthese
    [SerializeField] Vector2Int _taille; //taille du niveau
    [SerializeField] Tilemap _tilemap;
    public Tilemap tilemap {get => _tilemap;} //getter du tilemap
    [Header("Modeles")]
    [SerializeField] private GameObject[] _tSallesModeles; //les prefabs de salles
    [SerializeField] TileBase _tuileModele; //une tuile
    [SerializeField] GameObject _cleModele; //les modèles des objets tp3
    
    public GameObject cleModele {
        get {
            return _cleModele;
        }
    }
    [SerializeField] GameObject _porteModele;
    [SerializeField] GameObject[] _TjoyauxModele;
    [SerializeField] GameObject _activateurModele;
    [SerializeField] GameObject _bonusModele;
    [SerializeField] GameObject _effectorModele;
    [SerializeField] GameObject _joueur; //le gameobject du joueur pour l'instancier tp3
    [SerializeField] Activateur _activateur; // pour accéder au script de l'activateur(abonner l'événement) tp3
    [SerializeField] int _nbJoyauxParSalle = 20; // le nb de gems par salle tp3
    [SerializeField] int _nbBonusParSalle = 2; // le nombre de sauts plus haut; tp3
    List<Vector2Int> _lesPosSurReperes = new List<Vector2Int>(); //liste pos reperes pour pas avoir autres sur même case tp3
    List<Vector2Int> _lesPosLibres = new List<Vector2Int>(); //les pos libres tp3
    private Salle _salle; 
    Vector2Int _posOpposePorte; //position de porte tp3
    Vector2Int _placementActivateur; //pos activateur tp3
    Vector2Int _placementEffector;
    static Niveau _instance; //singleton du niveau tp3
    static public Niveau instance => _instance;
    [Header("Textes")]
    [SerializeField] TextMeshProUGUI _champArgent; //l'argent qui est montré à l'usager tp3
    [SerializeField] public TextMeshProUGUI _champNiveau;
    [SerializeField] public TextMeshProUGUI _champGemsX2; //texte gemsX2 tp4
    [SerializeField] public TextMeshProUGUI _champVitesseX2; //texte vitesseX2 tp4
    [SerializeField] public TextMeshProUGUI _champVies;
    [SerializeField] TextMeshProUGUI _champCompteur; //texte compteur tp4
    [Header("Donnees")]
    [SerializeField] SOPerso _donneesPerso; //les donnes du perso tp3
    public SOPerso donneesPerso 
    { 
        get { return _donneesPerso; } 
    }
    [SerializeField] SOSauvegarde _donneesSauvegarde;
    [SerializeField] SONavigation _changeScene;
    private bool _aLaCle = false; //bool pour savoir si a la clé tp3
    float _temps; //le temps compteur tp4
    [SerializeField] Transform _transfromConfiner; //le transform du confiner pour la caméra cinemachine tp4
    List<Transform> _waypoints = new List<Transform>(); //liste des waypoints 
    static bool _aBoussole;
    public bool aBoussole
    {
        get { return _aBoussole; }
        set { _aBoussole = value; }
    }
    public List<Transform>waypoints
    {
        get {return _waypoints;}
        set {_waypoints = value; Debug.Log("ajouter waypoint");}
    }

    public bool aLaCle //public avec un get pour être accessible dans autre script tp3
    {
        get { return _aLaCle; }
        set { _aLaCle = value; }
    }
    private int _nbSautBonus = 0;
    public int nbSautBonus //public avec un get pour être accessible dans autre script tp3
    {
        get { return _nbSautBonus; }
        set { _nbSautBonus = Mathf.Clamp(value, 0, 2); } //saut bonus peut pas être plus que 2 ou moins que 0 tp3
    }
    public void AjouterRepere(Transform repere) //ajouter un repere à la liste tp3
    {
        _waypoints.Add(repere);
        Debug.Log(_waypoints.Count);
    }


    //faire arbre gd random hb random si cle = gauche met porte droit exemple
    void Update()
    {
        _temps =_donneesPerso.compteur -= Time.deltaTime; //temps descend tp4
        _champCompteur.text = (int)_temps + " secondes";  //change le temps texte en même temps tp4
        if(_temps <= 0) //si compteur est plus petit que 0 fin de partie
        {
            FinDePartie();
            
        }
    
    }
    /// <summary>
    /// Coroutine qui gère le sons des ennemis #synthese
    /// </summary>
    /// <returns></returns>
    IEnumerator JouerSonEnnemis() 
    {
        while (true) 
        {
            float tempsAttente = Random.Range(7f, 15f); // Temps random attente entre 3 et 10 secondes #synthese
            yield return new WaitForSeconds(tempsAttente); // Attend le nombre de temps #synthese

            float distance = Random.Range(0.5f, 1.5f); // Facteur de distance aléatoire entre 0.5 et 1.5
            _audioSource.pitch = Random.Range(0.8f, 1.2f); // Facteur de hauteur aléatoire entre 0.8 et 1.2


            _audioSource.PlayOneShot(_sons[1], distance); // Joue son de l'ennemi #synthese
        }
    }    
    
    /// <summary>
    /// Lorsqu'on a fini la partie, cette fonction est appelée pour avoir accès au score du joueur tp4
    /// </summary>
    public void FinDePartie()
    {
        Debug.Log("Fin de partie");
        
        _donneesSauvegarde.EcrireFichier(); //fin partie écrit données niveau tp4
        _donneesSauvegarde.LireFichier(); //fin partie lire données niveau tp4
        ArretMusique();
        _changeScene.AllerSceneSuivante();
        
    }
    /// <summary>
    /// Arret de musique lorsqu'on clique sur le bouton tp4
    /// </summary>
    public void ArretMusique()
    {
        GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueEvenA, false); //arrete la musique tp4
        GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueEvenB, false); //arrete la musique tp4
        
    }
    public void ArretMusiquePrincipale(){
        GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueBase, false); //arrete la musique tp4
        GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueEvenA, false); //arrete la musique tp4
        GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueEvenB, false); //arrete la musique tp4
    }
    /// <summary>
    /// Lorsqu'on clique sur le bouton pour aller au menu
    /// </summary>
    public void AllerMenu()
    {
        _donneesPerso.Initialiser(); //initialise les donnees du joueur tp4
    }
    /// <summary>
    /// #synthese
    /// Joue un son dans le tableau de sons
    /// </summary>
    /// <param name="index"></param>
    public void JouerSon(int index)
    {
        _audioSource.clip = _sons[index];
        _audioSource.Play();
    }

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();//va chercher la composante du niveau #synthese
        StartCoroutine(JouerSonEnnemis());//appel coroutine jouer sons ennemis #synthese
        GestAudio.instance.ChangerEtatLecturePiste(TypePiste.musiqueBase, true); //arretela musique tp4
        _champGemsX2.text = "Joyaux X2";
        _champGemsX2.alpha = 0.5f;
        _champVitesseX2.text = "Vitesse X2";
        _champVitesseX2.alpha = 0.5f;
        _champCompteur.text = _temps + " secondes";
        Debug.Log(donneesPerso.nbVies + "wy6y");
        Debug.Log(_donneesPerso.niveau + "");
        _taille = new Vector2Int(_donneesPerso.niveau + 2, 3);
        Debug.Log(_taille + "ssnadaih");
        _champArgent.text = _donneesPerso.argent + ""; //les champs texte sont égales aux données du joueur tp4
        _champNiveau.text = "Niveau " + _donneesPerso.niveau;
        if (_instance != null) { Destroy(gameObject); return; } //s'assurer qu'y a pas 2 Niveau tp3
        _instance = this;
        _activateur.evenementMiseAJour.AddListener(ActiverBonus); //abonner ActiverBonus tp3

        Vector2Int tailleSalle = Salle.taille; //va get la taille

        Vector2Int placementCle; //pos clé
        
        placementCle = Tirage(); //appel la fonction tirage et retourne l'emplacement de la clé en vecteur tp4

        _placementActivateur = new Vector2Int(Random.Range(0, _taille.x), Random.Range(0, _taille.y)); //donne une position random dans une des salles et si y a pas de porte ou clé ou effector place activateur tp3
        while (_placementActivateur == placementCle || _placementActivateur == _posOpposePorte || _placementActivateur == _placementEffector) //tant que y en a un dans même salle refait le tirage tp3
        {
            _placementActivateur = new Vector2Int(Random.Range(0, _taille.x), Random.Range(0, _taille.y));
        }

        _placementEffector = new Vector2Int(Random.Range(0, _taille.x), Random.Range(0, _taille.y)); //donne une position random dans une des salles et si y a pas de porte ou clé ou activateur place effector tp3
        while (_placementEffector == placementCle || _placementEffector == _posOpposePorte || _placementActivateur == _placementEffector) //tant que y en a un dans même salle refait le tirage tp3
        {
            _placementEffector = new Vector2Int(Random.Range(0, _taille.x), Random.Range(0, _taille.y));
        }
        Debug.Log(_posOpposePorte + "sz");
        Debug.Log(placementCle + "ss");
        //boucle qui fait 3 rangés par 3 colonnes
        for (int y = 0; y < _taille.y; y++)
        {
            for (int x = 0; x < _taille.x; x++)
            {
                Vector2Int placementSalle = new Vector2Int(x, y);
                int salleRandom = Random.Range(0, _tSallesModeles.Length); //salle au hasard dans le array
                Vector2 pos = new Vector2(tailleSalle.x * x, -y * tailleSalle.y); //la position d'une salle qui augmente 0*18 1*18 2*18
                GameObject uneSalleRandom = Instantiate(_tSallesModeles[salleRandom], pos, Quaternion.identity, transform); //Instancie la salle à la position
                _salle = uneSalleRandom.GetComponent<Salle>(); //pour parler à la salle qu'on a instancié
                _salle.name = "Salle_" + x + "_" + y;
                _salle.Tester();
                //si la position de l'objet est égal à la position de la salle appel placersurrepere - le decalage de la tilemap et ajoute la position à posReperes tp3
                if (placementCle == placementSalle)
                {
                    Vector2Int decalage = Vector2Int.CeilToInt(_tilemap.transform.position);
                    Vector2Int posRep = _salle.PlacerCleSurRepere(_cleModele) - decalage;
                    _lesPosSurReperes.Add(posRep);


                }
                else if (_posOpposePorte == placementSalle)
                {
                    PlacerPorte(tailleSalle, placementCle);

                }
                else if (_placementActivateur == placementSalle)
                {
                    PlacerActivateur();
                }
                else if (_placementEffector == placementSalle)
                {
                    PlacerEffector();
                }
            }
        }

        // Définition de la position de la pièce du joueur tp3
        Vector2Int posPieceJoueur;

        // Si _posOpposePorte est sur le côté gauche tp3
        if (_posOpposePorte.x < _taille.x / 2)
        {
            posPieceJoueur = _posOpposePorte + Vector2Int.right; // la pièce  est à droite tp3
        }
        // Si _posOpposePorte est sur le côté droit tp3
        else if (_posOpposePorte.x > _taille.x / 2)
        {
            posPieceJoueur = _posOpposePorte + Vector2Int.left; // la pièce est à gauche tp3
        }
        // Si _posOpposePorte est sur le côté inférieur tp3
        else if (_posOpposePorte.y < _taille.y / 2)
        {
            posPieceJoueur = _posOpposePorte + Vector2Int.up; // la pièce  est en haut tp3
        }
        // Si _posOpposePorte est sur le côté supérieur tp3
        else
        {
            posPieceJoueur = _posOpposePorte + Vector2Int.down; // la pièce  est en bas tp3
        }

        // Définition de la position du joueur en fonction de la pièce opposee tp3
        Vector2 posJoueur = new Vector2(posPieceJoueur.x * tailleSalle.x, -posPieceJoueur.y * tailleSalle.y);

        // Instanciation du joueur dans la nouvelle pièce opposee tp3
        Instantiate(_joueur, posJoueur, Quaternion.identity);


        Vector2Int tailleNiveau = new Vector2Int(_taille.x, _taille.y) * tailleSalle; //taille nombre niveaux * taille d'une salle


        Vector2Int min = new Vector2Int(0, 0) - Salle.taille / 2; //le debut du niveau
        Vector2Int max = min + tailleNiveau; // fin du niveau
        Debug.Log(min);
        Debug.Log(max);
        for (int y = min.y; y <= max.y; y++)
        {
            for (int x = min.x; x <= max.x; x++)
            {
                Vector3Int pos = new Vector3Int(x, -y, -2);

                if (x == min.x || x == max.x) tilemap.SetTile(pos, _tuileModele); //lorsqu'on arrive à la fin sur x ou y ou au debut, met une tuile pour refermer les salles
                if (y == min.y || y == max.y) tilemap.SetTile(pos, _tuileModele);

            }

        }
        int tailleX = tailleNiveau.x; //taille en y et x du confiner du niveau tp4
        int tailleY = tailleNiveau.y;
        _transfromConfiner.localScale = new Vector2(tailleX + 1, tailleY + 1); //change le scale du gameobject qui a le polygon collider tp4

        TrouverPosLibre();
        PlacerLesJoyaux();
    }
    /// <summary>
    /// Tirage gauche droite ou haut bas
    /// Si c'est gauche ou droite refais tirage
    /// Donner ex: gauche mettre la position à 0 en x et random en y
    /// Pour la porte on inverse x pour que la position soit à droite et random en y
    /// Même chose pour haut bas
    /// tp4
    /// </summary>
    /// <returns>Un vector de l'emplacement de quel salle il est</returns>
    private Vector2Int Tirage()
    {
        Vector2Int placementCle;
        int tirageGD_ou_HB = Random.Range(0, 2);
        if (tirageGD_ou_HB == 0) // Gauche droite
        {
            int tirageGauche_ou_Droite = Random.Range(0, 2);
            if (tirageGauche_ou_Droite == 0)
            { //place gauche 
                placementCle = new Vector2Int(0, Random.Range(0, _taille.y));
                _posOpposePorte = new Vector2Int(2, Random.Range(0, _taille.y));
            }
            else
            {
                placementCle = new Vector2Int(2, Random.Range(0, _taille.y));
                _posOpposePorte = new Vector2Int(0, Random.Range(0, _taille.y));
            }
        }
        else
        {//Haut Bas
            int tirageHaut_ou_Bas = Random.Range(0, 2);
            if (tirageHaut_ou_Bas == 0)
            { //place haut
                placementCle = new Vector2Int(Random.Range(0, _taille.x), 0);
                _posOpposePorte = new Vector2Int(Random.Range(0, _taille.x), 2);
            }
            else
            {
                placementCle = new Vector2Int(Random.Range(0, _taille.x), 2);
                _posOpposePorte = new Vector2Int(Random.Range(0, _taille.x), 0);
            }

        }
        Debug.Log(placementCle + "cle1");
        return placementCle;
    }

    public void AppelerEvent() //appel de l'événement tp3
    {
        Debug.Log("test");
        _activateur.evenementMiseAJour.Invoke();
    }
    /// <summary>
    /// si la position de l'objet est égal à la position de la salle appel placersurrepere - le decalage de la tilemap et ajoute la position à posReperes tp3
    /// </summary>
    public void PlacerActivateur()
    {
        Vector2Int decalage = Vector2Int.CeilToInt(_tilemap.transform.position);
        Vector2Int posRepActivateur = _salle.PlacerActivateurSurRepere(_activateurModele) - decalage;
        _lesPosSurReperes.Add(posRepActivateur);
    }
    /// <summary>
    /// si la position de l'objet est égal à la position de la salle appel placersurrepere - le decalage de la tilemap et ajoute la position à posReperes tp3
    /// </summary>
    public void PlacerEffector()
    {
        Vector2Int decalage = Vector2Int.CeilToInt(_tilemap.transform.position);
        Vector2Int posRepEffector = _salle.PlacerActivateurSurRepere(_effectorModele) - decalage;
        _lesPosSurReperes.Add(posRepEffector);
    }
    /// <summary>
    /// Sert à activer les bonus sur la scène
    /// </summary>
    public void ActiverBonus()
    {
        
        Transform conteneur = new GameObject("Bonus").transform; //mettre les bonus dans un conteneur enfant de niveau tp3
        conteneur.parent = transform;
        int nbBonus = _nbBonusParSalle * (_taille.x + _taille.y); //ex 2 * 9 salles tp3
        //Va checker si la pos est libre et instancie le bonus tileanchor pour milieu de position tp3
        for(int i = 0; i < nbBonus; i++)
        {
            Vector2Int pos = ObtenirPosLibre();

            Vector3 pos3 = (Vector3)(Vector2)pos + _tilemap.transform.position + _tilemap.tileAnchor;
            Instantiate(_bonusModele, pos3, Quaternion.identity, conteneur);

            if(_lesPosLibres.Count == 0){ //si 0 arrête tp3
                Debug.LogWarning("Fini"); break;
            }
        }
    }

    /// <summary>
    /// si la position de l'objet est égal à la position de la salle appel placersurrepere - le decalage de la tilemap et ajoute la position à posReperes tp3
    /// </summary>
    /// <param name="tailleSalle">Taille de salle</param>
    /// <param name="placementCle">La position de la cle</param>
    public void PlacerPorte(Vector2Int tailleSalle, Vector2Int placementCle)
    {
        int sallePorte = Random.Range(0, _tSallesModeles.Length);
        Vector2 posPorte = new Vector2(tailleSalle.x * _posOpposePorte.x, -_posOpposePorte.y * tailleSalle.y); 
        Debug.Log(posPorte + "sxxxx");
        Vector2Int decalage = Vector2Int.CeilToInt(_tilemap.transform.position);
        Vector2Int posRepPorte = _salle.PlacerPorteSurRepere(_porteModele) - decalage;
        
        _lesPosSurReperes.Add(posRepPorte);
        
       

    }
    
    /// <summary>
    /// trouver les tuiles libres dans la tilemap du niveau grâce à cellBounds
    /// boucle qui part de min à max en y et dedans fait les x
    /// pos de tuile augmente 
    /// va chercher la tuile à la position
    /// si y a pas de tuile ajoute à posLibres
    /// TP3
    /// </summary>
    public void TrouverPosLibre()
    {
        BoundsInt bornes = _tilemap.cellBounds;
        for(int y = bornes.yMin; y < bornes.yMax; y++)
        {
            for(int x = bornes.xMin; x < bornes.xMax; x++)
            {
                Vector2Int posTuile = new Vector2Int(x,y);
                TileBase tuile = _tilemap.GetTile((Vector3Int)posTuile);
                Vector3 posTilemap = _tilemap.transform.position;
                if(tuile == null) _lesPosLibres.Add(posTuile);
                
            }
        }
        foreach(Vector2Int pos in _lesPosSurReperes) //enlève des posLibres les pos sur reperes tp3
        {
            _lesPosLibres.Remove(pos);
        }
        Debug.Log(_lesPosLibres.Count);
    }
    /// <summary>
    /// ajoute joyaux conteneur enfant de niveau
    /// pour chaque joyaux dans niveau fait appel obtenirPos
    /// Ensuite place le sur pos
    /// TP3
    /// </summary>
    public void PlacerLesJoyaux()
    {
        Transform conteneur = new GameObject("Joyaux").transform;
        conteneur.parent = transform;
        int nbJoyaux = _nbJoyauxParSalle * (_taille.x + _taille.y);
        for(int i = 0; i < nbJoyaux; i++)
        {
            Vector2Int pos = ObtenirPosLibre();

            Vector3 pos3 = (Vector3)(Vector2)pos + _tilemap.transform.position + _tilemap.tileAnchor;
            int nbRandom = Random.Range(0, _TjoyauxModele.Length);
            Instantiate(_TjoyauxModele[nbRandom], pos3, Quaternion.identity, conteneur);

            if(_lesPosLibres.Count == 0){
                Debug.LogWarning("Fini"); break;
            }
        }
    }
    /// <summary>
    /// trouve tuile random 
    /// et l'enlève de posLibres
    /// TP3
    /// </summary>
    /// <returns></returns>
    private Vector2Int ObtenirPosLibre()
    {
        int indexPosLibre = Random.Range(0, _lesPosLibres.Count);
        Vector2Int pos = _lesPosLibres[indexPosLibre];
        _lesPosLibres.RemoveAt(indexPosLibre);
        return pos;
    }

    /// <summary>
    /// reçoit une tuile et la place sur sa tilemap
    /// </summary>
    /// <param name="tm"></param>
    /// <param name="_doitRester"></param>
    /// <param name="niveau"></param>
    /// <param name="pos"></param>
    /// <param name="decalage"></param>
    public void TraiterTuile(Tilemap tm,bool _doitRester, Niveau niveau, Vector3Int pos, Vector3Int decalage)
    {
        TileBase tile = tm.GetTile(pos); //la position de la tuile sur le tilemap
        if (tile != null)
            {
                niveau.tilemap.SetTile(pos + decalage, tile); //si y en a unemet une tuile à la position qui était sur la tilemap mais sur la tilemap du niveau
            }
            
    }
    /// <summary>
    /// Reçoit le nbGems et envoie le nombre de gems après un niveau aux données tp3
    /// </summary>
    /// <param name="nbGems"></param>
    public void AjouterGems(int nbGems)
    {
        _donneesPerso.argent += nbGems;
        _champArgent.text = _donneesPerso.argent + "";
    }
    void OnDestroy() //si y est plus là enlève les listeners
    {
        Perso.instance.Initialiser();
    }
    void OnApplicationQuit()
    {
        _donneesPerso.Initialiser();
    }
}

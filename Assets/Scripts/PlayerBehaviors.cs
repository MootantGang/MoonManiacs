using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerBehaviors : NetworkBehaviour, IEffectHandler
{
    public GameObject myLunarModule;
    public GameObject ownedInventoryBox;

    [SyncVar]
    public bool boxInPossession = false;

    /// <summary>
    /// PROVISIONAL HASTA SABER DONDE PONERLO
    /// </summary>
    public float attractionSpeed = 15f;
    /// <summary>
    /// Objeto dentro de area de deteccion del player al que se esta mirando para interactuar
    /// </summary>
    public Transform interactionItem = null;
    private Ray cameraRay;
    private RaycastHit rayHit;
    [SerializeField]
    private float INTERACT_DISTANCE = 2f;
    [SerializeField]
    private float DISTANCE_WEIGHT = 0.6f;
    [SerializeField]
    private float ANGLE_WEIGHT = 0.4f;
    [SerializeField]
    private float INTERACTION_ANGLE = 30f;
    private float INTERACT_ANGLE;
    private bool preventInteraction = false;
    public Material enabledMat;
    public Material disabledMat;
    [SerializeField]
    private int resourcesCapacity = 10;

    [SerializeField]
    [SyncVar]
    private int currentResources = 0;

    public int currentMines = 0;
    public GameObject blackHoleMinePrefab;
    public GameObject myGun;
    public GameObject newBox;
    bool hasBox = false;
    public GameObject boxPosition;
    private Animator playerAnimator;
    [SyncVar]
    public float maxEnergy = 100f;
    [SyncVar]
    private float energy = 100f;
    [SerializeField]
    private float energyPerShoot = 20f;
    [SerializeField]
    private float energyRegenPerSecond = 10f;
    [SerializeField]
    private float shootCooldown = 0.3f;
    [SyncVar]
    private float nextShootTime = 0f;
    public bool menuOpened = false;

    private InputTest playerInput;

    public HUDManager hudManagerScript;
    [SerializeField]
    private GameObject myPistol;
    public GameObject bulletPrefab;
    public float shootSpeed = 150f;

    void Start()
    {
        INTERACT_ANGLE = Mathf.Cos(Mathf.Rad2Deg*INTERACTION_ANGLE);

        playerAnimator = GetComponent<Animator>();
        transform.position = NetworkManager.singleton.startPositions[GetComponent<PlayerNetworkControl>().index].position;       

        CmdCreateBox();

        FindLunarModule(GetComponent<PlayerNetworkControl>().index);
        playerInput = GetComponent<InputTest>();

        if (isLocalPlayer) {
            GameManager.instance.myPlayer = gameObject;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    [Command]
    public void CmdCreateBox()
    {
        if (hasBox)
            return;
        hasBox = true;
        
        Vector3 aux = transform.position + transform.forward * 2;
        GameObject newInstantiatedBox = Instantiate(newBox, aux, transform.rotation);
        
        GetComponent<PlayerBehaviors>().ownedInventoryBox = newInstantiatedBox;
        
        NetworkServer.Spawn(GetComponent<PlayerBehaviors>().ownedInventoryBox);
        GameManager.instance.CmdChangeBoxOwner(newInstantiatedBox, gameObject);
    }

    void Update()
    {
        if (isLocalPlayer) {
            hudManagerScript.SetCurrentAlumoonite(currentResources);
            DetectionArea();
 
            if(CanInteract()) {
                if (InputManager.InteractionButton()) {
                    if (boxInPossession) {
                        CmdDropInventoryBox();
                    } else {
                        if (interactionItem != null) {
                            if (interactionItem.CompareTag("InventoryBox")) {
                                CmdPickInventoryBox(interactionItem.gameObject);
                            } else {
                                // interaccion con el objeto en cuestion
                                if (interactionItem.GetComponent<LunarModule>() && interactionItem.gameObject == myLunarModule) {
                                    interactionItem.GetComponent<LunarModule>().Interaction();
                                }
                                if (interactionItem.CompareTag("MineralVein")) { // veta de mineral
                                    CmdMineMineral(interactionItem.gameObject);
                                }
                                if (interactionItem.CompareTag("MineralFragment")) { // mineral
                                    if (currentResources < 10) {
                                        CmdPickAlomoonite(interactionItem.gameObject);
                                    }
                                }
                            }
                        }
                    }
                }
                if (InputManager.ShootGunButton() && !boxInPossession)
                {
                    ShootLaser();
                }
                if (currentMines > 0 && InputManager.TrapButton()) {
                    PlantBlackHoleMine();
                }
            }
        }
        
        playerInput.playerAnimator.SetBool("Shooting", Time.time < nextShootTime);
        playerInput.playerAnimator.SetBool("Carrying Box", boxInPossession);

        energy += Mathf.Clamp(Time.deltaTime * energyRegenPerSecond, 0, maxEnergy);
    }

    public bool CanInteract() {
        return !menuOpened && playerInput.inputEnabled;
    }

    public void OnEffectEnter(IEffectSource source) {
        if (source.owner != gameObject) {// La idea es saber si soy o no el owner para que no me afecte mi propia mina o efecto
            switch (source.type) {
                case EffectType.MOO_MINE:
                    if (boxInPossession) {
                        // tirar aloomonite
                    } else {
                        // incapacitar para actuar
                    }
                    break;
                case EffectType.BLACK_HOLE:
                    //if (source.owner == gameObject) { // La idea es saber si soy o no el owner para que no me afecte mi propia mina
                    //Debug.Log("En blackhole");
                        if (boxInPossession) {
                            CmdDropInventoryBox();
                        }
                        //preventInteraction = true;
                        transform.position = Vector3.MoveTowards(transform.position, source.GetGameObject().transform.position, attractionSpeed * Time.deltaTime);
                    //}
                    break;
                case EffectType.STUN:
                    // animacion de stun
                    // bloquear movimiento
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// CREO QUE NUNCA SE TRIGGEREA PORQUE SE DESTRUYEN LOS COLLIDERS Y OBJETOS DE MINA, ETC ANTES DE DETECTAR EL EXIT
    /// </summary>
    /// <param name="source"></param>
    public void OnEffectExit(IEffectSource source) {
        if (source.owner != gameObject) {
            switch (source.type) {
                case EffectType.MOO_MINE:
                    if (boxInPossession) {
                        // tirar aloomonite 2 segundos mas
                    } else {
                        // incapacitar para actuar 2 segundos mas
                    }
                    break;
                case EffectType.BLACK_HOLE:

                    break;
                case EffectType.STUN:
                    // ¿nada?
                    break;
                default:
                    break;
            }
        }
    }    
    
    public void OnEffectStay(IEffectSource source) {
        if (source.owner != gameObject) {
            switch (source.type) {
                case EffectType.MOO_MINE:
                    if (boxInPossession) {
                        // tirar aloomonite
                    } else {
                        // incapacitar para actuar
                    }
                    break;
                case EffectType.BLACK_HOLE:
                    //if (source.owner == gameObject) { // La idea es saber si soy o no el owner para que no me afecte mi propia mina
                        if (boxInPossession) {
                            CmdDropInventoryBox();
                        }
                        //preventInteraction = true;
                        transform.position = Vector3.MoveTowards(transform.position, source.GetGameObject().transform.position, attractionSpeed * Time.deltaTime);
                    //}
                    break;
                case EffectType.STUN:
                    // ¿nada?
                    break;
                default:
                    break;
            }
        }

    }
    
    /// <summary>
    /// Metodo que recoge la caja del jugador y se la coloca en la cabeza.
    /// Relacion padre-hijo
    /// </summary>
    public void PickInventoryBox(GameObject interactionItem) {
        // animacion coger caja
        if (ownedInventoryBox != null && ownedInventoryBox.GetComponent<Box>().owner == gameObject) {
            GameManager.instance.CmdChangeBoxOwner(ownedInventoryBox, null);
        }
        ownedInventoryBox = interactionItem.gameObject;
        boxInPossession = true;
        GameManager.instance.CmdChangeBoxOwner(ownedInventoryBox, gameObject);
        ownedInventoryBox.layer = LayerMask.NameToLayer("DisabledInteractable");

        ownedInventoryBox.GetComponent<Box>().carryingPlayer = gameObject;
        RpcChangeCarrierPlayer(ownedInventoryBox, gameObject);
    }

    [Command]
    public void CmdPickInventoryBox(GameObject interactionItem)
    {
        if (!boxInPossession)
            PickInventoryBox(interactionItem);
    }

    [ClientRpc]
    public void RpcChangeCarrierPlayer(GameObject interactionItem, GameObject player)
    {
        if (player == null)
        {
            interactionItem.GetComponent<Rigidbody>().isKinematic = false;
            interactionItem.layer = LayerMask.NameToLayer("Interactable");
        }

        else
        {
            interactionItem.GetComponent<Rigidbody>().isKinematic = true;
            interactionItem.layer = LayerMask.NameToLayer("DisabledInteractable");
        }
    }

    /// <summary>
    /// Metodo que suelta la caja del jugador en el suelo delante de el.
    /// Relacion padre-hijo
    /// </summary>
    public void DropInventoryBox() {
        ownedInventoryBox.transform.parent = null;
        GameManager.instance.CmdChangeBoxOwner(ownedInventoryBox, gameObject);
        ownedInventoryBox.layer = LayerMask.NameToLayer("Interactable");

        boxInPossession = false;

        ownedInventoryBox.GetComponent<Box>().carryingPlayer = null;
        RpcChangeCarrierPlayer(ownedInventoryBox, null);
    }

    [Command]
    public void CmdDropInventoryBox()
    {
        DropInventoryBox();
    }

    /// <summary>
    /// Metodo para llamar cuando el player entra en rango de su caja(?)
    /// </summary>
    /// <returns></returns>
    public int TransferResourcesToInventoryBox() {
        int aux = currentResources;
        currentResources = 0;
        return aux;
    }

    /// <summary>
    /// Metodo que genera un area esferica de X diametro y detecta todos los objetos dentro de ella que tengan la layer 9 - Interactable
    /// Comprobará los items que esten en un rango [-30º, 30º] con respecto al forward del personaje para determinar el objeto
    /// a ser interactuado
    /// </summary>
    private void DetectionArea() {
        Collider[] interactablesInRange = Physics.OverlapSphere(transform.position, INTERACT_DISTANCE, 1<<9 | 1<<10);
   
        INTERACT_ANGLE = Mathf.Cos(Mathf.Rad2Deg*INTERACTION_ANGLE);
        Vector3 direction = new Vector3();
        Vector3 distanceVector;
        float distance;
        float maxDistance = INTERACT_DISTANCE * INTERACT_DISTANCE;
        float previousDistance = maxDistance;
        float elegibility;
        float previousElegibility = 0f;
        if (interactionItem != null) {          
            RestoreStandardMaterial();
        }
        interactionItem = null;
        for (int i = 0; i < interactablesInRange.Length; ++i) {
            if (interactablesInRange[i].gameObject.layer == 9 || interactablesInRange[i].gameObject.layer == 10) {
                distanceVector = interactablesInRange[i].transform.position - transform.position;
                direction = distanceVector.normalized;
                float angle = Vector3.Dot(transform.forward, direction);
                if (-angle < INTERACT_ANGLE) {
                    distance = distanceVector.sqrMagnitude;
                    elegibility = ((maxDistance - distance) / maxDistance) * DISTANCE_WEIGHT + ((1f - Mathf.Abs(angle)) / (1 - INTERACT_ANGLE)) * ANGLE_WEIGHT;
                    if (elegibility > previousElegibility) {
                        previousElegibility = elegibility;
                        interactionItem = interactablesInRange[i].gameObject.transform;
                    }
                }
            }
        }
        if (interactionItem != null) {
            OutlineItem(interactionItem);
        }
    }

    private void PlantBlackHoleMine() {
        Vector3 aux = transform.position + transform.forward * 2;
        GameObject mineInstance = Instantiate(blackHoleMinePrefab, aux, Quaternion.identity);
        currentMines--;
        mineInstance.GetComponent<BlackHoleActivation>().owner = this.gameObject;
    }

    [Command]
    public void CmdPickAlomoonite(GameObject mineral)
    {
        MineralFragment mineralFragment = mineral.GetComponent<MineralFragment>();
        if (mineralFragment != null && mineralFragment.Activated(MineralFragment.collectorSource.PLAYER))
        {
            currentResources = Mathf.Clamp((currentResources + 1), 0, resourcesCapacity);
            NetworkServer.Destroy(mineral);
        }
    }

    public int TransferResourcesToBox() {
        int aux = currentResources;
        currentResources = 0;
        return aux;
    }

    private void OutlineItem(Transform interactionItem){
        if (interactionItem.CompareTag("LunarModule")) {
            if (interactionItem.gameObject == myLunarModule) {
                interactionItem.GetComponent<LunarModule>().ChangeInteractionStatus(true);
            }
        } else {
            if (interactionItem.GetComponent<Outline>() != null || interactionItem.GetComponentInChildren<Outline>() != null) {
                if (interactionItem.GetComponent<Outline>() != null) {
                    interactionItem.GetComponent<Outline>().OutlineItem();
                } else {
                    interactionItem.GetComponentInChildren<Outline>().OutlineItem();
                }
            }
        }
    }

    private void RestoreStandardMaterial() {
        if (interactionItem.CompareTag("LunarModule")) {
            if (interactionItem.gameObject == myLunarModule) {
                interactionItem.GetComponent<LunarModule>().ChangeInteractionStatus(false);
            }
        } else {
            if (interactionItem.GetComponent<Outline>() != null || interactionItem.GetComponentInChildren<Outline>() != null) {
                if (interactionItem.GetComponent<Outline>() != null) {
                    interactionItem.GetComponent<Outline>().RestoreStandardMaterial();
                } else {
                    interactionItem.GetComponentInChildren<Outline>().RestoreStandardMaterial();
                }
            }
        }
    }

    private void FindLunarModule(int index){
        switch (index)
        {
            case 0:
                myLunarModule = GameObject.Find("LunarModule_Red");
                break;
            case 1:
                myLunarModule = GameObject.Find("LunarModule_Green");
                break;
            case 2:
                myLunarModule = GameObject.Find("LunarModule_Blue");
                break;
            case 3:
                myLunarModule = GameObject.Find("LunarModule_Yellow");
                break;
            case 4:
                myLunarModule = GameObject.Find("LunarModule_Teal");
                break;
            case 5:
                myLunarModule = GameObject.Find("LunarModule_Pink");
                break;
            default:
                break;
        }
        myLunarModule.GetComponent<LunarModule>().myPlayer = gameObject;
    }

    public void ShootLaser() {
        Vector3 direction, location;
        myPistol.GetComponent<Shoot>().GetShootLaserInfo(out direction, out location);

        CmdShootLaser(direction, location);
    }

    [Command]
    public void CmdShootLaser(Vector3 direction, Vector3 origin)
    {
        if (!boxInPossession && energy >= energyPerShoot && Time.time > nextShootTime) {
            nextShootTime = Time.time + shootCooldown;
            energy -= energyPerShoot;
            
            GameObject laserShoot = Instantiate(bulletPrefab, origin, Quaternion.identity);
            laserShoot.GetComponent<Rigidbody>().velocity = direction * shootSpeed;
            Destroy(laserShoot, 5f);
            NetworkServer.Spawn(laserShoot);
        }
    }

    [Command]
    public void CmdUnattendBox()
    {
        ownedInventoryBox.GetComponent<Box>().UnattendedBox();
    }

    [Command]
    public void CmdMineMineral(GameObject mineralVein) {
        List<GameObject> minerals = mineralVein.GetComponent<MineralVein>().DropMineral();
        if (minerals != null && minerals.Count > 0) {
            foreach(GameObject mineral in minerals) {
                NetworkServer.Spawn(mineral);
                GameManager.instance.RpcPlayRandomSound(mineral, MineralFragment.instanceSource.VEIN);
            }
        }
    }

    public bool IsLocalPlayer() {
        return isLocalPlayer;
    }

    public void OnDestroy()
    {
        GameManager.instance.DestroyPlayer(gameObject); 
    }
}

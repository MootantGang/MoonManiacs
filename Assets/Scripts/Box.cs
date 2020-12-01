using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Box : NetworkBehaviour
{
    [SyncVar]
    public int mineralAmount = 0;
    private int lastMineralAmount = -1;
    public int maxMineralAmount = 50;

    [SyncVar]
    public GameObject owner = null;

    public GameObject[] mineralPrefabs; // Objects to be shown randomly when dropping mineral from box
    public float mineralRadius = 15.0f;
    public float attendanceRadius = 25.0f;
    public float playerRadius = 7.5f;
    public float lunarModuleRadius = 15f;

    public float cooldownTime = 2.0f;
    public float currentcooldownTime = 0.0f;
    public bool coolingdown = false;

    public Material boxMaterial;
    public Material unattendedBoxMat;
    public List<Texture2D> texturasIndex;
    private int texInd = 0;

    [SyncVar]
    [HideInInspector]
    public GameObject carryingPlayer = null;

    [SyncVar]
    int materialIndex;

    void Start()
    {
        UpdateMaterialID();
    }

    void Update()
    {
        UpdateCooldown();
        CheckOwnerDistance();
        if (isServer) {
            DetectionArea();
        }
        if (owner != null && owner.GetComponent<PlayerBehaviors>().IsLocalPlayer())
            owner.GetComponent<HUDManager>().SetCurrentAlumooniteBox(mineralAmount);
        if (lastMineralAmount != mineralAmount) {
            lastMineralAmount = mineralAmount;
            UpdateFillingTexture();
        }
        if (carryingPlayer)
        {
            transform.position = carryingPlayer.GetComponent<PlayerBehaviors>().boxPosition.transform.position;
            transform.rotation = carryingPlayer.GetComponent<PlayerBehaviors>().boxPosition.transform.rotation;
        }
    }

    private void DetectionArea() {
        Collider[] mineralsInRange = Physics.OverlapSphere(transform.position, mineralRadius, 1<<10);
        foreach (Collider mineral in mineralsInRange)
        {
            AcquireMineral(mineral.gameObject.GetComponent<MineralFragment>());
        }
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, playerRadius, 1<<11);
        foreach (Collider player in playersInRange)
        {
            if (player.gameObject == owner) {
                mineralAmount += player.GetComponent<PlayerBehaviors>().TransferResourcesToBox();
            }   
        }
        Collider[] interactables = Physics.OverlapSphere(transform.position, lunarModuleRadius, 1 << 9); //Layer interactable
        foreach  (Collider interactable in interactables)
        {
            if (owner != null && interactable.CompareTag("LunarModule") && interactable.gameObject == owner.GetComponent<PlayerBehaviors>().myLunarModule)
            {
                interactable.GetComponent<LunarModule>().AddResources(TransferResources());
            }
        }
    }

    public void AcquireMineral(MineralFragment mineral)
    {
        if (mineral != null && mineral.Activated(MineralFragment.collectorSource.BOX))
        {
            mineralAmount = Mathf.Clamp((mineralAmount + 1), 0, maxMineralAmount);
            NetworkServer.Destroy(mineral.gameObject);
        }
    }

    public GameObject DropMineral()
    {
        if (!coolingdown && mineralAmount > 0)
        {
            GameObject mineralFragment = Instantiate(mineralPrefabs[Random.Range(0, mineralPrefabs.Length)]) as GameObject;
            mineralFragment.transform.position = transform.position + new Vector3(0.0f, 2.0f, 0.0f);

            Rigidbody rb = mineralFragment.GetComponent<Rigidbody>();
            Vector3 trajectory = new Vector3(0.0f, 0.0f, 0.0f);
            trajectory.x = Random.Range(-1.0f, 1.0f);
            trajectory.y = Random.Range(1.0f, 5.0f);
            trajectory.z = Random.Range(-1.0f, 1.0f);
            float force = 5.0f;
 
            rb.AddForce(trajectory * force, ForceMode.Impulse);

            // Cooldown
            coolingdown = true;

            mineralAmount = Mathf.Clamp((mineralAmount - 1), 0, maxMineralAmount);

            return mineralFragment;
        }
        return null;
    }
       
    public void UpdateCooldown()
    {
        if (coolingdown)
        {
            currentcooldownTime += Time.deltaTime;

            if (currentcooldownTime >= cooldownTime)
            {
                currentcooldownTime = 0.0f;
                coolingdown = false;
            }
        }
    }

    public int TransferResources()
    {
        int mineralTransfered = mineralAmount;
        mineralAmount = 0;
        return mineralTransfered;
    }

    public void UnattendedBox()
    {
        // CAMBIAR MATERIAL
        MeshRenderer[] boxChildren = this.gameObject.GetComponentsInChildren<MeshRenderer>();

        Material[] boxMats = boxChildren[0].materials;
        boxMats[0] = unattendedBoxMat;

        boxChildren[0].materials = boxMats;

        owner = null;     
    }

    public void CheckOwnerDistance()
    {
        if (owner != null)
        {
            Vector3 distance2player = owner.transform.position - transform.position;
            float distance = distance2player.sqrMagnitude;

            if(distance > (attendanceRadius * attendanceRadius))
            {
                owner.GetComponent<PlayerBehaviors>().ownedInventoryBox = null;
                GameManager.instance.CmdChangeBoxOwner(gameObject, null);
            }
        }
    }

    public GameObject GetBoxOwner()
    {
        return owner;
    }

    public void UpdateMaterialID()
    {     
        MeshRenderer[] boxChildren = GetComponentsInChildren<MeshRenderer>();

        Material[] boxMats = boxChildren[0].materials;

        if (owner != null)
        {
            PlayerNetworkControl playerNetworkControl = owner.GetComponent<PlayerNetworkControl>();
            materialIndex = playerNetworkControl.index;
            boxMats[0] = playerNetworkControl.BoxMaterials[materialIndex];
            boxChildren[0].materials = boxMats;
        }
        else
        {
            UnattendedBox();
        }      
    }

    private int CheckAmount()
    {
        int textureIndex = 0;
        if (mineralAmount > 7)
            textureIndex += 1;
        if (mineralAmount>21)
            textureIndex += 1;
        if (mineralAmount > 28)
            textureIndex += 1;
        if (mineralAmount > 35)
            textureIndex += 1;
        if (mineralAmount > 42)
            textureIndex += 1;
        if (mineralAmount > 49)
            textureIndex += 1;
        return textureIndex;
    }

    private void UpdateFillingTexture() {
            texInd = CheckAmount();
            boxMaterial.SetTexture("MineralFeedBack", texturasIndex[texInd]);
    }  
}

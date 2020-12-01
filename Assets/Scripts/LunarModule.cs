using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LunarModule : NetworkBehaviour
{
    public GameObject myPlayer;
    [SyncVar]
    public int resourcesNeededToWin = 200;
    [SyncVar]
    public int currentResources = 0;

    private bool winner = false;
    [SerializeField]
    private GameObject menuLunarModule;
    public Image alomooniteBarFilling;
    private bool selectedItem = false;
    [SerializeField]
    private Text mineAmountText;
    public int blackHoleCost = 10;
    [SerializeField]
    private int index;

    [SerializeField]
    private Material lunarModuleStandardMaterial;
    [SerializeField]
    private Material cylinderLightStandardMaterial;
    
    [SerializeField]
    private Material lunarModuleWhiteMaterial;
    [SerializeField]
    private Material cylinderLightWhiteMaterial;
    [SerializeField]
    private MeshRenderer lunarModuleMeshRenderer;
    [SerializeField]
    private MeshRenderer cylinderLightMeshRenderer;

    [SyncVar]
    public float current_fill = 0;

    void Update()
    {
        if (myPlayer != null && myPlayer.GetComponent<PlayerBehaviors>().IsLocalPlayer())
            UpdateAlomooniteBar();
    }

    /// <summary>
    /// Se lanza al presionar la F sobre tu modulo lunar. Abriria el menu de la nave para craftear cosas
    /// </summary>
    public void Interaction() {
        myPlayer.GetComponent<PlayerBehaviors>().menuOpened = true;
        menuLunarModule.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        mineAmountText.text = myPlayer.GetComponent<PlayerBehaviors>().currentMines.ToString() + "/3";
    }

    public void ExitLunarModuleHUD() {
        GameManager.instance.myPlayer.GetComponent<PlayerBehaviors>().menuOpened = false;
        Cursor.visible = false;
        selectedItem = false;
    }

    private void UpdateAlomooniteBar() {
        current_fill = (float) currentResources / (float) resourcesNeededToWin;

        alomooniteBarFilling.fillAmount = Mathf.Clamp(current_fill, 0, resourcesNeededToWin);
    }

    private void AddAlomoonite() {
        currentResources += 4;
        UpdateAlomooniteBar();
    }

    public int getCurrentResources() {
        return currentResources;
    }

    public void SelectItem() {
        selectedItem = true;
    }

    public void Craft() {
        Debug.Log(gameObject.name);
        if (selectedItem && currentResources >= blackHoleCost && myPlayer.GetComponent<PlayerBehaviors>().currentMines < 3) {
            currentResources -= blackHoleCost;
            myPlayer.GetComponent<PlayerBehaviors>().currentMines++;
        }
        mineAmountText.text = myPlayer.GetComponent<PlayerBehaviors>().currentMines.ToString() + "/3";
    }

    public void AssignPlayer()
    {
        foreach (GameObject player in GameManager.instance.players)
        {
            if (player.GetComponent<PlayerNetworkControl>().index == index)
            {
                myPlayer = player;
                myPlayer.GetComponent<PlayerBehaviors>().myLunarModule = gameObject;
            }
        }
    }

    public void ChangeInteractionStatus(bool status) {
        if (status) {
            lunarModuleMeshRenderer.material = lunarModuleWhiteMaterial;
            cylinderLightMeshRenderer.material = cylinderLightWhiteMaterial;
        } else {
            lunarModuleMeshRenderer.material = lunarModuleStandardMaterial;
            cylinderLightMeshRenderer.material = cylinderLightStandardMaterial;
        }
    }

    public void AddResources(int resources)
    {
        if (resources == 0) {
            return;
        }
        currentResources += resources;
        currentResources = Mathf.Clamp(currentResources, 0, resourcesNeededToWin);
    }
}

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerNetworkControl : NetworkBehaviour
{
    //This script assigns an index and a custom color to each new connected player
    //It also Instantiates the "StartButton" only for the host

    public SkinnedMeshRenderer characterMesh;
    public MeshRenderer boxMesh;
    public MeshRenderer gunMesh;

    [SyncVar]
    public int index;

    [SyncVar]
    public Color m_color; //character color

    [SyncVar]
    public Color e_color; //character emissive color

    [SyncVar]
    public Color b_color; //box color

    [SyncVar]
    public Color g_color; //gun color

    [SyncVar]
    public Color ge_color; //gun emissive color

    [SyncVar]
    public string myName = "";

    public string[] playerNames = new string[6] {"Red Player", "Green Player", "Blue Player", "Yellow Player", "Teal Player", "Pink Player"};

    public GameObject startButton;
    GameObject StartButtonInstantiated;

    public Material[] SuitMaterials = new Material[6];
    public Material[] BoxMaterials = new Material[6];

    void Start()
    {      
        characterMesh.material.SetColor("Color_980F13A0", m_color);
        characterMesh.material.SetColor("Color_196D0FD3", e_color);
        gunMesh.material.SetColor("Color_5B742903", g_color);
        gunMesh.material.SetColor("Color_B6412E8C", ge_color);
        gameObject.name = myName;
        CmdSetMeshColors(m_color, e_color, b_color, g_color, ge_color, index, myName);     
    }

    void Update()
    {
        if (isServer)
        {         
            if (NetworkServer.connections.Count >= 2 && GameManager.instance.StartButtonIsInstantiated == false)
            {
                if (GameObject.Find("StartButton(Clone)") == null)
                    StartButtonInstantiated = Instantiate(startButton, GameObject.Find("UICanvas").transform);
                GameManager.instance.StartButtonIsInstantiated = true;
            }

            if (GameManager.instance.gameHasStarted == false && GameManager.instance.StartButtonIsInstantiated == true)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    startButton.GetComponent<LobbyButtons>().StartGame();
                    if (StartButtonInstantiated != null)
                        StartButtonInstantiated.SetActive(false);                 
                }
            }
        }
    }

    public override void OnStartClient()
    {
        if (isServer)
        {     
            index = NetworkServer.connections.Count - 1;
            for (int i = 0; i < NetworkServer.connections.Count; i++)
            {
                if (index == i)
                {
                    m_color = SuitMaterials[i].GetColor("Color_980F13A0");
                    e_color = SuitMaterials[i].GetColor("Color_196D0FD3");
                    b_color = BoxMaterials[i].GetColor("Color_80348FC8");
                    g_color = SuitMaterials[i].GetColor("Color_980F13A0");
                    ge_color = SuitMaterials[i].GetColor("Color_196D0FD3");
                    myName = playerNames[i];
                }
            }
            RpcSetColor(m_color, e_color, b_color, g_color, ge_color, index, myName);         
        }
    }

    [ClientRpc]
    void RpcSetColor(Color m, Color e, Color b, Color g, Color ge, int i, string name)
    {
        characterMesh.material.SetColor("Color_980F13A0", m);
        characterMesh.material.SetColor("Color_196D0FD3", e);
        gunMesh.material.SetColor("Color_5B742903", g);
        gunMesh.material.SetColor("Color_B6412E8C", ge);
        gameObject.name = name;
        index = i;
    }

    [Command]
    public void CmdSetMeshColors(Color m, Color e, Color b, Color g, Color ge, int i, string name)
    {       
        m_color = m;
        e_color = e;
        b_color = b;
        g_color = g;
        ge_color = ge;
        myName = name;
        index = i;  
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    /// <summary> Instancia del GameManager para acceder desde cualquier parte del juego </summary>
    public static GameManager instance = null;
    [SyncVar]
    public float gameDuration = 600f;

    public Text timerText;

    public bool lastMinute = false;

    public GameObject alarmHUD;
    [SerializeField]
    private GameObject[] lunarModules;
    [HideInInspector]
    public GameObject[] players;
    private bool finishedGame = false;
    public AudioClip alarmSound;
    public AudioClip lastMinuteOST;
    public AudioSource audioSource_OST;
    public GameObject boxPrefab;

    [SyncVar]
    public bool gameHasStarted = false;

    public bool StartButtonIsInstantiated = false;

    public GameObject connectedPlayersTextGO;
    Text connectedPlayersText;

    public GameObject myPlayer;

    // Awake con patron singleton
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } 
        else if(instance != this) 
        {
            Destroy(gameObject);
        }
    }

    public void StartGame() {
        gameHasStarted = true;
        CalculateTimer();
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<InputTest>().inputEnabled = true;
        }
        lunarModules = GameObject.FindGameObjectsWithTag("LunarModule");
    }

    [Command]
    public void CmdChangeBoxOwner(GameObject box, GameObject newOwner)
    {
        RpcUpdateBoxOwnerID(box, newOwner);
    }

    [ClientRpc]
    public void RpcUpdateBoxOwnerID(GameObject box, GameObject owner)
    {
        Box boxScript = box.GetComponent<Box>();
        boxScript.owner = owner;

        boxScript.UpdateMaterialID();
    }

    [Command]
    public void CmdDropBoxMineral(GameObject box) {
        GameObject newMineral = box.GetComponent<Box>().DropMineral();
        if (newMineral != null) {
            NetworkServer.Spawn(newMineral);
            RpcPlayRandomSound(newMineral, MineralFragment.instanceSource.BOX);
        }
    }

    [Command]
    public void CmdShootParticles(GameObject shootParticles) {
        NetworkServer.Spawn(shootParticles);
    }

    [Command]
    public void CmdDestroyLaserShoot(GameObject laserShoot) {
        NetworkServer.Destroy(laserShoot);
    }

    [Command]
    public void CmdUpdateMineralLifeTime(GameObject mineral)
    {
        mineral.GetComponent<MineralFragment>().destroyTime -= Time.deltaTime;
        if (mineral.GetComponent<MineralFragment>().destroyTime <= 0)
        {
            NetworkServer.Destroy(mineral);
        }
    }
    
    [ClientRpc]
    public void RpcPlayRandomSound(GameObject mineral, MineralFragment.instanceSource instance)
    {
        mineral.GetComponent<MineralFragment>().PlayRandomSound(instance);
    }

    void Update()
    {
        if (!gameHasStarted)       
            return;                

        if (isServer){
            if (!finishedGame) {
                if (gameDuration > 0) {
                    if (gameDuration <= 60 && !lastMinute) {
                        lastMinute = true;
                        TimeAlarm();
                    }
                    CalculateTimer();
                    CalculateTimerForPlayers();
                    foreach(GameObject player in players) {
                        LunarModule playerLunarModule = player.GetComponent<PlayerBehaviors>().myLunarModule.GetComponent<LunarModule>();
                        
                        if (playerLunarModule.currentResources >= playerLunarModule.resourcesNeededToWin) {
                            finishedGame = true;
                            timerText.text = "0:00";
                            SetWinner(player);
                        }
                    }
                } else {
                    finishedGame = true;
                    timerText.text = "0:00";
                    SetWinner(CheckWinner());
                }
            }
        } else {
            CalculateTimerForPlayers();
        }
    }

    public void CalculateTimer() {
        gameDuration -= Time.deltaTime;
    }

    public void CalculateTimerForPlayers() {
        string minutes = Mathf.Floor(gameDuration / 60).ToString();
        string seconds = Mathf.Clamp(gameDuration % 60, 0, 59).ToString("00");
        timerText.text = minutes + ":" + seconds;
    }

    private void TimeAlarm() {
        RpcTimeAlarm();
    }

    [ClientRpc]
    private void RpcTimeAlarm() {
        alarmHUD.SetActive(true);
        gameObject.GetComponent<AudioSource>().Play();
        audioSource_OST.clip = lastMinuteOST;
        audioSource_OST.Play();
        Invoke("HideAlarmMessage", 5f);
    }

    public void SetWinner(GameObject player) {
        RpcShowWinner(player);
    }

    public GameObject CheckWinner() {
        int winnerResources = -1;
        GameObject winnerPlayer = null;
        foreach (GameObject module in lunarModules)
        {
            int currentPlayerResources = module.GetComponent<LunarModule>().getCurrentResources();
            if (module.GetComponent<LunarModule>().myPlayer != null && currentPlayerResources != 0 && currentPlayerResources > winnerResources) {
                winnerPlayer = module.GetComponent<LunarModule>().myPlayer;
                winnerResources = currentPlayerResources;
            }
        }
        return winnerPlayer;
    }

    private void HideAlarmMessage() {
        alarmHUD.SetActive(false);
        gameObject.GetComponent<AudioSource>().Stop();
    }

    [ClientRpc]
    public void RpcShowWinner(GameObject winnerPlayer) {
        myPlayer.GetComponent<HUDManager>().ShowWinner(winnerPlayer);
        Invoke("ShowEndGameHUD", 5f);
    }

    private void ShowEndGameHUD() {
        players[0].GetComponent<HUDManager>().ShowEndGameHUD();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void DestroyPlayer(GameObject p)
    {
        List<GameObject> newPlayers = new List<GameObject>();
        foreach (GameObject player in players)
        {
            if (player != p)
            {
                newPlayers.Add(player);
            }
        }

        Array.Resize(ref players, newPlayers.Count);
        players = newPlayers.ToArray();
    }


    public void MainMenu(string name)
    {
        SceneManager.LoadScene(name);
    }

    public bool IsServer()
    {
        return isServer;
    }

    public void ExitGame()
    {
        RpcQuitGame();
    }

    [ClientRpc]
    public void RpcQuitGame()
    {
        Application.Quit();
    }
}

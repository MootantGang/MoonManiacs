using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HUDManager : NetworkBehaviour
{

    private GameObject startInstructions; 
    private Text winnerText;
    private GameObject endGameHUD;
    public Button exitLunarModuleButton;
    private Text currentAlumoonite;
    private Text currentAlumooniteBox;
    [SyncVar]
    public GameObject winner = null;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Blocking_Jesus")
        {

            SearchElementsHUD();
            if (isLocalPlayer) {
                startInstructions.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (SceneManager.GetActiveScene().name == "Blocking_Jesus" && startInstructions.activeSelf && InputManager.Exit())
        {
            HideInstructions();
        }
    }

    public void HideInstructions() {
        startInstructions.SetActive(false);    
    }

    public void ShowWinner(GameObject winner) {
        winnerText.transform.parent.gameObject.SetActive(true);
        winnerText.text = (winner == null ? "No one" : winner.name) + " WINS!!!";
    }

    public void ShowEndGameHUD() {
        endGameHUD.SetActive(true);
    }

    public void ExitGame() {
        GameManager.instance.RpcQuitGame();
    }

    private void SearchElementsHUD()
    {
        GameObject ui = GameObject.Find("UICanvas");

        startInstructions = FindObject(ui, "StartInstructions");
        winnerText = FindObject(ui, "WinnerText").GetComponent<Text>();
        endGameHUD = FindObject(ui, "EndGameScreen");
        currentAlumoonite = FindObject(ui, "AlumooniteText").GetComponent<Text>();
        currentAlumooniteBox = FindObject(ui, "AlumooniteBoxText").GetComponent<Text>();
    }

    public GameObject FindObject(GameObject parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public void SetCurrentAlumoonite(int alumoonite)
    {
        currentAlumoonite.text = "x " + alumoonite.ToString();
    }

    public void SetCurrentAlumooniteBox(int alumooniteBox)
    {
        currentAlumooniteBox.text = "x " + alumooniteBox.ToString();
    }

    public void MainMenu(string name)
    {
        SceneManager.LoadScene(name);
    }
}

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class LobbyButtons : MonoBehaviour
{
    private NetworkManager networkManager;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        networkManager = NetworkManager.singleton;
    }

    public void LeaveRoom()
    {
        MatchInfo matchInfo = networkManager.matchInfo;
        networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
        networkManager.StopHost();
    }

    public void StartGame()
    {
        GameManager.instance.StartGame();       
    }

    public void QuitGame()
    {
        NetworkManager.singleton.ServerChangeScene("Blocking_Jesus");
        LeaveRoom();
    }
}

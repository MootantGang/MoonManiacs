using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RoomListItem : MonoBehaviour
{
    public delegate void JoinRoomDelegate(MatchInfoSnapshot _match);
    private JoinRoomDelegate joinRoomCallback;

    //[SerializeField]
    public Text roomNameText;

    private MatchInfoSnapshot match;

    //public GameObject networkm;

    //private void Start()
    //{
    //    networkm = GameObject.FindGameObjectWithTag("NetworkManager");
    //}

    public void Setup (MatchInfoSnapshot _match, JoinRoomDelegate _joinRoomCallback)
    {
        match = _match;
        joinRoomCallback = _joinRoomCallback;

        roomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
    }

    public void JoinRoom()
    {
        //networkm.GetComponent<MooMatchInfo>().mootantMatch = match;
        joinRoomCallback.Invoke(match);
    }
}
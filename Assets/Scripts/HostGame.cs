﻿using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour
{
    private uint roomSize = 6;

    private string roomName;

    NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName(string name)
    {
        roomName = name;
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomName))
        {
            //Debug.Log("Creating Room: " + roomName + " with room for " + roomSize + " players.");
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
        }
        /*else
            Debug.LogWarning("Please insert a name");*/
    }
}
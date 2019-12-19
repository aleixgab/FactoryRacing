﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    private TextMesh nameLabel;

    private CustomNetworkManager networkManager;

    public Transform finishTransform;

    // Name sync /////////////////////////////////////
    [SyncVar(hook = "SyncNameChanged")]
    string playerName = "Player";

    [Command]
    void CmdChangeName(string name) { playerName = name; }

    void SyncNameChanged(string name) { nameLabel.text = name; }


    // Prefab sync /////////////////////////////////////
    [Command]
    void CmdChangePlayerPrefab(int prefabIndex)
    {
        networkManager.ChangePlayerPrefab(this, prefabIndex);
    }
    [Command]
    void CmdChangePlayerPrefab(int prefabIndex, Transform parent)
    {
        networkManager.ChangePlayerPrefab(this, prefabIndex, parent);
    }
     
    // OnGUI /////////////////////////////////////////
    private void OnGUI()
    {
        if (isLocalPlayer)
        {
            GUILayout.BeginArea(new Rect(Screen.width - 260, 10, 250, Screen.height - 20));

            string prevPlayerName = playerName;
            playerName = GUILayout.TextField(playerName);
            if (playerName != prevPlayerName)
            {
                if (nameLabel != null)
                {
                    CmdChangeName(playerName);
                }
            }

            GUILayout.EndArea();
        }
    }

    // Finish sync ////////////////////////////////
    [SyncVar(hook = "SyncFinishLap")]
    bool isFinished = false;

    [Command]
    void CmdFinishLap() { isFinished = true; }
    void SyncFinishLap(bool b) 
    {
        finishTransform.gameObject.SetActive(b);
        CmdChangePlayerPrefab(networkManager.playerPrefabIndex + 3, finishTransform);
    }



    // Use this for initialization
    void Start ()
    {
        //nameLabel = transform.Find("Label").gameObject.GetComponent<TextMesh>();

        networkManager = NetworkManager.singleton.GetComponent<CustomNetworkManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag.ToString());
        if(other.CompareTag("Finish"))
        {
            isFinished = true;
            CmdFinishLap();
        }
    }
}

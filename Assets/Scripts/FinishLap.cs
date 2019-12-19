using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FinishLap : NetworkBehaviour
{

    [SyncVar(hook = "FinishLap")]
    string playerName = "Player";

    [Command]
    void CmdFinishLap(string name)
    {
        playerName = name;
    }

}

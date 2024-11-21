using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : NetworkBehaviour
{
    public string nickname;
    public int id;
    public int thisID;
   
    [Networked] public string NetworkedNickname { get; private set; }
    [Networked] public int NetworkedID { get; private set; }

    public static event Action OnNicknameChangeEvent;
    public static event Action<string> OnSpawnSetUINameEvent;
    
    
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            NetworkedNickname = FindObjectOfType<FusionBootstrap>().Username;
            NetworkedID = FindObjectOfType<FusionStarter>().currentPlayerData.id;
            if (NetworkedNickname == "")
            {
                NetworkedNickname = "Player " + Object.StateAuthority.PlayerId;
            }
            
            OnSpawnSetUINameEvent?.Invoke(NetworkedNickname);
            
        }

        nickname = NetworkedNickname;
        id = NetworkedID;
        
        Debug.Log(NetworkedNickname + " Joined the game. with user ID: " + NetworkedID);
        GetComponent<PlayerChat>().SendChatInputMessage(" Joined the game.");
        RPC_UpdateTextName();

    }

    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_UpdateTextName()
    {
        OnNicknameChangeEvent?.Invoke();
    }
}

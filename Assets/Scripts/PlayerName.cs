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
    public int thisID;
    
   
    [Networked] public string NetworkedNickname { get; private set; }

    public static event Action OnNicknameChangeEvent;
    
    
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            NetworkedNickname = FindObjectOfType<FusionBootstrap>().DefaultNickname;
            
        }

        thisID = Object.StateAuthority.PlayerId;
        nickname = NetworkedNickname;
        
        Debug.Log(NetworkedNickname + " Joined the game.");
        RPC_UpdateTextName();

    }

    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_UpdateTextName()
    {
        OnNicknameChangeEvent?.Invoke();
    }
}

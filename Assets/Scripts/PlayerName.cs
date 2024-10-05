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
    
    [Networked]
    public string NetworkedNickname { get; private set; }

    public static event Action OnNicknameChangeEvent;
    
    
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            NetworkedNickname = FindObjectOfType<FusionBootstrap>().DefaultNickname;
        }

        nickname = NetworkedNickname;
        RPC_UpdateTextName();

    }

    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_UpdateTextName()
    {
        OnNicknameChangeEvent?.Invoke();
    }
}

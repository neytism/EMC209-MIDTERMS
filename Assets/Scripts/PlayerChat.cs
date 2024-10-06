using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerChat : NetworkBehaviour
{
    public PlayerName playerName;
    
    private UIManager _uiManager;

    private void Awake()
    {
        _uiManager = FindObjectOfType<UIManager>();
        
        if (_uiManager != null)
        {
            _uiManager.OnButtonSendChatEvent += SendChatInputMessage;
        }
        else
        {
            Debug.LogWarning("UI Manager Found");
        }
    }
    
    public void SendChatInputMessage(string message)
    {
        if (Runner.LocalPlayer == Object.InputAuthority)
        {
            RPC_SendChat( playerName.NetworkedNickname, message);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendChat(string pname, string message)
    {
        _uiManager.InstantiateChat(pname, message);
    }

}

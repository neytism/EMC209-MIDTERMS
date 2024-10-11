using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerTeam : NetworkBehaviour //player manager
{
    public static event Action<bool> OnPlayerJoin;
    public static event Action<int, bool> OnChangeReadyStatusEvent;
    public static event Action<int, bool> OnChangeTeamStatusEvent;
    
    [Networked] public int Kills { get; set; } 
    [Networked] public int Deaths { get; set; } 
    [Networked] public bool IsReadyPlayer { get; set; } 
    
    [Networked] public bool IsRedTeam { get; set; } 

    private TeamManager _teamManager;
    public PlayerMovement playerMovement;

    public override void Spawned()
    {
        if (Runner.LocalPlayer != Object.StateAuthority)
        {
            OnPlayerJoin?.Invoke(IsRedTeam);
        }

        ReadyButton.OnHitReadyButtonEvent += SetIsReady;
        GameManager.OnStartBattleEvent += SpawnAndSetColor;
        
        PlayerInfo player = new PlayerInfo
        {
            name = GetComponent<PlayerName>().NetworkedNickname,
            deaths = Deaths,
            isReady = IsReadyPlayer,
            isRedTeam = IsRedTeam,
            kills = Kills,
            onlineId = Object.StateAuthority.PlayerId
        };
        
        string ser = JsonUtility.ToJson(player);

        FindObjectOfType<GameManager>().JoinPlayer(ser);

        StartCoroutine(SetTeamManager());
    }

    public void SetIsReady(bool b)
    {
        if (Runner.LocalPlayer == Object.StateAuthority)
        {
            IsReadyPlayer = b;
        }

        RPC_RelayReadyStatus(Object.StateAuthority.PlayerId, IsReadyPlayer);

    }

    [Rpc]
    public void RPC_RelayReadyStatus(int id, bool ready)
    {
        OnChangeReadyStatusEvent?.Invoke(id, ready);
    }

    private IEnumerator SetTeamManager()
    {
        while (_teamManager == null)
        {
            _teamManager = FindObjectOfType<TeamManager>();
            yield return null;  
        }
        
        if (HasStateAuthority)
        {
            SetTeam();
        }
        
        
    }
    
    public void SetTeam()
    {
        IsRedTeam = _teamManager.redTeamCount <= _teamManager.blueTeamCount;
        

        RPC_SendJoinTeam(IsRedTeam);
        RPC_RelayTeamSet(Object.StateAuthority.PlayerId, IsRedTeam);
    }
    
    [Rpc]
    public void RPC_RelayTeamSet(int id, bool isRedTeam)
    {
        OnChangeTeamStatusEvent?.Invoke(id, isRedTeam);
    }

    public void SpawnAndSetColor()
    {
        if (!HasStateAuthority) return;
        
        Debug.Log(IsRedTeam);
        
        Transform spawnPoint = _teamManager.SelectRandomSpawnPoint(IsRedTeam);

        playerMovement.CanMove = false;

        playerMovement.targetTransform = spawnPoint;
        playerMovement.isHelpingTeleport = true;

        
        //StartCoroutine(playermMovement.WaitBeforeMoving(spawnPoint.position));

        PlayerColor playerColor = GetComponent<PlayerColor>();

        playerColor.SetPlayerColor(IsRedTeam ? Color.red : Color.blue);
    }
    
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SendJoinTeam(bool isRedTeam)
    {
        OnPlayerJoin?.Invoke(isRedTeam);
        _teamManager.JoinTeam(isRedTeam);
        
    }
    
    //
    // public void SendPlayerInfo(string message)
    // {
    //     if (Runner.LocalPlayer == Object.InputAuthority)
    //     {
    //         RPC_SendPlayerInfo(message);
    //     }
    // }
    //
    // [Rpc(RpcSources.All, RpcTargets.All)]
    // public void RPC_SendPlayerInfo(string message)
    // {
    //   
    // }
    
    
}


[Serializable]
public class PlayerInfo
{
    public string name;
    public int onlineId;
    public int kills;
    public int deaths;
    public bool isReady;
    public bool isRedTeam;
}

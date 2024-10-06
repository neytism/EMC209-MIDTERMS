using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerTeam : NetworkBehaviour
{
    public static event Action<bool> OnPlayerJoin;

    [Networked] public bool IsRedTeam { get; set; } 

    private TeamManager _teamManager;
    public PlayerMovement playermMovement;

    public override void Spawned()
    {
        if (Runner.LocalPlayer != Object.StateAuthority)
        {
            OnPlayerJoin?.Invoke(IsRedTeam);
        }
        
        StartCoroutine(SetTeamManager());
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
        
        Debug.Log(IsRedTeam);
        
        Transform spawnPoint = _teamManager.SelectRandomSpawnPoint(IsRedTeam);

        StartCoroutine(playermMovement.WaitBeforeMoving(spawnPoint.position));

        PlayerColor playerColor = GetComponent<PlayerColor>();

        playerColor.SetPlayerColor(IsRedTeam ? Color.red : Color.blue);

        RPC_SendJoinTeam(IsRedTeam);
        
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SendJoinTeam(bool isRedTeam)
    {
        OnPlayerJoin?.Invoke(isRedTeam);
        //_teamManager.JoinTeam(isRedTeam);
    }
    
    
}

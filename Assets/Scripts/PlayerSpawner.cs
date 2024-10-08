using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public Transform[] spawnPoints;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            int spawnIndex = Runner.SessionInfo.PlayerCount % spawnPoints.Length;
            
            Runner.Spawn(PlayerPrefab, spawnPoints[spawnIndex].position, Quaternion.identity, player);


        }
    }

    
}
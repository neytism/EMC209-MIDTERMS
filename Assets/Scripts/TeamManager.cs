using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class TeamManager : MonoBehaviour
{
    public Material redMaterial;
    public Material blueMaterial;

    public Transform[] redSpawnPoints;
    public Transform[] blueSpawnPoints;

    public int redTeamCount = 0;
    public int blueTeamCount = 0;

    private void OnEnable()
    {
        PlayerTeam.OnPlayerJoin += JoinTeam;
    }

    public Transform SelectRandomSpawnPoint(bool isRedTeam)
    {
        Transform[] spawnPoints = isRedTeam ? redSpawnPoints : blueSpawnPoints;
        
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points available for the selected team.");
            return new GameObject().transform; // Return a default empty GameObject to avoid errors
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex];
    }

    public void JoinTeam(bool isRedTeam)
    {
        if (isRedTeam)
        {
            redTeamCount++;
        }
        else
        {
            blueTeamCount++;
        }
        
    }
}

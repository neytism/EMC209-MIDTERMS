using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   [Header("PLAYERS")] 
   public List<PlayerInfo> playerInfos = new List<PlayerInfo>();

   public static event Action OnStartBattleEvent;
   [SerializeField] private  UIManager _UIManager;

   private void OnEnable()
   {
      PlayerHealth.OnDeathUpdateKDEvent += UpdateKillDeathInfo;
      PlayerTeam.OnChangeReadyStatusEvent += SetPlayerAsReady;
      PlayerTeam.OnChangeTeamStatusEvent += SetPlayerTeam;
      GameTimer.OnEndTimerEvent += GameOver;
   }

   private void GameOver()
   {
      _UIManager.GameOver(playerInfos, IsRedTeamWinner());
   }

   public void JoinPlayer(string playerInfo)
   {
      PlayerInfo player = JsonUtility.FromJson<PlayerInfo>(playerInfo);
    
      playerInfos.Add(player);
      SortPlayerInfos();
      _UIManager.UpdateReadyListUI(playerInfos);
   }

   public void SetPlayerAsReady(int id, bool isReady)
   {
      foreach (var playerInfo in playerInfos)
      {
         if (playerInfo.onlineId == id)
         {
            playerInfo.isReady = isReady;
            break;
         }
      }
      _UIManager.UpdateReadyListUI(playerInfos);
      CheckIfEveryoneIsReady();
   }
   
   public void SetPlayerTeam(int id, bool isRedTeam)
   {
      foreach (var playerInfo in playerInfos)
      {
         if (playerInfo.onlineId == id)
         {
            playerInfo.isRedTeam = isRedTeam;
            break;
         }
      }
   }

   public void CheckIfEveryoneIsReady()
   {
      foreach (var playerInfo in playerInfos)
      {
         if (!playerInfo.isReady) return;
      }

      _UIManager.SetGameUIAfterAllReady();
      _UIManager.UpdatePlayerKDListUI(playerInfos);
      OnStartBattleEvent?.Invoke();
   }

   public void UpdateKillDeathInfo(int idDead, int idKiller)
   {
      foreach (var playerInfo in playerInfos)
      {
         if (playerInfo.onlineId == idDead)
         {
            playerInfo.deaths++;
            break;
         }
      }
      
      foreach (var playerInfo in playerInfos)
      {
         if (playerInfo.onlineId == idKiller)
         {
            playerInfo.kills++;
            break;
         }
      }
      
      SortPlayerInfosByKills();
      _UIManager.UpdatePlayerKDListUI(playerInfos);
   }
   
   public bool IsRedTeamWinner()
   {
      int redTeamKills = 0;
      int blueTeamKills = 0;

      foreach (var playerInfo in playerInfos)
      {
         if (playerInfo.isRedTeam)
         {
            redTeamKills += playerInfo.kills;
         }
         else
         {
            blueTeamKills += playerInfo.kills;
         }
      }

      return redTeamKills > blueTeamKills;
   }
   
   
   //sorting
   
   private void SortPlayerInfos()
   {
      playerInfos.Sort((p1, p2) => p1.onlineId.CompareTo(p2.onlineId));
   }
   
   private void SortPlayerInfosByKills()
   {
      playerInfos.Sort((p1, p2) => p2.kills.CompareTo(p1.kills));
   }

}

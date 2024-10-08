using System;
using Fusion;
using UnityEngine;

public class GameTimer : NetworkBehaviour
{
    [Networked] private TickTimer countdownTimer { get; set; } 
    [Networked] private bool timerActive { get; set; } 

    public float timerDuration = 180f; // 180 is 3 minutes
    private UIManager _uiManager;

    public override void Spawned()
    {
        _uiManager = FindObjectOfType<UIManager>();
        
        UIManager.OnStartBattleEvent += RPC_StartTimer;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartTimer()
    {
        timerActive = true;
        countdownTimer = TickTimer.CreateFromSeconds(Runner, timerDuration);
    }

    public override void FixedUpdateNetwork()
    {
        if (timerActive)
        {
            if (countdownTimer.Expired(Runner))
            {
                timerActive = false;
                RPC_OnTimerEnd(); 
            }
            else
            {
                RPC_DisplayTime(countdownTimer.RemainingTime(Runner).Value);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_DisplayTime(float remainingTime)
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        string timeString = string.Format("{0:D2}:{1:D2}", minutes, seconds);
        
        _uiManager.UpdateTimer(timeString);
        //Debug.Log("Time Remaining: " + timeString);
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_OnTimerEnd()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _uiManager.GameOver();
        Debug.Log("Timer has ended!");
       
    }
}
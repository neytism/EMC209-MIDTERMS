using System;
using Fusion;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public static event Action<float,float> OnLocalPlayerChangeHealthEvent; 
    
    [Networked, OnChangedRender(nameof(HealthChanged))]
    private float NetworkedHealth { get; set; }

    private const float MaxHealth = 100f;
    public event Action<float> OnDamageEvent;

    public override void Spawned()
    {
        NetworkedHealth = MaxHealth;
    }


    void HealthChanged()
    {
        Debug.Log($"Health changed to: {NetworkedHealth}");
        OnDamageEvent?.Invoke(NetworkedHealth/MaxHealth);

        if (HasStateAuthority)
        {
            if (NetworkedHealth >= MaxHealth) return;
            OnLocalPlayerChangeHealthEvent?.Invoke(NetworkedHealth,MaxHealth);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(float damage)
    {
        Debug.Log("Received DealDamageRpc on StateAuthority, modifying Networked variable");
        NetworkedHealth -= damage;
        
    }

}
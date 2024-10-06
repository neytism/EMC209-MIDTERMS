using System;
using Fusion;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public static event Action OnHurtEvent; 
    [Networked, OnChangedRender(nameof(HealthChanged))]
    private float NetworkedHealth { get; set; }

    private const float MaxHealth = 200f;
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
            OnHurtEvent?.Invoke();
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(float damage)
    {
        Debug.Log("Received DealDamageRpc on StateAuthority, modifying Networked variable");
        NetworkedHealth -= damage;
        
    }

}
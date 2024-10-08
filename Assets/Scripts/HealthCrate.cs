using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HealthCrate : NetworkBehaviour
{
    public float healAmount = 50;
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.TryGetComponent<PlayerHealth>(out var health))
        {
            if (health.IsDead) return;

            if (health.GetIsFullHealth()) return;

            if (healAmount > 0)
            {
                health.HealRpc(healAmount);
            }
            else
            {
                //health.DealDamageRpc(-healAmount);
            }
            
            Runner.Despawn(Object);
        }
       
    }
}

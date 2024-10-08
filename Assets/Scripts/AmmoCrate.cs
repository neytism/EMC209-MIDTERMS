using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class AmmoCrate : NetworkBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.TryGetComponent<PlayerWeapon>(out var weapon))
        {
            weapon.Reload();
            Runner.Despawn(Object);
        }
       
    }
}

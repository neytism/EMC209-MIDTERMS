using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotSoLobbyCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.TryGetComponent<PlayerMovement>(out var player))
        {
            player.CanMove = true;
        }
       
    }
}

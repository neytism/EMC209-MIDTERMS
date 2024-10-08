using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyButton : MonoBehaviour
{
    public static event Action<bool> OnHitReadyButtonEvent; 
    public bool isReady = false;

    public MeshRenderer ren;
    public Material red;
    public Material green;

    private void Awake()
    {
        ren.material = isReady ? green : red;
    }

    public void Interact()
    {
        isReady = !isReady;

        ren.material = isReady ? green : red;
        
        OnHitReadyButtonEvent?.Invoke(isReady);
    }
}

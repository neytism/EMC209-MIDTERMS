using System;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerColor : NetworkBehaviour
{
    public Color localColor;
    public GameObject[] visualObjects;

    public SkinnedMeshRenderer MeshRenderer;

    [Networked, OnChangedRender(nameof(ColorChanged))]
    public Color NetworkedColor { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            foreach (var vis in visualObjects)
            {
                vis.layer = LayerMask.NameToLayer("InvisibleToSelf");
            }
        }

        localColor = NetworkedColor;
        MeshRenderer.material.color = localColor;
    }

    // void Update()
    // {
    //     if (HasStateAuthority && Input.GetKeyDown(KeyCode.E))
    //     {
    //         // Changing the material color here directly does not work since this code is only executed on the client pressing the button and not on every client.
    //         NetworkedColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
    //     }
    // }
    
    void ColorChanged()
    {
        localColor = NetworkedColor;
        MeshRenderer.material.color = localColor;
    }

    public void SetPlayerColor(Color newColor)
    {
        if (HasStateAuthority)
        {
            NetworkedColor = newColor;
        }
        
    }
    
    
}
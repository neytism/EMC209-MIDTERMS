using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenu : MonoBehaviour
{
    private UIManager _uiManager;
    
    private void Start()
    {
        _uiManager = FindObjectOfType<UIManager>();
        if(!_uiManager) Debug.LogError("No UI Manager for Menu");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
           _uiManager.ShowMenu();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameTextController : MonoBehaviour
{
    public PlayerName _playerName;
    
    public TextMeshProUGUI nameHolder;
    
    private void Awake()
    {
        PlayerName.OnNicknameChangeEvent += UpdateNameText;
    }
    
    private void UpdateNameText()
    {
        nameHolder.text = _playerName.nickname;
    }
}

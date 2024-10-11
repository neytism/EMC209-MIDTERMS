using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Fusion;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
   public event Action<string> OnButtonSendChatEvent;
   
   [Header("General")]
   public GameObject HUDObject;
   
   [Header("Health")]
   public TextMeshProUGUI healthText;
   public Image healthBar;
   public Image blood;
   
   [Header("Chat Input")]
   public GameObject chatInputContainer;
   public Button buttonSendChat;
   public TMP_InputField inputChat;
   
   [Header("Chat Log")]
   public GameObject chatLogContainer;
   public RectTransform chatLogHolder;
   public GameObject chatTextPrefab;
   
   [Header("Gun")]
   public TextMeshProUGUI gunText;
   public TextMeshProUGUI ammoText;
   
   [Header("Name")]
   public TextMeshProUGUI nameText;
   
   [Header("Player List")]
   public TextMeshProUGUI playerListReadyInfo;
   public TextMeshProUGUI playerListKD;
   
   [Header("Timer")]
   public TextMeshProUGUI timerText;
   
   [Header("GameOver")]
   public GameObject gameOverPanel;
   public TextMeshProUGUI teamWinnerText;
   public GameObject playerInfoKDPrefab;
   
   private void Awake()
   {
      HUDObject.SetActive(false);
      buttonSendChat.onClick.AddListener(() =>
      {
         OnButtonSendChatEvent?.Invoke(inputChat.text);
         chatInputContainer.SetActive(false);
      });
      
        
   }
   
   private void OnEnable()
   {
      PlayerName.OnSpawnSetUINameEvent += OnLocalPlayerSpawn;
      PlayerHealth.OnLocalPlayerChangeHealthEvent += UpdateUIHealth;
      PlayerHealth.OnLocalPlayerHurtEvent += TriggerHurtEffect;
      
      PlayerWeapon.OnAmmoChangeEvent += UpdateUIOnUseAmmo;
      PlayerWeapon.OnGunChangeEvent += UpdateUIOnChangeGun;
      
   }
   
   private void Update()
   {
      if(Input.GetKeyUp(KeyCode.Return))
      {
         if (chatInputContainer.activeSelf)
         {
            if (inputChat.text == String.Empty)
            {
               chatInputContainer.SetActive(false);
               chatLogContainer.SetActive(false);
               inputChat.DeactivateInputField();
               
            }
            else
            {
               buttonSendChat.onClick.Invoke();
            }
               
         }
         else
         {
            chatLogContainer.SetActive(true);
            chatInputContainer.SetActive(true);
            inputChat.ActivateInputField();
         }
            
         inputChat.Select();
         inputChat.text = "";
      }
   }

   private void OnLocalPlayerSpawn(string newPlayerName)
   {
      HUDObject.SetActive(true);
      nameText.text = newPlayerName;
   }

   private void UpdateUIHealth(float currentHealth, float maxHealth)
   {
      healthText.text = $"{currentHealth}";
      healthBar.fillAmount = currentHealth / maxHealth;
   }

   private void TriggerHurtEffect()
   {
      blood.DOColor(new Color(blood.color.r, blood.color.g, blood.color.b, 1f), 0f).onComplete = () =>
      {
         blood.DOColor(new Color(blood.color.r, blood.color.g, blood.color.b, 0f), 0.5f);
      };
   }
   
   private void UpdateUIOnUseAmmo(int currentAmmo)
   {
      ammoText.text = $"{currentAmmo}";
   }
   
   private void UpdateUIOnChangeGun(string gunName)
   {
      gunText.text = $"{gunName}";
   }
   
   public void InstantiateChat(string pname, string message)
   {
      StartCoroutine(ShowHideChatLog(3f));
      GameObject newChat = Instantiate(chatTextPrefab, chatLogHolder);
      newChat.GetComponent<TextMeshProUGUI>().text = "<" + pname + "> " +message;
   }
   
   private IEnumerator ShowHideChatLog(float secs)
   {
      chatLogContainer.SetActive(true);

      yield return new WaitForSeconds(secs);

      chatLogContainer.SetActive(false);
   }

   public void UpdateTimer(string time)
   {
      timerText.text = time;
   }
   
   public void GameOver(List<PlayerInfo> playerInfos, bool isRedTeamWinner)
   {
      gameOverPanel.SetActive(true);

      teamWinnerText.text = isRedTeamWinner ? "TEAM RED WINS!<br><br>" : "TEAM BLUE WINS!<br><br>";
      
      foreach (var playerInfo in playerInfos)
      {
         string color = playerInfo.isRedTeam ? "red" : "blue";
         GameObject o = Instantiate(playerInfoKDPrefab, gameOverPanel.transform);
         o.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"<color=\"{color}\">{playerInfo.name}";
         o.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"<color=\"{color}\">{playerInfo.kills.ToString()}";
         o.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"<color=\"{color}\">{playerInfo.deaths.ToString()}";
      }
   }

   public void UpdateReadyListUI(List<PlayerInfo> playerInfos)
   {
      playerListReadyInfo.text = "";

      string newText = "";

      foreach (var playerInfo in playerInfos)
      {
         string isReady = playerInfo.isReady ? "Ready" : "Not Ready";
         newText += $"{isReady} - {playerInfo.name} <br>";
      }

      playerListReadyInfo.text = newText;
   }
   
   public void UpdatePlayerKDListUI(List<PlayerInfo> playerInfos)
   {
      playerListKD.text = "";

      string newText = "";

      foreach (var playerInfo in playerInfos)
      {
         string color = playerInfo.isRedTeam ? "red" : "blue";
         newText += $"<color=\"{color}\">{playerInfo.name} - {playerInfo.kills} - {playerInfo.deaths}<br>";
      }

      playerListKD.text = newText;
   }

   public void SetGameUIAfterAllReady()
   {
      playerListReadyInfo.gameObject.SetActive(false);
      playerListKD.gameObject.SetActive(true);
   }

   
}

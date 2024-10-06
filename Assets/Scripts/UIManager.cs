using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
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
   
   [Header("Name")]
   public TextMeshProUGUI nameText;

   
   
   private void Awake()
   {
      buttonSendChat.onClick.AddListener(() =>
      {
         OnButtonSendChatEvent?.Invoke(inputChat.text);
         chatInputContainer.SetActive(false);
      });
      
        
   }
   
   private void OnEnable()
   {
      PlayerName.OnSpawnSetUINameEvent += OnLocalPlayerSpawn;
      PlayerHealth.OnLocalPlayerChangeHealthEvent += UpdateUIOnDamage;
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

   private void UpdateUIOnDamage(float currentHealth, float maxHealth)
   {
      blood.DOColor(new Color(blood.color.r, blood.color.g, blood.color.b, 1f), 0f).onComplete = () =>
      {
         blood.DOColor(new Color(blood.color.r, blood.color.g, blood.color.b, 0f), 0.5f);
      };

      healthText.text = $"{currentHealth}";
      healthBar.fillAmount = currentHealth / maxHealth;
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
}

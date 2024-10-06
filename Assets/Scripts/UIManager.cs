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
   
   public GameObject chatInputContainer;
    
   public Button buttonSendChat;
   public TMP_InputField inputChat;
   
   public RectTransform chatLogHolder;
   public GameObject chatTextPrefab;
   
   public TextMeshProUGUI nameText;

   public Image blood;
   
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
      PlayerName.OnSpawnSetUINameEvent += UpdateNameUI;
      PlayerHealth.OnHurtEvent += TriggerHurtEffect;
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
               inputChat.DeactivateInputField();
            }
            else
            {
               buttonSendChat.onClick.Invoke();
            }
               
         }
         else
         {
            chatInputContainer.SetActive(true);
            inputChat.ActivateInputField();
         }
            
         inputChat.Select();
         inputChat.text = "";
      }
   }

   private void UpdateNameUI(string obj)
   {
      nameText.text = obj;
   }

   private void TriggerHurtEffect()
   {
      blood.DOColor(new Color(blood.color.r, blood.color.g, blood.color.b, 1f), 0f).onComplete = () =>
      {
         blood.DOColor(new Color(blood.color.r, blood.color.g, blood.color.b, 0f), 0.5f);
      };
   }
   
   public void InstantiateChat(string pname, string message)
   {
      GameObject newChat = Instantiate(chatTextPrefab, chatLogHolder);
      newChat.GetComponent<TextMeshProUGUI>().text = "<" + pname + "> " +message;
   }
}

﻿using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Chat
{
    public class ChatSystem : MonoBehaviour
    {
        [Header("Chat Settings")]
        [SerializeField] private GameObject chatPanel;
        [SerializeField] private GameObject playerBubble;
        [SerializeField] private GameObject friendBubble;
        [SerializeField] private List<GameObject> messageList = new List<GameObject>();
        [SerializeField] private int maxMessages = 20;
        
        void Start()
        {
            PlayerTextInput.OnTextInput += DisplayPlayerText;
            FriendBehaviour.OnFriendResponse += DisplayFriendText;
        }
        
        void Update()
        {
            //Removes the first message when it reaches the maximum
            if (messageList.Count > maxMessages)
            {
                Destroy(messageList[0]);
                messageList.Remove(messageList[0]);
            }
        }
        
        private void DisplayPlayerText(string text)
        {
            if (!GameManager.Instance.inGame) return;
        
            GameObject message = Instantiate(playerBubble, chatPanel.transform);
            message.GetComponent<ChatBubble>().Setup(text);
            messageList.Add(message);
        }
        
        private void DisplayFriendText(string text)
        {
            if (!GameManager.Instance.inGame) return;
            
            GameObject message = Instantiate(friendBubble, chatPanel.transform);
            message.GetComponent<ChatBubble>().Setup(text);
            messageList.Add(message);
        }
        
        private void OnDestroy()
        {
            PlayerTextInput.OnTextInput -= DisplayPlayerText;
            FriendBehaviour.OnFriendResponse -= DisplayFriendText;
        }
    }
}
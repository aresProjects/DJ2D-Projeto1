﻿using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Core
{
    public class FriendBehaviour : MonoBehaviour
    {
        [Header("Commands Settings")]
        [SerializeField] private string[] commands;
        
        [Header("Movement Settings")] 
        [SerializeField] private Tilemap wallTilemap; //To prevent the "friend" bot going over walls
        [SerializeField] private int moveDistance;
        private Transform friendPos;
        
        [Header("Monologue Settings")]
        [SerializeField] private TMP_InputField inputField; //Used to prevent the player responding before the starting monologue
        [SerializeField] private List<string> startingMonologues;
        [SerializeField] private float tBetweenMonologues;
        
        [Header("Enemy Settings")]
        [SerializeField] private EnemyBehaviour enemy;
        
        public static Action<string> OnFriendResponse;
        public static Action<Vector3> OnFriendMove;
        
        void Start()
        {
            friendPos = GetComponent<Transform>();
            PlayerTextInput.OnTextInput += TryMove; //Subscribes to the action "OnTextInput"
            OnFriendResponse += PlaySound;
            
            StartCoroutine(StartingMonologue());
        }
        
        void Update()
        {
            CheckDeath();
            CheckExit();
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Tries moving the "friend" bot by checking if the input corresponds to a command
        /// </summary>
        /// <param name="textInput"></param>
        /// <returns></returns>
        private void TryMove(string textInput)
        {
            string[] words = textInput.Split(' ');
            int direction = -1;
            Vector3 currentPos = friendPos.position;
            int nMoves = 1;
            bool canMove = true;
            bool isRunning = false; //Prevents the friend "bot" from responding to each "run" command
            
            if (words.Length == 2)
            {
                //Parses the number of moves
                if (Int32.TryParse(words[0], out int result))
                {
                    nMoves = result;
                }
                else if (words[0].ToUpper() == "RUN") //Checks if the input corresponds to "run"
                {
                    nMoves = wallTilemap.size.x; //Random num (with a minimum of the width of the wall tilemap)
                    isRunning = true;
                }
                else
                {
                    OnFriendResponse?.Invoke("Sorry, could you repeat?");
                    return;
                }
                
                //Checks if the input correspond to any command
                for (int i = 0; i < commands.Length; i++)
                {
                    if (words[1].ToUpper() == commands[i])
                    {
                        direction = i;
                        break;
                    }
                }
            }
            
            //Moves the "friend" bot "nMoves" times, always checking if it's possible to do so
            for (int i = 0; i < nMoves; i++)
            {
                switch (direction)
                {
                    case -2: //WALL (CAN'T MOVE)
                    {
                        if (!isRunning || i == 0)
                        {
                            OnFriendResponse?.Invoke("There is a wall in front of me!");
                        }
                        canMove = false;
                        break;
                    }
                    case -1: //WRONG COMMAND
                    {
                        OnFriendResponse?.Invoke("Sorry, could you repeat?");
                        canMove = false;
                        break;
                    }
                    case 0: //LEFT
                    {
                        if (wallTilemap.HasTile(new Vector3Int((int) currentPos.x - 2, (int) currentPos.y, 0)))
                        {
                            goto case -2;
                        }
                        
                        friendPos.Translate(Vector2.left * moveDistance);
                        break;
                    }
                    case 1: //RIGHT
                    {
                        if (wallTilemap.HasTile(new Vector3Int((int) currentPos.x + 1, (int) currentPos.y, 0)))
                        {
                            goto case -2;
                        }
                        
                        friendPos.Translate(Vector2.right * moveDistance);
                        break;
                    }
                    case 2: //UP
                    {
                        if (wallTilemap.HasTile(new Vector3Int((int) currentPos.x, (int) currentPos.y + 1, 0)))
                        {
                            goto case -2;
                        }
                        
                        friendPos.Translate(Vector2.up * moveDistance);
                        break;
                    }
                    case 3: //DOWN
                    {
                        if (wallTilemap.HasTile(new Vector3Int((int) currentPos.x, (int) currentPos.y - 2, 0)))
                        {
                            goto case -2;
                        }
                        
                        friendPos.Translate(Vector2.down * moveDistance);
                        break;
                    }
                }
                
                if (!canMove) break;
                
                currentPos = friendPos.position;

                OnFriendMove?.Invoke(currentPos); //Used in "EnemyBehaviour"
            }
        }
        
        private IEnumerator StartingMonologue()
        {
            inputField.readOnly = true;
        
            foreach (var monologue in startingMonologues)
            {
                yield return new WaitForSeconds(tBetweenMonologues + monologue.Length/20);
                OnFriendResponse?.Invoke(monologue);
            }
            
            inputField.readOnly = false;
            inputField.ActivateInputField();
        }
        
        private void CheckExit()
        {
            if (transform.position == MazeGenerator.exitPosition)
            {
                SceneManager.LoadScene("WinScreen");
            }
        }
        
        private void CheckDeath()
        {
            if (enemy.transform.position == transform.position && enemy.canMove)
            {
                SceneManager.LoadScene("ScreenOfDeath");
            }
        }
        
        private void PlaySound(string s)
        {
            AudioManager.Instance.PlaySound(AudioManager.SoundName.ChatNotification);
        }
        
        private void OnDestroy()
        {
            PlayerTextInput.OnTextInput -= TryMove;
            OnFriendResponse -= PlaySound;
        }
    }
}
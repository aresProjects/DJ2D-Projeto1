﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerTextInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField textInput;
    
    public static Action<string> OnTextInput; //Used to notify the "friend" bot

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && textInput.text != "")
        {
            SaveText();
            ClearText();
        }
    }

    private void SaveText()
    {
        if (OnTextInput != null)
        {
            OnTextInput(textInput.text);
        }
    }

    private void ClearText()
    {
        textInput.text = "";
        textInput.ActivateInputField();
    }
}

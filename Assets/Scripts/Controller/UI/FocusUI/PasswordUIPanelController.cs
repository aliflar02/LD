using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PasswordUIPanelController : MonoBehaviour
{
    [SerializeField] private List<GameObject> btnList;
    [SerializeField] private List<TMP_Text> textList;
    [SerializeField] private string correctPassword = "AAAAAB";
    // Start is called before the first frame update
    void Start()
    {
        MUIEventListener.Get(gameObject).onClick = _ =>
        {
            UIManager.Instance.HideFocusUI();
        };

        int bindCount = Mathf.Min(btnList.Count, textList.Count);
        for (int i = 0; i < bindCount; i++)
        {
            int index = i; // 需要一个局部变量来捕获当前的索引
            MUIEventListener.Get(btnList[i]).onClick = _ =>
            {
                textList[index].text = GetNextLetter(textList[index].text);
                // 检查密码是否正确
                CheckPassword();
            };
        }
    }

    private void CheckPassword()
    {
        string enteredPassword = string.Empty;
        foreach (var text in textList)
        {
            enteredPassword += text.text;
        }

        if (enteredPassword.Equals(correctPassword, StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("密码正确！");
            UIManager.Instance.ShowFocusUI(FocusPanelType.BoxResult);
        }
    }

    private string GetNextLetter(string currentText)
    {
        if (string.IsNullOrEmpty(currentText))
        {
            return "A";
        }

        char currentChar = char.ToUpperInvariant(currentText[0]);
        if (currentChar < 'A' || currentChar > 'Z')
        {
            return "A";
        }

        char nextChar = currentChar == 'Z' ? 'A' : (char)(currentChar + 1);
        return nextChar.ToString();
    }


}

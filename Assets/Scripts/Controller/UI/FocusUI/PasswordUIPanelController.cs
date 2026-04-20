using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordUIPanelController : MonoBehaviour
{
    [SerializeField] private List<GameObject> btnList;
    [SerializeField] private List<TMP_Text> textList;
    [SerializeField] private GameObject boxObject;
    [SerializeField] private Image outputBgImage;
    [SerializeField] private Image RedMask;
    [SerializeField] private string correctPassword = "AAAAAB";
    [SerializeField] private string inputPassword = "";
    private int currentInputIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        RedMask.gameObject.SetActive(false);
        ClearInput();

        MUIEventListener.Get(gameObject).onClick = _ =>
        {
            AudioManager.Instance.PlaySFX(ESFXType.Click);
            UIManager.Instance.HideFocusUI();
        };

        for (int i = 0; i < btnList.Count; i++)
        {
            int index = i; // 需要一个局部变量来捕获当前的索引
            btnList[i].transform.localScale = Vector3.one;
            TMP_Text text = btnList[i].GetComponentInChildren<TMP_Text>();
            text.text = GetLetterByIndex(i);
            MUIEventListener.Get(btnList[i]).onClick = _ =>
            {
                _.transform.DOPunchScale(Vector3.one * -0.1f, 0.2f, 1).SetEase(Ease.OutCubic);
                AudioManager.Instance.PlaySFX(ESFXType.Password);
                OnLetterButtonClicked(index);
            };
        }
    }

    private void OnLetterButtonClicked(int letterIndex)
    {
        if (currentInputIndex >= textList.Count)
        {
            return;
        }

        string letter = GetLetterByIndex(letterIndex);
        textList[currentInputIndex].text = letter;
        currentInputIndex++;

        if (currentInputIndex >= textList.Count)
        {
            CheckPassword();
        }
    }

    private void CheckPassword()
    {
        string enteredPassword = string.Empty;
        foreach (var text in textList)
        {
            enteredPassword += text.text;
        }

        inputPassword = enteredPassword;

        if (enteredPassword.Equals(correctPassword, StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("密码正确！");
            AudioManager.Instance.PlaySFX(ESFXType.RedLight);
            AudioManager.Instance.PlayBGM(EBGMType.End);
            RedMask.gameObject.SetActive(true);
            RedMask.DOFade(1f, 0.5f).From(0f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                boxObject.SetActive(false);
                RedMask.DOFade(0f, 0.2f).SetEase(Ease.InCubic).OnComplete(() =>
                {
                    UIManager.Instance.ShowFocusUI(FocusPanelType.BoxResult);
                });
            });
        }
        else
        {
            Debug.Log("密码错误，已清空，请重新输入。");
            outputBgImage.DOColor(Color.red, 0.2f).From(Color.white).SetEase(Ease.InCubic).OnComplete(() =>
            {
                outputBgImage.color = Color.white;
                ClearInput();
            });
        }
    }

    private void ClearInput()
    {
        currentInputIndex = 0;
        inputPassword = string.Empty;
        foreach (var text in textList)
        {
            text.text = string.Empty;
        }
    }

    private string GetLetterByIndex(int index)
    {
        if (index < 0)
        {
            return "A";
        }

        int normalized = index % 26;
        char c = (char)('A' + normalized);
        return c.ToString();
    }


}

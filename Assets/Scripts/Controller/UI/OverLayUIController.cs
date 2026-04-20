using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OverLayUIController : MonoBehaviour
{
    [Header("全屏遮罩")]
    [SerializeField] private Image blackFadeImage;
    [SerializeField] private Image redFlashImage;
    [SerializeField] private Image BGImage;

    [SerializeField] private bool hasClicked = false;

    void Awake()
    {
        // 初始化
        blackFadeImage.gameObject.SetActive(true);
        redFlashImage.gameObject.SetActive(false);

        // 监听事件
        MUIEventListener.Get(blackFadeImage.gameObject).onClick = OnOverlayClicked;
        MUIEventListener.Get(redFlashImage.gameObject).onClick = OnRedFlashClicked;
    }

    private void OnOverlayClicked(GameObject go)
    {
        AudioManager.Instance.PlaySFX(ESFXType.Click);
        AudioManager.Instance.PlayBGM(EBGMType.Main);
        if (!hasClicked)
        {
            hasClicked = true;
            UIManager.Instance.ShowDialoguePanel();

            // 点击后执行的逻辑，显示煤油灯图片
            redFlashImage.gameObject.SetActive(true);
            //显示出底部暗的环境
            blackFadeImage.DOFade(0F, 0.5f).From(1f).OnComplete(() =>
            {
                blackFadeImage.gameObject.SetActive(false);
            });
        }

    }

    // 点击煤油灯
    private void OnRedFlashClicked(GameObject go)
    {
        AudioManager.Instance.PlaySFX(ESFXType.TrunLight);
        if (hasClicked)
        {
            Debug.Log("点击煤油灯，淡出遮罩，打开煤油灯界面");
            // 点击后执行的逻辑，淡出遮罩
            redFlashImage.gameObject.SetActive(false);
            BGImage.DOFade(0, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
            UIManager.Instance.ShowFocusUI(FocusPanelType.Lamp);
        }
    }

}

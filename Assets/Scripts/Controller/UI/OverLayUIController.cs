using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OverLayUIController : MonoBehaviour
{
    [Header("全屏遮罩")]
    [SerializeField] private Image blackFadeImage;
    [SerializeField] private Image redFlashImage;

    [SerializeField] private bool hasClicked = false;

    void Awake()
    {
        // 初始化
        blackFadeImage.gameObject.SetActive(true);
        redFlashImage.gameObject.SetActive(false);
        //AudioManager.Instance.PlayBGM("BGM_MainTheme");
        Debug.Log("打开背景音乐");

        // 监听事件
        MUIEventListener.Get(blackFadeImage.gameObject).onClick += OnOverlayClicked;
        MUIEventListener.Get(redFlashImage.gameObject).onClick += OnRedFlashClicked;
    }

    private void OnOverlayClicked(GameObject go)
    {
        if (!hasClicked)
        {
            hasClicked = true;
            Debug.Log("弹出人物立绘和对话框 “莱特博士让我来取文件，这里好黑…”");
            // 点击后执行的逻辑，显示煤油灯图片
            redFlashImage.gameObject.SetActive(true);
            redFlashImage.DOFade(0.5f, 0.5f).From(0);
        }

    }

    // 点击煤油灯
    private void OnRedFlashClicked(GameObject go)
    {
        if (hasClicked)
        {
            Debug.Log("点击煤油灯，淡出遮罩，打开煤油灯界面");
            // 点击后执行的逻辑，淡出遮罩
            redFlashImage.DOFade(0, 0.5f);
            blackFadeImage.DOFade(0, 0.5f).OnComplete(() =>
            {
                blackFadeImage.gameObject.SetActive(false);
                gameObject.SetActive(false);
            });
            UIManager.Instance.ShowFocusUI(FocusPanelType.Lamp);
        }
    }

    void OnDestroy()
    {
        // 取消事件监听
        MUIEventListener.Get(blackFadeImage.gameObject).onClick -= OnOverlayClicked;
        MUIEventListener.Get(redFlashImage.gameObject).onClick -= OnRedFlashClicked;
    }
}

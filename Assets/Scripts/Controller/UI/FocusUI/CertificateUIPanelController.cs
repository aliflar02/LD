using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CertificateUIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject endGameScene;
    [SerializeField] private GameObject blackBg;
    [SerializeField] private GameObject certificatePanel;
    [SerializeField] private GameObject endGamePanel;
    void Awake()
    {
        certificatePanel.SetActive(false);
        endGamePanel.SetActive(false);
        endGameScene.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        MUIEventListener.Get(blackBg).onClick = _ =>
        {
            //点击黑背景，显示证明书界面
            certificatePanel.SetActive(true);
        };
        MUIEventListener.Get(certificatePanel).onClick = _ =>
        {
            //点击证明书界面
            certificatePanel.SetActive(false);
            Debug.Log("点击了证明书界面，黑屏，打开黑暗场景，打开对话，关闭对话后游戏结束");
            endGameScene.SetActive(true);
        };
        MUIEventListener.Get(endGameScene).onClick = _ =>
        {
            endGameScene.SetActive(false);
            endGamePanel.SetActive(true);
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
}

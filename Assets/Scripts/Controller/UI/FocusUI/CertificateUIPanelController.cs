
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CertificateUIPanelController : MonoBehaviour
{
    [SerializeField] private Image endGameScene;
    [SerializeField] private GameObject blackBg;
    [SerializeField] private GameObject certificatePanel;
    [SerializeField] private GameObject endGamePanel;
    void Awake()
    {
        certificatePanel.SetActive(false);
        endGamePanel.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        endGameScene.gameObject.SetActive(false);
        MUIEventListener.Get(blackBg).onClick = _ =>
        {
            //点击黑背景，显示证明书界面
            AudioManager.Instance.PlaySFX(ESFXType.Click);
            certificatePanel.SetActive(true);
        };
        MUIEventListener.Get(certificatePanel).onClick = _ =>
        {
            //点击证明书界面
            certificatePanel.SetActive(false);
            AudioManager.Instance.PlaySFX(ESFXType.Click);
            blackBg.SetActive(true);
            UIManager.Instance.ShowDialoguePanel();
            UIManager.Instance.PlaySequence("SEQ_08_LOOP_ENDING");
        };
        MUIEventListener.Get(endGameScene.gameObject).onClick = _ =>
        {
            AudioManager.Instance.PlaySFX(ESFXType.Click);
            endGameScene.gameObject.SetActive(false);
            endGamePanel.SetActive(true);
        };
        MUIEventListener.Get(endGamePanel).onClick = _ =>
        {
            AudioManager.Instance.PlaySFX(ESFXType.Click);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        };
        UIManager.Instance.RegisterSequenceFinishedListener(sequenceId =>
        {
            if (sequenceId == "SEQ_08_LOOP_ENDING")
            {
                endGameScene.gameObject.SetActive(true);
                endGameScene.DOFade(1f, 0.3f).From(0f).OnComplete(() =>
                {
                    blackBg.SetActive(false);
                });
            }
        });
    }
}

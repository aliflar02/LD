
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CertificateUIPanelController : MonoBehaviour
{
    [SerializeField] private Image endGameScene;
    [SerializeField] private GameObject endGameLightScene;
    [SerializeField] private GameObject endGameSceneLight;

    [SerializeField] private GameObject blackBg;
    [SerializeField] private GameObject certificatePanel;
    [SerializeField] private GameObject certificatePanel1;
    [SerializeField] private GameObject endGamePanel;

    //结束流程
    [SerializeField] private GameObject deskHotspot;
    [SerializeField] private GameObject deskPanel;
    [SerializeField] private GameObject notePanel;
    void Awake()
    {
        certificatePanel.SetActive(false);
        certificatePanel1.SetActive(false);
        endGamePanel.SetActive(false);
        endGameLightScene.SetActive(false);
        endGameSceneLight.SetActive(true);

        deskPanel.SetActive(false);
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
            certificatePanel.SetActive(false);
            certificatePanel1.SetActive(true);
            AudioManager.Instance.PlaySFX(ESFXType.RedLight);
        };
        MUIEventListener.Get(certificatePanel1).onClick = _ =>
        {
            //点击证明书界面
            certificatePanel1.SetActive(false);
            AudioManager.Instance.PlaySFX(ESFXType.Click);
            blackBg.SetActive(true);
            UIManager.Instance.ShowDialoguePanel();
            UIManager.Instance.PlaySequence("SEQ_08_LOOP_ENDING");
        };
        MUIEventListener.Get(endGameSceneLight.gameObject).onClick = _ =>
        {
            AudioManager.Instance.PlaySFX(ESFXType.TrunLight);
            endGameLightScene.SetActive(true);
            endGameSceneLight.SetActive(false);
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

        MUIEventListener.Get(deskHotspot).onClick = _ =>
        {
            deskPanel.SetActive(true);
        };
        MUIEventListener.Get(notePanel).onClick = _ =>
            {
                AudioManager.Instance.PlaySFX(ESFXType.Click);
                endGameLightScene.SetActive(false);
                endGameSceneLight.SetActive(false);
                endGamePanel.SetActive(true);
            };
    }
}

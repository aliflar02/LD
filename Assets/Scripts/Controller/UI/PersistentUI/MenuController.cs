using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Framework.Core;


public class MenuController : MonoBehaviour
{
    [SerializeField] private Button btnOpenMenu;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TMP_Text menuText;
    [SerializeField] private Button btnYes;
    [SerializeField] private Button btnNo;
    [SerializeField] private bool isMenuOpen = false;
    [SerializeField] private float openDuration = 0.3f;
    [SerializeField] private float closeDuration = 0.25f;
    [SerializeField] private float openStartScale = 0.8f;

    private RectTransform menuPanelRect;
    private RectTransform btnOpenMenuRect;
    private Vector3 menuTargetWorldPosition;
    private Tween menuTween;

    private void Awake()
    {
        menuPanelRect = menuPanel.GetComponent<RectTransform>();
        btnOpenMenuRect = btnOpenMenu != null ? btnOpenMenu.GetComponent<RectTransform>() : null;
        menuTargetWorldPosition = menuPanelRect.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        menuTargetWorldPosition = menuPanelRect.position;
        menuPanel.SetActive(false);
        btnOpenMenu.onClick.AddListener(() =>
        {
            Debug.Log("Open Menu");
            if (isMenuOpen)
                return;
            AudioManager.Instance.PlaySFX(ESFXType.Click);
            isMenuOpen = true;
            menuTween?.Kill();
            menuPanel.SetActive(true);
            Vector3 buttonWorldPosition = GetOpenButtonWorldPosition();
            menuPanelRect.position = new Vector3(buttonWorldPosition.x, menuTargetWorldPosition.y, menuTargetWorldPosition.z);
            menuPanel.transform.localScale = Vector3.one * openStartScale;

            menuTween = DOTween.Sequence()
                .Append(menuPanelRect.DOMoveX(menuTargetWorldPosition.x, openDuration).SetEase(Ease.OutCubic))
                .Join(menuPanel.transform.DOScale(1f, openDuration).SetEase(Ease.OutBack));
        });

        btnYes.onClick.AddListener(() =>
        {
            Debug.Log("Yes");
            AudioManager.Instance.PlaySFX(ESFXType.Click);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
        btnNo.onClick.AddListener(() =>
        {
            Debug.Log("No");
            AudioManager.Instance.PlaySFX(ESFXType.Click);
            menuTween?.Kill();
            Vector3 buttonWorldPosition = GetOpenButtonWorldPosition();
            menuTween = DOTween.Sequence()
                .Append(menuPanelRect.DOMoveX(buttonWorldPosition.x, closeDuration).SetEase(Ease.InCubic))
                .Join(menuPanel.transform.DOScale(openStartScale, closeDuration).SetEase(Ease.InBack))
                .OnComplete(() =>
                {
                    menuPanel.SetActive(false);
                    isMenuOpen = false;
                });
        });

        GameManager.Instance.Model.CurrentLanguage.RegisterWithInitValue(value =>
        {
            switch (value)
            {
                case ELanguage.English:
                    menuText.text = "Exit";
                    break;
                case ELanguage.Chinese:
                    menuText.text = "退出游戏";
                    break;
            }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private Vector3 GetOpenButtonWorldPosition()
    {
        if (btnOpenMenuRect != null)
        {
            return btnOpenMenuRect.position;
        }

        return btnOpenMenu != null ? btnOpenMenu.transform.position : menuTargetWorldPosition;
    }
}

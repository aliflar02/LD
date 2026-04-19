using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class MenuController : MonoBehaviour
{
    [SerializeField] private Button btnOpenMenu;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button btnYes;
    [SerializeField] private Button btnNo;
    [SerializeField] private bool isMenuOpen = false;
    [SerializeField] private float openDuration = 0.3f;
    [SerializeField] private float closeDuration = 0.25f;
    [SerializeField] private float openStartScale = 0.8f;

    private RectTransform menuPanelRect;
    private Vector2 menuTargetAnchoredPosition;
    private Tween menuTween;

    private void Awake()
    {
        menuPanelRect = menuPanel.GetComponent<RectTransform>();
        menuTargetAnchoredPosition = menuPanelRect.anchoredPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
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
            menuPanelRect.anchoredPosition = Vector2.zero;
            menuPanel.transform.localScale = Vector3.one * openStartScale;

            menuTween = DOTween.Sequence()
                .Append(menuPanelRect.DOAnchorPos(menuTargetAnchoredPosition, openDuration).SetEase(Ease.OutCubic))
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
            menuTween = DOTween.Sequence()
                .Append(menuPanelRect.DOAnchorPos(Vector2.zero, closeDuration).SetEase(Ease.InCubic))
                .Join(menuPanel.transform.DOScale(openStartScale, closeDuration).SetEase(Ease.InBack))
                .OnComplete(() =>
                {
                    menuPanel.SetActive(false);
                    isMenuOpen = false;
                });
        });
    }

}
